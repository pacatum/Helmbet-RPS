using System;
using System.Security.Cryptography;
using BigI;
using ECurve;
using Tools;


namespace Base.ECC {

	public static class ECDSA {

		delegate bool SignDelegate( BigInteger t );


		public static Point RecoverPublicKey( Curve curve, BigInteger e, ECSignature signature, byte i ) {
			Assert.Equal( i & 3, i, "Recovery param is more than two bits" );

			var n = curve.N;
			var g = curve.G;
			var r = signature.R;
			var s = signature.S;

			Assert.Check( r.Sign > 0 && r.CompareTo( n ) < 0, "Invalid r value" );
			Assert.Check( s.Sign > 0 && s.CompareTo( n ) < 0, "Invalid s value" );

			// A set LSB signifies that the y-coordinate is odd
			var isYOdd = (i & 1) != 0;

			// The more significant bit specifies whether we should use the
			// first or second candidate key.
			var isSecondKey = (i >> 1) != 0;

			// 1.1 Let x = r + jn
			var x = isSecondKey ? r.Addition( n ) : r;
			var R = curve.PointFromX( isYOdd, x );

			// 1.4 Check that nR is at infinity
			var nR = R.Multiply( n );
			Assert.Check( curve.IsInfinity( nR ), "nR is not a valid curve point" );

			// Compute -e from e
			var eNegate = e.Negate.Modulo( n );

			// 1.6.1 Compute q = r^-1 (sR -  eG)
			//               q = r^-1 (sR + -eG)
			var rInverse = r.ModuloInverse( n );

			var q = R.MultiplyTwo( s, g, eNegate ).Multiply( rInverse );

			curve.Validate( q );

			return q;
		}

		public static ECSignature Sign( Curve curve, byte[] hash, BigInteger d, uint nonce ) {
			var e = BigInteger.FromBuffer( hash );
			var n = curve.N;
			var g = curve.G;
			var r = BigInteger.Zero;
			var s = BigInteger.Zero;
			DeterministicGenerateK( curve, hash, d, k => {
				// find canonically valid signature
				var q = g.Multiply( k );

				if ( curve.IsInfinity( q ) ) {
					return false;
				}

				r = q.AffineX.Modulo( n );
				if ( r.Sign == 0 ) {
					return false;
				}

				s = k.ModuloInverse( n ).Multiply( e.Addition( d.Multiply( r ) ) ).Modulo( n );
				if ( s.Sign == 0 ) {
					return false;
				}

				return true;
			}, nonce );

			var nOverTwo = n.ShiftRight( 1 );

			// enforce low S values, see bip62: 'low s values in signatures'
			if ( s.CompareTo( nOverTwo ) > 0 ) {
				s = n.Subtract( s );
			}

			return new ECSignature( r, s );
		}

		public static byte CalculatePublicKeyRecoveryParameter( Curve curve, BigInteger e, ECSignature signature, Point q ) {
			for ( var i = 0; i < 4; i++ ) {
				var qPrime = RecoverPublicKey( curve, e, signature, ( byte )i );

				// 1.6.2 Verify q
				if ( qPrime.Equals( q ) ) {
					return ( byte )i;
				}
			}
			throw new InvalidOperationException( "Unable to find valid recovery factor" );
		}

		public static bool Verify( Curve curve, byte[] hash, ECSignature signature, Point q ) {
			// 1.4.2 H = Hash(M), already done by the user
			// 1.4.3 e = H
			var e = BigInteger.FromBuffer( hash );
			return VerifyRaw( curve, e, signature, q );
		}

		static bool VerifyRaw( Curve curve, BigInteger e, ECSignature signature, Point q ) {
			var n = curve.N;
			var g = curve.G;
			var r = signature.R;
			var s = signature.S;

			// 1.4.1 Enforce r and s are both integers in the interval [1, n − 1]
			if ( r.Sign <= 0 || r.CompareTo( n ) >= 0 ) {
				return false;
			}
			if ( s.Sign <= 0 || s.CompareTo( n ) >= 0 ) {
				return false;
			}

			// c = s^-1 mod n
			var c = s.ModuloInverse( n );

			// 1.4.4 Compute u1 = es^−1 mod n
			//               u2 = rs^−1 mod n
			var u1 = e.Multiply( c ).Modulo( n );
			var u2 = r.Multiply( c ).Modulo( n );

			// 1.4.5 Compute R = (xR, yR) = u1G + u2Q
			var R = g.MultiplyTwo( u1, q, u2 );

			// 1.4.5 (cont.) Enforce R is not at infinity
			if ( curve.IsInfinity( R ) ) {
				return false;
			}

			// 1.4.6 Convert the field element R.x to an integer
			var xR = R.AffineX;

			// 1.4.7 Set v = xR mod n
			var v = xR.Modulo( n );

			// 1.4.8 If v = r, output "valid", and if v != r, output "invalid"
			return v.Equals( r );
		}

		// concat onli return new value TODO check
		static BigInteger DeterministicGenerateK( Curve curve, byte[] hash, BigInteger d, SignDelegate checkSign, uint nonce ) {
			if ( nonce > 0 ) {
				hash = SHA256.Create().HashAndDispose( hash.Concat( new byte[ nonce ] ) );
			}
			Assert.Equal( hash.Length, 32, "Hash must be 256 bit" );

			var x = d.ToBuffer( 32 );
			var key = new byte[ 32 ].Fill( ( byte )0 );
			var value = new byte[ 32 ].Fill( ( byte )1 );

			// Step D
			key = new HMACSHA256( key ).HashAndDispose( value.Add( ( byte )0 ).Concat( x, hash ) );

			// Step E
			value = new HMACSHA256( key ).HashAndDispose( value );

			// Step F
			key = new HMACSHA256( key ).HashAndDispose( value.Add( ( byte )1 ).Concat( x, hash ) );

			// Step G
			value = new HMACSHA256( key ).HashAndDispose( value );

			// Step H1/H2a, ignored as tlen == qlen (256 bit)
			// Step H2b
			value = new HMACSHA256( key ).HashAndDispose( value );

			var t = BigInteger.FromBuffer( value );

			// Step H3, repeat until t is within the interval [1, n - 1]
			while ( (t.Sign <= 0) || (t.CompareTo( curve.N ) >= 0) || !checkSign( t ) ) {
				key = new HMACSHA256( key ).HashAndDispose( value.Add( ( byte )0 ) );
				value = new HMACSHA256( key ).HashAndDispose( value );

				// Step H1/H2a, again, ignored as tlen == qlen (256 bit)
				// Step H2b again
				value = new HMACSHA256( key ).HashAndDispose( value );

				t = BigInteger.FromBuffer( value );
			}

			return t;
		}
	}
}