using System;
using BigI;
using Tools;


namespace ECurve {

	public class Point : IEquatable<Point> {

		readonly Curve curve;
		readonly BigInteger x;
		readonly BigInteger y;
		readonly BigInteger z;

		BigInteger zInv = null;


		public Point( Curve curve, BigInteger x, BigInteger y, BigInteger z ) {
			this.curve = curve;
			this.x = x;
			this.y = y;
			this.z = z;
		}

		BigInteger ZInv {
			get { return zInv ?? (zInv = z.ModuloInverse( curve.P )); }
		}

		public BigInteger X {
			get { return x; }
		}

		public BigInteger Y {
			get { return y; }
		}

		public BigInteger Z {
			get { return z; }
		}

		public BigInteger AffineX {
			get { return x.Multiply( ZInv ).Modulo( curve.P ); }
		}

		public BigInteger AffineY {
			get { return y.Multiply( ZInv ).Modulo( curve.P ); }
		}

		public static Point FromAffine( Curve curve, BigInteger x, BigInteger y ) {
			return new Point( curve, x, y, BigInteger.One );
		}

		public bool Equals( Point other ) {
			if ( other == this ) {
				return true;
			}
			if ( curve.IsInfinity( this ) ) {
				return curve.IsInfinity( other );
			}
			if ( curve.IsInfinity( other ) ) {
				return curve.IsInfinity( this );
			}

			// u = Y2 * Z1 - Y1 * Z2
			var u = other.y.Multiply( z ).Subtract( y.Multiply( other.z ) ).Modulo( curve.P );

			if ( u.Sign != 0 ) {
				return false;
			}

			// v = X2 * Z1 - X1 * Z2
			var v = other.x.Multiply( z ).Subtract( x.Multiply( other.z ) ).Modulo( curve.P );

			return v.Sign == 0;
		}

		Point Negate {
			get { return new Point( curve, x, curve.P.Subtract( y ), z ); }
		}

		public Point Addition( Point b ) {
			if ( curve.IsInfinity( this ) ) {
				return b;
			}
			if ( curve.IsInfinity( b ) ) {
				return this;
			}

			var x1 = x;
			var y1 = y;
			var x2 = b.x;
			var y2 = b.y;

			// u = Y2 * Z1 - Y1 * Z2
			var u = y2.Multiply( z ).Subtract( y1.Multiply( b.z ) ).Modulo( curve.P );
			// v = X2 * Z1 - X1 * Z2
			var v = x2.Multiply( z ).Subtract( x1.Multiply( b.z ) ).Modulo( curve.P );

			if ( v.Sign == 0 ) {
				if ( u.Sign == 0 ) {
					return Twice; // this == b, so double
				}
				return curve.Infinity; // this = -b, so infinity
			}

			var v2 = v.Square;
			var v3 = v2.Multiply( v );
			var x1v2 = x1.Multiply( v2 );
			var zu2 = u.Square.Multiply( z );

			// x3 = v * (z2 * (z1 * u^2 - 2 * x1 * v^2) - v^3)
			var x3 = zu2.Subtract( x1v2.ShiftLeft( 1 ) ).Multiply( b.z ).Subtract( v3 ).Multiply( v ).Modulo( curve.P );
			// y3 = z2 * (3 * x1 * u * v^2 - y1 * v^3 - z1 * u^3) + u * v^3
			var y3 = x1v2.Multiply( BigInteger.Three ).Multiply( u ).Subtract( y1.Multiply( v3 ) ).Subtract( zu2.Multiply( u ) ).Multiply( b.z ).Addition( u.Multiply( v3 ) ).Modulo( curve.P );
			// z3 = v^3 * z1 * z2
			var z3 = v3.Multiply( z ).Multiply( b.z ).Modulo( curve.P );

			return new Point( curve, x3, y3, z3 );
		}

		Point Twice {
			get {
				if ( curve.IsInfinity( this ) ) {
					return this;
				}
				if ( y.Sign == 0 ) {
					return curve.Infinity;
				}

				var x1 = x;
				var y1 = y;

				var y1z1 = y1.Multiply( z );
				var y1sqz1 = y1z1.Multiply( y1 ).Modulo( curve.P );
				var a = curve.A;

				// w = 3 * x1^2 + a * z1^2
				var w = x1.Square.Multiply( BigInteger.Three );

				if ( a.Sign != 0 ) {
					w = w.Addition( z.Square.Multiply( a ) );
				}

				w = w.Modulo( curve.P );
				// x3 = 2 * y1 * z1 * (w^2 - 8 * x1 * y1^2 * z1)
				var x3 = w.Square.Subtract( x1.ShiftLeft( 3 ).Multiply( y1sqz1 ) ).ShiftLeft( 1 ).Multiply( y1z1 ).Modulo( curve.P );
				// y3 = 4 * y1^2 * z1 * (3 * w * x1 - 2 * y1^2 * z1) - w^3
				var y3 = w.Multiply( BigInteger.Three ).Multiply( x1 ).Subtract( y1sqz1.ShiftLeft( 1 ) ).ShiftLeft( 2 ).Multiply( y1sqz1 ).Subtract( w.Power( 3 ) ).Modulo( curve.P );
				// z3 = 8 * (y1 * z1)^3
				var z3 = y1z1.Power( 3 ).ShiftLeft( 3 ).Modulo( curve.P );

				return new Point( curve, x3, y3, z3 );
			}
		}

		// Simple NAF (Non-Adjacent Form) multiplication algorithm
		// TODO: modularize the multiplication algorithm
		public Point Multiply( BigInteger k ) {
			if ( curve.IsInfinity( this ) ) {
				return this;
			}
			if ( k.Sign == 0 ) {
				return curve.Infinity;
			}
			var e = k;
			var h = e.Multiply( BigInteger.Three );
			var neg = Negate;
			var R = this;

			for ( var i = h.BitLength - 2; i > 0; i-- ) {
				var hBit = h.TestBit( i );
				var eBit = e.TestBit( i );

				R = R.Twice;

				if ( hBit != eBit ) {
					R = R.Addition( hBit ? this : neg );
				}
			}

			return R;
		}

		// Compute this*j + x*k (simultaneous multiplication)
		public Point MultiplyTwo( BigInteger j, Point x, BigInteger k ) {
			var i = Math.Max( j.BitLength, k.BitLength ) - 1;
			var R = curve.Infinity;
			var both = Addition( x );

			while ( i >= 0 ) {
				var jBit = j.TestBit( i );
				var kBit = k.TestBit( i );

				R = R.Twice;

				if ( jBit ) {
					if ( kBit ) {
						R = R.Addition( both );
					} else {
						R = R.Addition( this );
					}
				} else if ( kBit ) {
					R = R.Addition( x );
				}
				i--;
			}

			return R;
		}

		public byte[] GetEncoded( bool compressed = true ) {
			if ( curve.IsInfinity( this ) ) {
				return new byte[] { 0 }; // Infinity point encoded is simply '00'
			}
			var affineX = AffineX;
			var affineY = AffineY;
			var buffer = new byte[ 0 ];
			var byteLength = ( int )Math.Floor( (curve.P.BitLength + 7) / 8.0 );

			if ( compressed ) {		// 0x02/0x03 | X
				buffer = buffer.Add( ( byte )(affineY.IsEven ? 0x02 : 0x03) );
				buffer = buffer.Concat( affineX.ToBuffer( byteLength ) );
			} else {				// 0x04 | X | Y
				buffer = buffer.Add( ( byte )0x04 );
				buffer = buffer.Concat( affineX.ToBuffer( byteLength ) );
				buffer = buffer.Concat( affineY.ToBuffer( byteLength ) );
			}

			return buffer;
		}

		public static Point DecodeFrom( Curve curve, byte[] buffer ) {
			var type = buffer[ 0 ];
			var compressed = (type != 4);

			var byteLength = ( int )Math.Floor( (curve.P.BitLength + 7) / 8.0 );
			var x = BigInteger.FromBuffer( buffer.Slice( 1, 1 + byteLength ) );

			if ( compressed ) {
				Assert.Equal( buffer.Length, byteLength + 1, "Invalid sequence length" );
				Assert.Check( type == 0x02 || type == 0x03, "Invalid sequence tag" );
				var isOdd = (type == 0x03);
				return curve.PointFromX( isOdd, x );
			}
			var y = BigInteger.FromBuffer( buffer.Slice( 1 + byteLength ) );

			return FromAffine( curve, x, y );
		}

		public override string ToString() {
			if ( curve.IsInfinity( this ) ) {
				return "(INFINITY)";
			}
			return "(" + AffineX + "," + AffineY + ")";
		}
	}
}