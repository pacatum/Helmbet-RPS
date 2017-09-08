using BigI;
using Tools;


namespace ECurve {

	public class Curve {

		readonly static Curve secP256k1 = SecP256k1Instance();

		static Curve SecP256k1Instance() {
			return new Curve(
				new BigInteger( "fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f", 16 ),
				new BigInteger( "00", 16 ),
	  			new BigInteger( "07", 16 ),
				new BigInteger( "79be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798", 16 ),
				new BigInteger( "483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8", 16 ),
				new BigInteger( "fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141", 16 ),
				new BigInteger( "01", 16 )
			);
		}

		public static Curve SecP256k1 {
			get { return secP256k1; }
		}


		readonly BigInteger p;
		readonly BigInteger a;
		readonly BigInteger b;
		readonly Point g;
		readonly BigInteger n;
		readonly BigInteger h;
		readonly Point infinity;
		readonly BigInteger pOverFour;


		public BigInteger P {
			get { return p; }
		}

		public Point G {
			get { return g; }
		}

		public BigInteger A {
			get { return a; }
		}

		public BigInteger B {
			get { return b; }
		}

		public BigInteger N {
			get { return n; }
		}

		public Point Infinity {
			get { return infinity; }
		}

		public Curve( BigInteger p, BigInteger a, BigInteger b, BigInteger gX, BigInteger gY, BigInteger n, BigInteger h ) {
			this.p = p;
			this.a = a;
			this.b = b;
			this.n = n;
			this.h = h;

			g = Point.FromAffine( this, gX, gY );
			infinity = new Point( this, null, null, BigInteger.Zero );

			// result caching
			pOverFour = p.Addition( BigInteger.One ).ShiftRight( 2 );
		}

		public Point PointFromX( bool isOdd, BigInteger x ) {
			var alpha = x.Power( 3 ).Addition( a.Multiply( x ) ).Addition( b ).Modulo( p );
			var beta = alpha.ModuloPower( pOverFour, p ); // XXX: not compatible with all curves

			var y = beta;
			if ( beta.IsEven ^ !isOdd ) {
				y = p.Subtract( y ); // -y % p
			}

			return Point.FromAffine( this, x, y );
		}

		public bool IsInfinity( Point q ) {
			if ( q == infinity ) {
				return true;
			}
			return q.Z.Sign == 0 && q.Y.Sign != 0;
		}

		bool IsOnCurve( Point q ) {
			if ( IsInfinity( q ) ) {
				return true;
			}

			var x = q.AffineX;
			var y = q.AffineY;
			var a = this.a;
			var b = this.b;
			var p = this.p;

			// Check that xQ and yQ are integers in the interval [0, p - 1]
			if ( x.Sign < 0 || x.CompareTo( p ) >= 0 ) {
				return false;
			}
			if ( y.Sign < 0 || y.CompareTo( p ) >= 0 ) {
				return false;
			}

			// and check that y^2 = x^3 + ax + b (mod p)
			var lhs = y.Square.Modulo( p );
			var rhs = x.Power( 3 ).Addition( a.Multiply( x ) ).Addition( b ).Modulo( p );
			return lhs.Equals( rhs );
		}

 		// Validate an elliptic curve point.
 		// See SEC 1, section 3.2.2.1: Elliptic Curve Public Key Validation Primitive
		public bool Validate( Point q ) {
			// Check Q != O
			Assert.Check( !IsInfinity( q ), "Point is at infinity" );
			Assert.Check( IsOnCurve( q ), "Point is not on the curve" );

			// Check nQ = O (where Q is a scalar multiple of G)
			var nQ = q.Multiply( n );
			Assert.Check( IsInfinity( nQ ), "Point is not a scalar multiple of G" );

			return true;
		}
	}
}