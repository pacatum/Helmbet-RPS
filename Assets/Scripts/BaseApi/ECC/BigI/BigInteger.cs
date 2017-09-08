using System;
using System.Collections.Generic;
using Tools;


namespace BigI {

	public interface IExponential {

		BigInteger Convert( BigInteger x );
		BigInteger Revert( BigInteger x );
		void MultiplyTo( BigInteger x, BigInteger y, BigInteger result );
		void SquareTo( BigInteger x, BigInteger result );
	}


	public class BigInteger : IComparable<BigInteger>, IEquatable<BigInteger> {

		class NullExponential : IExponential {

			public BigInteger Convert( BigInteger x ) {
				return x;
			}

			public BigInteger Revert( BigInteger x ) {
				return x;
			}

			public void SquareTo( BigInteger x, BigInteger result ) {
				x.SquareTo( result );
			}

			public void MultiplyTo( BigInteger x, BigInteger y, BigInteger result ) {
				x.MultiplyTo( y, result );
			}
		}


		class ClassicExponential : IExponential {

			readonly BigInteger m;


			public ClassicExponential( BigInteger m ) {
				this.m = m;
			}

			public BigInteger Convert( BigInteger x ) {
				if ( x.sign < 0 || x.CompareTo( m ) >= 0 ) {
					return x.Modulo( m );
				}
				return x;
			}

			public BigInteger Revert( BigInteger x ) {
				return x;
			}

			public void SquareTo( BigInteger x, BigInteger result ) {
				x.SquareTo( result );
				Reduce( result );
			}

			public void MultiplyTo( BigInteger x, BigInteger y, BigInteger result ) {
				x.MultiplyTo( y, result );
				Reduce( result );
			}

			void Reduce( BigInteger x ) {
				x.DivideRemainderTo( m, null, x );
			}
		}


		class BarrettExponential : IExponential {

			readonly BigInteger m;
			readonly BigInteger mu;
			readonly BigInteger r2;
			readonly BigInteger q3;


			public BarrettExponential( BigInteger m ) {
				r2 = new BigInteger();
				q3 = new BigInteger();
				One.DigitLeftShiftTo( 2 * m.t, r2 );
				mu = r2.Divide( m );
				this.m = m;
			}

			public BigInteger Convert( BigInteger x ) {
				if ( x.sign < 0 || x.t > 2 * m.t ) {
					return x.Modulo( m );
				}
				if ( x.CompareTo( m ) < 0 ) {
					return x;
				}
				var result = new BigInteger();
				x.CopyTo( result );
				Reduce( result );
				return result;
			}

			public BigInteger Revert( BigInteger x ) {
				return x;
			}

			// result = x^2 mod m; x != result
			public void SquareTo( BigInteger x, BigInteger result ) {
				x.SquareTo( result );
				Reduce( result );
			}

			// result = x*y mod m; x,y != result
			public void MultiplyTo( BigInteger x, BigInteger y, BigInteger result ) {
				x.MultiplyTo( y, result );
				Reduce( result );
			}

			// x = x mod m (HAC 14.42)
			void Reduce( BigInteger x ) {
				x.DigitRightShiftTo( m.t - 1, r2 );
				if ( x.t > m.t + 1 ) {
					x.t = m.t + 1;
					x.Clamp();
				}
				mu.MultiplyUpperTo( r2, m.t + 1, q3 );
				m.MultiplyLowerTo( q3, m.t + 1, r2 );
				while ( x.CompareTo( r2 ) < 0 ) {
					x.AddOffset( 1, m.t + 1 );
				}
				x.SubtractTo( r2, x );
				while ( x.CompareTo( m ) >= 0 ) {
					x.SubtractTo( m, x );
				}
			}
		}


		class MontgomeryExponential : IExponential {

			readonly BigInteger m;
			readonly int mp;
			readonly int mpl;
			readonly int mph;
			readonly int um;
			readonly int mt2;


			public MontgomeryExponential( BigInteger m ) {
				this.m = m;
				mp = InverseDigit( m );
				mpl = mp & 0x7fff;
				mph = mp >> 15;
				um = (1 << (DB - 15)) - 1;
				mt2 = 2 * m.t;
			}

			// xR mod m
			public BigInteger Convert( BigInteger x ) {
				var result = new BigInteger();
				x.Abs.DigitLeftShiftTo( m.t, result );
				result.DivideRemainderTo( m, null, result );
				if ( x.sign < 0 && result.CompareTo( Zero ) > 0 ) {
					m.SubtractTo( result, result );
				}
				return result;
			}

			// x/R mod m
			public BigInteger Revert( BigInteger x ) {
				var result = new BigInteger();
				x.CopyTo( result );
				Reduce( result );
				return result;
			}

			// result = "x^2/R mod m"; x != result
			public void SquareTo( BigInteger x, BigInteger result ) {
				x.SquareTo( result );
				Reduce( result );
			}

			// result = "xy/R mod m"; x,y != result
			public void MultiplyTo( BigInteger x, BigInteger y, BigInteger result ) {
				x.MultiplyTo( y, result );
				Reduce( result );
			}

			// x = x/R mod m (HAC 14.32)
			void Reduce( BigInteger x ) {
				while ( x.t <= mt2 ) { // pad x so am has enough room later
					x[ x.t++ ] = 0;
				}
				for ( var i = 0; i < m.t; i++ ) {
					// faster way of calculating u0 = x[i]*mp mod DV
					var j = x[ i ] & 0x7fff;
					var u0 = (j * mpl + (((j * mph + (x[ i ] >> 15) * mpl) & um) << 15)) & DM;
					// use am to combine the multiply-shift-add into one call
					j = i + m.t;
					x[ j ] += ( int )m.Am( 0, u0, x, i, 0, m.t );
					// propagate carry
					while ( x[ j ] >= DV ) {
						x[ j ] -= DV;
						x[ ++j ]++;
					}
				}
				x.Clamp();
				x.DigitRightShiftTo( m.t, x );
				if ( x.CompareTo( m ) >= 0 ) {
					x.SubtractTo( m, x );
				}
			}

			// return "-1/a % 2^DB"
			// justification:
			//         xy == 1 (mod m)
			//         xy =  1+km
			//   xy(2-xy) = (1+km)(1-km)
			// x[y(2-xy)] = 1-k^2m^2
			// x[y(2-xy)] == 1 (mod m^2)
			// if y is 1/x mod m, then y(2-xy) is 1/x mod m^2
			// should reduce x and y(2-xy) by m^2 at each step to keep size bounded.
			static int InverseDigit( BigInteger a ) {
				if ( a.t < 1 ) {
					return 0;
				}
				var x = a[ 0 ];
				if ( (x & 1) == 0 ) {
					return 0;
				}
				var y = x & 3; // y == 1/x mod 2^2
				y = (y * (2 - (x & 0xf) * y)) & 0xf; // y == 1/x mod 2^4
				y = (y * (2 - (x & 0xff) * y)) & 0xff; // y == 1/x mod 2^8
				y = (y * (2 - (((x & 0xffff) * y) & 0xffff))) & 0xffff; // y == 1/x mod 2^16
																		// last step - calculate inverse mod DV directly
																		// assumes 16 < DB <= 32 and assumes ability to handle 48-bit ints
				y = (y * (2 - x * y % DV)) % DV; // y == 1/x mod 2^dbits
												 // we really want the negative inverse, and -DV < y < DV
				return (y > 0) ? DV - y : -y;
			}
		}


		const int BITS_PER_DIGIT = 26;

		static readonly int DB = BITS_PER_DIGIT;
		static readonly int DM = (1 << BITS_PER_DIGIT) - 1;
		static readonly int DV = 1 << BITS_PER_DIGIT;

		static readonly int BI_FP = 52;
		static readonly long FV = ( long )Math.Pow( 2, BI_FP );
		static readonly int F1 = BI_FP - BITS_PER_DIGIT;
		static readonly int F2 = 2 * BITS_PER_DIGIT - BI_FP;

		// Digit conversions
		public static readonly string BI_RM = "0123456789abcdefghijklmnopqrstuvwxyz";
		public static readonly Dictionary<char, int> BI_RC = new Dictionary<char, int> {
			{ '0', 0 },     { '1', 1 },     { '2', 2 },     { '3', 3 },     { '4', 4 },     { '5', 5 },     { '6', 6 },     { '7', 7 },     { '8', 8 },     { '9', 9 },
			{ 'a', 10 },    { 'b', 11 },    { 'c', 12 },    { 'd', 13 },    { 'e', 14 },    { 'f', 15 },    { 'g', 16 },    { 'h', 17 },    { 'i', 18 },    { 'j', 19 },
			{ 'k', 20 },    { 'l', 21 },    { 'm', 22 },    { 'n', 23 },    { 'o', 24 },    { 'p', 25 },    { 'q', 26 },    { 'r', 27 },    { 's', 28 },    { 't', 29 },
			{ 'u', 30 },    { 'v', 31 },    { 'w', 32 },    { 'x', 33 },    { 'y', 34 },    { 'z', 35 },
			{ 'A', 10 },    { 'B', 11 },    { 'C', 12 },    { 'D', 13 },    { 'E', 14 },    { 'F', 15 },    { 'G', 16 },    { 'H', 17 },    { 'I', 18 },    { 'J', 19 },
			{ 'K', 20 },    { 'L', 21 },    { 'M', 22 },    { 'N', 23 },    { 'O', 24 },    { 'P', 25 },    { 'Q', 26 },    { 'R', 27 },    { 'S', 28 },    { 'T', 29 },
			{ 'U', 30 },    { 'V', 31 },    { 'W', 32 },    { 'X', 33 },    { 'Y', 34 },    { 'Z', 35 }
		};

		public static readonly BigInteger Zero = ValueOf( 0 );
		public static readonly BigInteger One = ValueOf( 1 );
		public static readonly BigInteger Two = ValueOf( 2 );
		public static readonly BigInteger Three = ValueOf( 3 );


		int t = 0;
		int sign = 0; // -1 or 0
		int[] magnitude = new int[ 0 ];


		int this[ int index ] {
			get { return magnitude[ index ]; }
			set {
				if ( index + 1 > magnitude.Length ) {
					Array.Resize( ref magnitude, index + 1 );
				}
				magnitude[ index ] = value;
			}
		}

		// Am: Compute w_j += (x*this_i), propagate carries,
		// c is initial carry, returns final carry.
		// c < 3*dvalue, x < 2*dvalue, this_i < dvalue
		// We need to select the fastest one that works in this environment.
		long Am( int i, int x, BigInteger w, int j, long c, int n ) {
			return Am1( i, x, w, j, c, n );
		}

		// Am1: use a single mult and divide to get the high bits,
		// max digit bits should be 26 because
		// max internal value = 2*dvalue^2-2*dvalue (< 2^53)
		long Am1( int i, int x, BigInteger w, int j, long c, int n ) {
			while ( --n >= 0 ) {
				var value = ( ulong )x * ( ulong )this[ i++ ] + ( ulong )w[ j ] + ( ulong )c;
				c = ( long )Math.Floor( ( double )value / 0x4000000 );
				w[ j++ ] = ( int )(value & 0x3ffffff);
			}
			return c;
		}

		// Am2 avoids a big mult-and-extract completely.
		// Max digit bits should be <= 30 because we do bitwise ops
		// on values up to 2*hdvalue^2-hdvalue-1 (< 2^31)
		long Am2( int i, int x, BigInteger w, int j, long c, int n ) {
			long xl = x & 0x7fff;
			long xh = x >> 15;
			while ( --n >= 0 ) {
				long l = this[ i ] & 0x7fff;
				long h = this[ i++ ] >> 15;
				var m = xh * l + h * xl;
				l = xl * l + ((m & 0x7fff) << 15) + w[ j ] + (c & 0x3fffffff);
				c = xh * h + ( long )(((( ulong )l) >> 30) + ((( ulong )m) >> 15) + ((( ulong )c) >> 30));
				w[ j++ ] = ( int )(l & 0x3fffffff);
			}
			return c;
		}

		// Alternately, set max digit bits to 28 since some
		// browsers slow down when dealing with 32-bit numbers.
		long Am3( int i, int x, BigInteger w, int j, long c, int n ) {
			long xl = x & 0x3fff;
			long xh = x >> 14;
			while ( --n >= 0 ) {
				long l = this[ i ] & 0x3fff;
				long h = this[ i++ ] >> 14;
				var m = xh * l + h * xl;
				l = xl * l + ((m & 0x3fff) << 14) + w[ j ] + c;
				c = (l >> 28) + (m >> 14) + xh * h;
				w[ j++ ] = ( int )(l & 0xfffffff);
			}
			return c;
		}

		BigInteger() { }

		public BigInteger( int value ) : this( value.ToString(), 10 ) { }

		public BigInteger( string value ) : this( value, 10 ) { }

		// set from string and radix
		public BigInteger( string str, int radix ) {
			var k = 0;
			switch ( radix ) {
			case 16:
				k = 4;
				break;
			case 8:
				k = 3;
				break;
			case 256:
				k = 8; // byte array
				break;
			case 2:
				k = 1;
				break;
			case 32:
				k = 5;
				break;
			case 4:
				k = 2;
				break;
			default:
				FromRadix( str, radix );
				return;
			}
			t = 0;
			sign = 0;
			var i = str.Length;
			var minus = false;
			var sh = 0;
			while ( --i >= 0 ) {
				var x = (k == 8) ? str[ i ] & 0xff : IntAt( str, i );
				if ( x < 0 ) {
					minus |= str[ i ].Equals( '-' );
					continue;
				}
				minus = false;
				if ( sh == 0 ) {
					this[ t++ ] = x;
				} else if ( sh + k > DB ) {
					this[ t - 1 ] |= ((x & ((1 << (DB - sh)) - 1)) << sh);
					this[ t++ ] = x >> (DB - sh);
				} else {
					this[ t - 1 ] |= x << sh;
				}
				sh += k;
				if ( sh >= DB ) {
					sh -= DB;
				}
			}
			if ( k == 8 && (str[ 0 ] & 0x80) != 0 ) {
				sign = -1;
				if ( sh > 0 ) {
					this[ t - 1 ] |= ((1 << (DB - sh)) - 1) << sh;
				}
			}
			Clamp();
			if ( minus ) {
				Zero.SubtractTo( this, this );
			}
		}

		// convert to radix string
		string ToRadix( int b ) {
			if ( Sign == 0 || b < 2 || b > 36 ) {
				return "0";
			}
			var cs = ChunkSize( b );
			var a = ( int )Math.Pow( b, cs );
			var d = ValueOf( a );
			var y = new BigInteger();
			var z = new BigInteger();
			var result = string.Empty;
			DivideRemainderTo( d, y, z );
			while ( y.Sign > 0 ) {
				result = (a + z.IntValue).ToString( b ).Substring( 1 ) + result;
				y.DivideRemainderTo( d, y, z );
			}
			return z.IntValue.ToString( b ) + result;
		}

		// convert from radix string
		void FromRadix( string str, int b ) {
			FromInt( 0 );
			var cs = ChunkSize( b );
			var d = ( int )Math.Pow( b, cs );
			var minus = false;
			var j = 0;
			var w = 0;
			for ( var i = 0; i < str.Length; i++ ) {
				var x = IntAt( str, i );
				if ( x < 0 ) {
					minus |= (str[ i ].Equals( '-' ) && Sign == 0);
					continue;
				}
				w = b * w + x;
				if ( ++j >= cs ) {
					Multiply( d );
					AddOffset( w, 0 );
					j = 0;
					w = 0;
				}
			}
			if ( j > 0 ) {
				Multiply( ( int )Math.Pow( b, j ) );
				AddOffset( w, 0 );
			}
			if ( minus ) {
				Zero.SubtractTo( this, this );
			}
		}

		// return value as integer
		int IntValue {
			get {
				if ( sign < 0 ) {
					if ( t == 1 ) {
						return this[ 0 ] - DV;
					}
					if ( t == 0 ) {
						return -1;
					}
				} else if ( t == 1 ) {
					return this[ 0 ];
				} else if ( t == 0 ) {
					return 0;
				}
				// assumes 16 < DB < 32
				return ((this[ 1 ] & ((1 << (32 - DB)) - 1)) << DB) | this[ 0 ];
			}
		}

		// return value as byte
		byte ByteValue {
			get { return ( byte )((t == 0) ? sign : ((this[ 0 ] << 24) >> 24)); }
		}

		// return value as short (assumes DB>=16)
		short ShortValue {
			get { return ( short )((t == 0) ? sign : ((this[ 0 ] << 16) >> 16)); }
		}

		// return x s.t. r^x < DV
		int ChunkSize( int r ) {
			return ( int )Math.Floor( Math.Log( 2 ) * DB / Math.Log( r ) );
		}

		static char Int2Char( int n ) {
			return BI_RM[ n ];
		}

		static int IntAt( string s, int i ) {
			if ( BI_RC.ContainsKey( s[ i ] ) ) {
				return BI_RC[ s[ i ] ];
			}
			return -1;
		}

		// 0 if this == 0, 1 if this > 0
		public int Sign {
			get {
				if ( sign < 0 ) {
					return -1;
				}
				if ( t <= 0 || (t == 1 && this[ 0 ] <= 0) ) {
					return 0;
				}
				return 1;
			}
		}

		// return the number of bits in "this"
		public int BitLength {
			get { return (t <= 0) ? 0 : (DB * (t - 1) + NumberOfBits( this[ t - 1 ] ^ (sign & DM) )); }
		}

		// return the number of bytes in "this"
		public int ByteLength {
			get { return BitLength >> 3; }
		}

		// true if this is even
		public bool IsEven {
			get { return ((t > 0) ? (this[ 0 ] & 1) : sign) == 0; }
		}

		// |this|
		public BigInteger Abs {
			get { return (sign < 0) ? Negate : this; }
		}

		// -this
		public BigInteger Negate {
			get {
				var result = new BigInteger();
				Zero.SubtractTo( this, result );
				return result;
			}
		}

		// copy this
		public BigInteger Clone {
			get {
				var result = new BigInteger();
				CopyTo( result );
				return result;
			}
		}

		// this + a
		public BigInteger Addition( BigInteger a ) {
			var result = new BigInteger();
			AdditionTo( a, result );
			return result;
		}

		// this - a
		public BigInteger Subtract( BigInteger a ) {
			var result = new BigInteger();
			SubtractTo( a, result );
			return result;
		}

		// this * a
		public BigInteger Multiply( BigInteger a ) {
			var result = new BigInteger();
			MultiplyTo( a, result );
			return result;
		}

		// this^2
		public BigInteger Square {
			get {
				var result = new BigInteger();
				SquareTo( result );
				return result;
			}
		}

		// this / a
		public BigInteger Divide( BigInteger a ) {
			var result = new BigInteger();
			DivideRemainderTo( a, result, null );
			return result;
		}

		// this % a
		public BigInteger Remainder( BigInteger a ) {
			var result = new BigInteger();
			DivideRemainderTo( a, null, result );
			return result;
		}

		// true iff nth bit is set
		public bool TestBit( int n ) {
			var j = ( int )Math.Floor( ( double )n / DB );
			if ( j >= t ) {
				return (sign != 0);
			}
			return (this[ j ] & (1 << (n % DB))) != 0;
		}

		// this^e
		public BigInteger Power( int e ) {
			return Exponential( e, new NullExponential() );
		}

		// this mod a
		public BigInteger Modulo( BigInteger a ) {
			var result = new BigInteger();
			Abs.DivideRemainderTo( a, null, result );
			if ( sign < 0 && result.CompareTo( Zero ) > 0 ) {
				a.SubtractTo( result, result );
			}
			return result;
		}

		// this^e % m (HAC 14.85)
		public BigInteger ModuloPower( BigInteger e, BigInteger m ) {
			var i = e.BitLength;
			var k = 0;
			var result = ValueOf( 1 );
			IExponential z = null;
			if ( i <= 0 ) {
				return result;
			}
			if ( i < 18 ) {
				k = 1;
			} else if ( i < 48 ) {
				k = 3;
			} else if ( i < 144 ) {
				k = 4;
			} else if ( i < 768 ) {
				k = 5;
			} else {
				k = 6;
			}
			if ( i < 8 ) {
				z = new ClassicExponential( m );
			} else if ( m.IsEven ) {
				z = new BarrettExponential( m );
			} else {
				z = new MontgomeryExponential( m );
			}

			// precomputation
			var g = new BigInteger[ 2 ];
			var n = 3;
			var k1 = k - 1;
			var km = (1 << k) - 1;
			g[ 1 ] = z.Convert( this );
			if ( k > 1 ) {
				var g2 = new BigInteger();
				z.SquareTo( g[ 1 ], g2 );
				while ( n <= km ) {
					if ( n + 1 > g.Length ) {
						Array.Resize( ref g, n + 1 );
					}
					g[ n ] = new BigInteger();
					z.MultiplyTo( g2, g[ n - 2 ], g[ n ] );
					n += 2;
				}
			}
			var j = e.t - 1;
			var isFirstIteration = true;
			var result2 = new BigInteger();
			i = NumberOfBits( e[ j ] ) - 1;
			while ( j >= 0 ) {
				var w = 0;
				if ( i >= k1 ) {
					w = (e[ j ] >> (i - k1)) & km;
				} else {
					w = (e[ j ] & ((1 << (i + 1)) - 1)) << (k1 - i);
					if ( j > 0 ) {
						w |= e[ j - 1 ] >> (DB + i - k1);
					}
				}
				n = k;
				while ( (w & 1) == 0 ) {
					w = w >> 1;
					n--;
				}
				if ( (i -= n) < 0 ) {
					i += DB;
					j--;
				}
				if ( isFirstIteration ) { // ret == 1, don't bother squaring or multiplying it
					g[ w ].CopyTo( result );
					isFirstIteration = false;
				} else {
					while ( n > 1 ) {
						z.SquareTo( result, result2 );
						z.SquareTo( result2, result );
						n -= 2;
					}
					if ( n > 0 ) {
						z.SquareTo( result, result2 );
					} else {
						var temp = result;
						result = result2;
						result2 = temp;
					}
					z.MultiplyTo( result2, g[ w ], result );
				}
				while ( j >= 0 && (e[ j ] & (1 << i)) == 0 ) {
					z.SquareTo( result, result2 );
					var temp = result;
					result = result2;
					result2 = temp;
					if ( --i < 0 ) {
						i = DB - 1;
						j--;
					}
				}
			}
			return z.Revert( result );
		}

		// this^e % m, 0 <= e < 2^32
		BigInteger ModuloPowerInt( long e, BigInteger m ) {
			if ( e < 256 || m.IsEven ) {
				return Exponential( e, new ClassicExponential( m ) );
			}
			return Exponential( e, new MontgomeryExponential( m ) );
		}

		// 1/this % m (HAC 14.61)
		public BigInteger ModuloInverse( BigInteger m ) {
			var ac = m.IsEven;
			if ( Sign == 0 ) {
				throw new InvalidOperationException( "division by zero" );
			}
			if ( (IsEven && ac) || m.Sign == 0 ) {
				return Zero;
			}
			var u = m.Clone;
			var v = Clone;
			var a = ValueOf( 1 );
			var b = ValueOf( 0 );
			var c = ValueOf( 0 );
			var d = ValueOf( 1 );
			while ( u.Sign != 0 ) {
				while ( u.IsEven ) {
					u.RightShiftTo( 1, u );
					if ( ac ) {
						if ( !a.IsEven || !b.IsEven ) {
							a.AdditionTo( this, a );
							b.SubtractTo( m, b );
						}
						a.RightShiftTo( 1, a );
					} else if ( !b.IsEven ) {
						b.SubtractTo( m, b );
					}
					b.RightShiftTo( 1, b );
				}
				while ( v.IsEven ) {
					v.RightShiftTo( 1, v );
					if ( ac ) {
						if ( !c.IsEven || !d.IsEven ) {
							c.AdditionTo( this, c );
							d.SubtractTo( m, d );
						}
						c.RightShiftTo( 1, c );
					} else if ( !d.IsEven ) {
						d.SubtractTo( m, d );
					}
					d.RightShiftTo( 1, d );
				}
				if ( u.CompareTo( v ) >= 0 ) {
					u.SubtractTo( v, u );
					if ( ac ) {
						a.SubtractTo( c, a );
					}
					b.SubtractTo( d, b );
				} else {
					v.SubtractTo( u, v );
					if ( ac ) {
						c.SubtractTo( a, c );
					}
					d.SubtractTo( b, d );
				}
			}
			if ( v.CompareTo( One ) != 0 ) {
				return Zero;
			}
			while ( d.CompareTo( m ) >= 0 ) {
				d.SubtractTo( m, d );
			}
			while ( d.Sign < 0 ) {
				d.AdditionTo( m, d );
			}
			return d;
		}

		// this << n
		public BigInteger ShiftLeft( int n ) {
			var result = new BigInteger();
			if ( n < 0 ) {
				RightShiftTo( -n, result );
			} else {
				LeftShiftTo( n, result );
			}
			return result;
		}

		// this >> n
		public BigInteger ShiftRight( int n ) {
			var r = new BigInteger();
			if ( n < 0 ) {
				LeftShiftTo( -n, r );
			} else {
				RightShiftTo( n, r );
			}
			return r;
		}

		// return + if this > a, - if this < a, 0 if equal
		public int CompareTo( BigInteger a ) {
			var result = sign - a.sign;
			if ( result != 0 ) {
				return result;
			}
			result = t - a.t;
			if ( result != 0 ) {
				return (sign < 0) ? -result : result;
			}
			var i = t;
			while ( --i >= 0 ) {
				if ( (result = this[ i ] - a[ i ]) != 0 ) {
					return result;
				}
			}
			return 0;
		}

		public bool Equals( BigInteger a ) {
			return CompareTo( a ) == 0;
		}

		public BigInteger Min( BigInteger a ) {
			return (CompareTo( a ) < 0) ? this : a;
		}

		public BigInteger Max( BigInteger a ) {
			return (CompareTo( a ) > 0) ? this : a;
		}

		public override string ToString() {
			return ToString( 10 );
		}

		// return string representation in given radix
		public string ToString( int radix ) {
			if ( sign < 0 ) {
				return "-" + Negate.ToString( radix );
			}
			var k = 0;
			switch ( radix ) {
			case 16:
				k = 4;
				break;
			case 8:
				k = 3;
				break;
			case 2:
				k = 1;
				break;
			case 32:
				k = 5;
				break;
			case 4:
				k = 2;
				break;
			default:
				return ToRadix( radix );
			}
			var km = (1 << k) - 1;
			var d = 0;
			var m = false;
			var result = string.Empty;
			var i = t;
			var p = DB - (i * DB) % k;
			if ( i-- > 0 ) {
				if ( p < DB && (d = this[ i ] >> p) > 0 ) {
					m = true;
					result = Int2Char( d ).ToString();
				}
				while ( i >= 0 ) {
					if ( p < k ) {
						d = (this[ i ] & ((1 << p) - 1)) << (k - p);
						d |= this[ --i ] >> (p += DB - k);
					} else {
						d = (this[ i ] >> (p -= k)) & km;
						if ( p <= 0 ) {
							p += DB;
							i--;
						}
					}
					m |= d > 0;
					if ( m ) {
						result += Int2Char( d );
					}
				}
			}
			return m ? result : "0";
		}

		// convert to bigendian byte array
		byte[] ToByteArray() {
			var i = t;
			var result = new int[] { sign };
			var p = DB - (i * DB) % 8;
			var d = 0;
			var k = 0;
			if ( i-- > 0 ) {
				if ( p < DB && (d = (this[ i ] >> p)) != ((sign & DM) >> p) ) {
					result[ k++ ] = d | (sign << (DB - p));
				}
				while ( i >= 0 ) {
					if ( p < 8 ) {
						d = (this[ i ] & ((1 << p) - 1)) << (8 - p);
						d |= this[ --i ] >> (p += DB - 8);
					} else {
						d = (this[ i ] >> (p -= 8)) & 0xff;
						if ( p <= 0 ) {
							p += DB;
							i--;
						}
					}
					if ( (d & 0x80) != 0 ) {
						d |= -256;
					}
					if ( k == 0 && (sign & 0x80) != (d & 0x80) ) {
						k++;
					}
					if ( k > 0 || d != sign ) {
						if ( k > result.Length - 1 ) {
							Array.Resize( ref result, k + 1 );
						}
						result[ k++ ] = d;
					}
				}
			}
			return Array.ConvertAll( result, item => ( byte )item );
		}

		public byte[] ToBuffer() {
			return ToByteArrayUnsigned();
		}

		public byte[] ToDERInteger() {
			return ToByteArray();
		}

		byte[] ToByteArrayUnsigned() {
			var byteArray = ToByteArray();
			return byteArray[ 0 ] == 0 ? byteArray.Slice( 1 ) : byteArray;
		}

		public byte[] ToBuffer( int size ) {
			var byteArray = ToByteArrayUnsigned();
			var zeros = new byte[ Math.Max( 0, size - byteArray.Length ) ].Fill( ( byte )0 );
			return zeros.Concat( byteArray );
		}

		public static BigInteger FromBuffer( byte[] buffer ) {
			// BigInteger expects a DER integer conformant byte array
			if ( (buffer[ 0 ] & 0x80) != 0 ) {
				buffer = new byte[] { 0 }.Concat( buffer );
			}
			var data = new string( Array.ConvertAll( buffer, b => ( char )b ) );
			return new BigInteger( data, 256 );
		}

		public static BigInteger ValueOf( int i ) {
			var result = new BigInteger();
			result.FromInt( i );
			return result;
		}

		// set from integer value x, -DV <= x < DV
		void FromInt( int x ) {
			t = 1;
			sign = (x < 0) ? -1 : 0;
			if ( x > 0 ) {
				this[ 0 ] = x;
			} else if ( x < -1 ) {
				this[ 0 ] = x + DV;
			} else {
				t = 0;
			}
		}

		// returns bit length of the integer value
		int NumberOfBits( int value ) {
			var result = 1;
			var temp = 0;
			if ( (temp = ( int )((( uint )value) >> 16)) != 0 ) {
				value = temp;
				result += 16;
			}
			if ( (temp = value >> 8) != 0 ) {
				value = temp;
				result += 8;
			}
			if ( (temp = value >> 4) != 0 ) {
				value = temp;
				result += 4;
			}
			if ( (temp = value >> 2) != 0 ) {
				value = temp;
				result += 2;
			}
			if ( (temp = value >> 1) != 0 ) {
				value = temp;
				result += 1;
			}
			return result;
		}

		// copy this to result
		void CopyTo( BigInteger result ) {
			for ( var i = t - 1; i >= 0; i-- ) {
				result[ i ] = this[ i ];
			}
			result.t = t;
			result.sign = sign;
		}

		// result = this + a
		void AdditionTo( BigInteger a, BigInteger result ) {
			var i = 0;
			var c = 0;
			var minT = Math.Min( a.t, t );
			while ( i < minT ) {
				c += this[ i ] + a[ i ];
				result[ i++ ] = c & DM;
				c >>= DB;
			}
			if ( a.t < t ) {
				c += a.sign;
				while ( i < t ) {
					c += this[ i ];
					result[ i++ ] = c & DM;
					c >>= DB;
				}
				c += sign;
			} else {
				c += sign;
				while ( i < a.t ) {
					c += a[ i ];
					result[ i++ ] = c & DM;
					c >>= DB;
				}
				c += a.sign;
			}
			result.sign = (c < 0) ? -1 : 0;
			if ( c > 0 ) {
				result[ i++ ] = c;
			} else if ( c < -1 ) {
				result[ i++ ] = DV + c;
			}
			result.t = i;
			result.Clamp();
		}

		// result = this - a
		void SubtractTo( BigInteger a, BigInteger result ) {
			var i = 0;
			var c = 0;
			var minT = Math.Min( a.t, t );
			while ( i < minT ) {
				c += this[ i ] - a[ i ];
				result[ i++ ] = c & DM;
				c >>= DB;
			}
			if ( a.t < t ) {
				c -= a.sign;
				while ( i < t ) {
					c += this[ i ];
					result[ i++ ] = c & DM;
					c >>= DB;
				}
				c += sign;
			} else {
				c += sign;
				while ( i < a.t ) {
					c -= a[ i ];
					result[ i++ ] = c & DM;
					c >>= DB;
				}
				c -= a.sign;
			}
			result.sign = (c < 0) ? -1 : 0;
			if ( c < -1 ) {
				result[ i++ ] = DV + c;
			} else if ( c > 0 ) {
				result[ i++ ] = c;
			}
			result.t = i;
			result.Clamp();
		}

		// result = this * a, result != this,a (HAC 14.12)
		// "this" should be the larger one if appropriate.
		void MultiplyTo( BigInteger a, BigInteger result ) {
			var x = Abs;
			var y = a.Abs;
			result.t = x.t + y.t;
			var i = x.t;
			while ( --i >= 0 ) {
				result[ i ] = 0;
			}
			for ( i = 0; i < y.t; i++ ) {
				result[ i + x.t ] = ( int )x.Am( 0, y[ i ], result, i, 0, x.t );
			}
			result.sign = 0;
			result.Clamp();
			if ( sign != a.sign ) {
				Zero.SubtractTo( result, result );
			}
		}

		// result = this^2, result != this (HAC 14.16)
		void SquareTo( BigInteger result ) {
			var x = Abs;
			var i = result.t = 2 * x.t;
			while ( --i >= 0 ) {
				result[ i ] = 0;
			}
			for ( i = 0; i < x.t - 1; i++ ) {
				var c = x.Am( i, x[ i ], result, 2 * i, 0, 1 );
				if ( (result[ i + x.t ] += ( int )x.Am( i + 1, 2 * x[ i ], result, 2 * i + 1, c, x.t - i - 1 )) >= DV ) {
					result[ i + x.t ] -= DV;
					result[ i + x.t + 1 ] = 1;
				}
			}
			if ( result.t > 0 ) {
				result[ result.t - 1 ] += ( int )x.Am( i, x[ i ], result, 2 * i, 0, 1 );
			}
			result.sign = 0;
			result.Clamp();
		}

		// divide this by measure, quotient and remainder to quotient, remainder (HAC 14.20)
		// remainder != quotient, this != measure. quotient or remainder may be null.
		void DivideRemainderTo( BigInteger measure, BigInteger quotient, BigInteger remainder ) {
			var pm = measure.Abs;
			if ( pm.t <= 0 ) {
				return;
			}
			var pt = Abs;
			if ( pt.t < pm.t ) {
				if ( quotient != null ) {
					quotient.FromInt( 0 );
				}
				if ( remainder != null ) {
					CopyTo( remainder );
				}
				return;
			}
			remainder = remainder ?? new BigInteger();
			var y = new BigInteger();
			var ts = sign;
			var ms = measure.sign;
			var nsh = DB - NumberOfBits( pm[ pm.t - 1 ] ); // normalize modulus
			if ( nsh > 0 ) {
				pm.LeftShiftTo( nsh, y );
				pt.LeftShiftTo( nsh, remainder );
			} else {
				pm.CopyTo( y );
				pt.CopyTo( remainder );
			}
			var ys = y.t;
			var y0 = y[ ys - 1 ];
			if ( y0 == 0 ) {
				return;
			}
			var yt = ( double )y0 * (1 << F1) + ((ys > 1) ? (y[ ys - 2 ] >> F2) : 0);
			var d1 = FV / yt;
			var d2 = (1 << F1) / yt;
			var e = 1 << F2;
			var i = remainder.t;
			var j = i - ys;
			var temp = quotient ?? new BigInteger();
			y.DigitLeftShiftTo( j, temp );
			if ( remainder.CompareTo( temp ) >= 0 ) {
				remainder[ remainder.t++ ] = 1;
				remainder.SubtractTo( temp, remainder );
			}
			One.DigitLeftShiftTo( ys, temp );
			temp.SubtractTo( y, y ); // "negative" y so we can replace sub with am later
			while ( y.t < ys ) {
				y[ y.t++ ] = 0;
			}
			while ( --j >= 0 ) {
				// Estimate quotient digit
				var qd = (remainder[ --i ] == y0) ? DM : ( int )Math.Floor( remainder[ i ] * d1 + (remainder[ i - 1 ] + e) * d2 );
				if ( (remainder[ i ] += ( int )y.Am( 0, qd, remainder, j, 0, ys )) < qd ) { // Try it out
					y.DigitLeftShiftTo( j, temp );
					remainder.SubtractTo( temp, remainder );
					while ( remainder[ i ] < --qd ) {
						remainder.SubtractTo( temp, remainder );
					}
				}
			}
			if ( quotient != null ) {
				remainder.DigitRightShiftTo( ys, quotient );
				if ( ts != ms ) {
					Zero.SubtractTo( quotient, quotient );
				}
			}
			remainder.t = ys;
			remainder.Clamp();
			if ( nsh > 0 ) {
				remainder.RightShiftTo( nsh, remainder ); // Denormalize remainder
			}
			if ( ts < 0 ) {
				Zero.SubtractTo( remainder, remainder );
			}
		}

		// this^e, e < 2^32, doing sqr and mul with "r" (HAC 14.79)
		BigInteger Exponential( long e, IExponential z ) {
			if ( e > 0xffffffff || e < 1 ) {
				return One;
			}
			var result = new BigInteger();
			var result2 = new BigInteger();
			var g = z.Convert( this );
			var i = NumberOfBits( ( int )e ) - 1;
			g.CopyTo( result );
			while ( --i >= 0 ) {
				z.SquareTo( result, result2 );
				if ( (e & (1 << i)) > 0 ) {
					z.MultiplyTo( result2, g, result );
				} else {
					var temp = result;
					result = result2;
					result2 = temp;
				}
			}
			return z.Revert( result );
		}

		// result = this << bitNumber
		void LeftShiftTo( int bitNumber, BigInteger result ) {
			var bs = bitNumber % DB;
			var cbs = DB - bs;
			var bm = (1 << cbs) - 1;
			var ds = ( int )Math.Floor( ( double )bitNumber / DB );

			var c = (sign << bs) & DM;
			for ( var i = t - 1; i >= 0; i-- ) {
				result[ i + ds + 1 ] = (this[ i ] >> cbs) | c;
				c = (this[ i ] & bm) << bs;
			}
			for ( var i = ds - 1; i >= 0; i-- ) {
				result[ i ] = 0;
			}
			result[ ds ] = c;
			result.t = t + ds + 1;
			result.sign = sign;
			result.Clamp();
		}

		// result = this << n*DB
		void DigitLeftShiftTo( int n, BigInteger result ) {
			for ( var i = t - 1; i >= 0; i-- ) {
				result[ i + n ] = this[ i ];
			}
			for ( var i = n - 1; i >= 0; i-- ) {
				result[ i ] = 0;
			}
			result.t = t + n;
			result.sign = sign;
		}

		// result = this >> bitNumber
		void RightShiftTo( int bitNumber, BigInteger result ) {
			result.sign = sign;
			var ds = ( int )Math.Floor( ( double )bitNumber / DB );
			if ( ds >= t ) {
				result.t = 0;
				return;
			}
			var bs = bitNumber % DB;
			var cbs = DB - bs;
			var bm = (1 << bs) - 1;
			result[ 0 ] = this[ ds ] >> bs;
			for ( var i = ds + 1; i < t; i++ ) {
				result[ i - ds - 1 ] |= (this[ i ] & bm) << cbs;
				result[ i - ds ] = this[ i ] >> bs;
			}
			if ( bs > 0 ) {
				result[ t - ds - 1 ] |= (sign & bm) << cbs;
			}
			result.t = t - ds;
			result.Clamp();
		}

		// result = this >> n*DB
		void DigitRightShiftTo( int n, BigInteger result ) {
			for ( var i = n; i < t; i++ ) {
				result[ i - n ] = this[ i ];
			}
			result.t = Math.Max( t - n, 0 );
			result.sign = sign;
		}

		// clamp off excess high words
		void Clamp() {
			var c = sign & DM;
			while ( t > 0 && this[ t - 1 ] == c ) {
				t--;
			}
		}

		// this *= n, this >= 0, 1 < n < DV
		void Multiply( int n ) {
			this[ t ] = ( int )Am( 0, n - 1, this, 0, 0, t );
			t++;
			Clamp();
		}

		// this += n << w words, this >= 0
		void AddOffset( int n, int w ) {
			if ( n == 0 ) {
				return;
			}
			while ( t <= w ) {
				this[ t++ ] = 0;
			}
			this[ w ] += n;
			while ( this[ w ] >= DV ) {
				this[ w ] -= DV;
				if ( ++w >= t ) {
					this[ t++ ] = 0;
				}
				this[ w ]++;
			}
		}

		// result = lower n words of "this * a", a.t <= n
		// "this" should be the larger one if appropriate.
		void MultiplyLowerTo( BigInteger a, int n, BigInteger result ) {
			var i = result.t = Math.Min( t + a.t, n );
			result.sign = 0; // assumes a, this >= 0
			while ( i > 0 ) {
				result[ --i ] = 0;
			}
			var j = result.t - t;
			while ( i < j ) {
				result[ i + t ] = ( int )Am( 0, a[ i ], result, i, 0, t );
				i++;
			}
			j = Math.Min( a.t, n );
			while ( i < j ) {
				Am( 0, a[ i ], result, i, 0, n - i );
				i++;
			}
			result.Clamp();
		}

		// result = "this * a" without lower n words, n > 0
		// "this" should be the larger one if appropriate.
		void MultiplyUpperTo( BigInteger a, int n, BigInteger result ) {
			n--;
			var i = result.t = t + a.t - n;
			result.sign = 0; // assumes a, this >= 0
			while ( --i >= 0 ) {
				result[ i ] = 0;
			}
			for ( i = Math.Max( n - t, 0 ); i < a.t; i++ ) {
				result[ t + i - n ] = ( int )Am( n - i, a[ i ], result, 0, 0, t + i - n );
			}
			result.Clamp();
			result.DigitRightShiftTo( 1, result );
		}
	}
}