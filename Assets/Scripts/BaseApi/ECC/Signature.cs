using System;
using System.Security.Cryptography;
using System.Text;
using BigI;
using ECurve;
using Tools;


namespace Base.ECC {

	public sealed class Signature {

		readonly byte i;
		readonly BigInteger r;
		readonly BigInteger s;


		Signature( BigInteger r, BigInteger s, byte i ) {
			this.r = r;
			this.s = s;
			this.i = i;
		}

		public static Signature FromHex( string hex ) {
			return FromBuffer( Tool.FromHex( hex ) );
		}

		public static Signature FromBuffer( byte[] buffer ) {
			Assert.Equal( buffer.Length, 65, "Invalid signature length" );
			var i = buffer[ 0 ];
			Assert.Equal( i - 27, (i - 27) & 7, "Invalid signature parameter" );
			var r = BigInteger.FromBuffer( buffer.Slice( 1, 33 ) );
			var s = BigInteger.FromBuffer( buffer.Slice( 33 ) );
			return new Signature( r, s, i );
		}

		public string ToHex() {
			return Tool.ToHex( ToBuffer() );
		}

		public byte[] ToBuffer() {
			var buffer = new byte[ 65 ];
			buffer[ 0 ] = i;
			Array.Copy( r.ToBuffer( 32 ), 0, buffer, 1, 32 );
			Array.Copy( s.ToBuffer( 32 ), 0, buffer, 33, 32 );
			return buffer;
		}

		public PublicKey RecoverPublicKeyFromBuffer( byte[] buffer ) {
			var hash = SHA256.Create().HashAndDispose( buffer );
			return RecoverPublicKey( hash );
		}

		PublicKey RecoverPublicKey( byte[] bufferSha256 ) {
			var e = BigInteger.FromBuffer( bufferSha256 );
			var q = ECDSA.RecoverPublicKey( Curve.SecP256k1, e, new ECSignature( r, s ), ( byte )((i - 27) & 3) );
			return PublicKey.FromPoint( q );
		}

		public static Signature Sign( string str, PrivateKey privateKey ) {
			return SignBuffer( Encoding.UTF8.GetBytes( str ), privateKey );
		}

		public static Signature SignHex( string hex, PrivateKey privateKey ) {
			return SignBuffer( Tool.FromHex( hex ), privateKey );
		}

		public static Signature SignBuffer( byte[] buffer, PrivateKey privateKey ) {
			var hash = SHA256.Create().HashAndDispose( buffer );
			return SignBufferSha256( hash, privateKey );
		}

		static Signature SignBufferSha256( byte[] bufferSha256, PrivateKey privateKey ) {
			if ( bufferSha256.Length != 32 ) {
				throw new ArgumentException( "bufferSha256: 32 byte buffer requred" );
			}
			var nonce = uint.MinValue;
			var e = BigInteger.FromBuffer( bufferSha256 );
			ECSignature ecSignature = null;
			var i = byte.MinValue;
			while ( true ) {
				ecSignature = ECDSA.Sign( Curve.SecP256k1, bufferSha256, privateKey.D, nonce++ );
				var der = ecSignature.ToDER();
				var lengthR = der[ 3 ];
				var lengthS = der[ 5 + lengthR ];
				if ( lengthR == 32 && lengthS == 32 ) {
					i = ECDSA.CalculatePublicKeyRecoveryParameter( Curve.SecP256k1, e, ecSignature, privateKey.ToPublicKey().Q );
					i += 4;  // compressed
					i += 27; // compact  //  24 or 27 :( forcing odd-y 2nd key candidate)
					break;
				}
				if ( nonce % 10 == 0 ) {
					Unity.Console.Warning( nonce, "attempts to find canonical signature" );
				}
			}
			return new Signature( ecSignature.R, ecSignature.S, i );
		}

		public bool VerifyHex( string hex, PublicKey publicKey ) {
			return VerifyBuffer( Tool.FromHex( hex ), publicKey );
		}

		public bool VerifyBuffer( byte[] buffer, PublicKey publicKey ) {
			var hash = SHA256.Create().HashAndDispose( buffer );
			return VerifyHash( hash, publicKey );
		}

		bool VerifyHash( byte[] hash, PublicKey publicKey ) {
			if ( hash.Length != 32 ) {
				Unity.Console.DebugWarning( "A sha256 hash should be 32 bytes long, instead got ", hash.Length );
			}
			return ECDSA.Verify( Curve.SecP256k1, hash, new ECSignature( r, s ), publicKey.Q );
		}
	}
}