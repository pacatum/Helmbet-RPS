using System;
using System.Security.Cryptography;
using System.Text;
using BigI;
using ECurve;
using SimpleBase;
using Tools;


namespace Base.ECC {

	public sealed class PrivateKey {

		readonly BigInteger d = null;

		PublicKey publicKey = null;


		public BigInteger D {
			get { return d; }
		}

		PrivateKey() { }

		PrivateKey( BigInteger d ) {
			this.d = d;
		}

		public static PrivateKey FromBuffer( byte[] buffer ) {
			if ( buffer.OrEmpty().Length == 0 ) {
				throw new ArgumentException( "Empty buffer" );
			}
			if ( buffer.Length != 32 ) {
				throw new ArgumentException( "Expecting 32 bytes, instead got " + buffer.Length );
			}
			return new PrivateKey( BigInteger.FromBuffer( buffer ) );
		}

		public static PrivateKey FromSeed( string seed ) {
			var hash = SHA256.Create().HashAndDispose( Encoding.UTF8.GetBytes( seed ) );
			return FromBuffer( hash );
		}

		public static PrivateKey FromWif( string wif ) {
			var privateWif = Base58.Decode( wif );
			var version = privateWif[ 0 ];
			Assert.Equal( 0x80, version, string.Format( "Expected version {0}, instead got {1}", 0x80, version ) );
			// checksum includes the version
			var privateKey = privateWif.Slice( 0, privateWif.Length - 4 );
			var checksum = privateWif.Slice( privateWif.Length - 4 );
			var newChecksum = SHA256.Create().HashAndDispose( privateKey );
			newChecksum = SHA256.Create().HashAndDispose( newChecksum );
			newChecksum = newChecksum.Slice( 0, 4 );

			var isEqual = checksum.DeepEqual( newChecksum ); // invalid checksum
			if ( !isEqual ) {
				throw new InvalidOperationException( "Checksum did not match" );
			}
			privateKey = privateKey.Slice( 1 );
			privateKey = privateKey.Slice( 0, 32 );
			return FromBuffer( privateKey );
		}

		public string ToWif() {
			var buffer = new byte[ 0 ];
			// checksum includes the version
			buffer = buffer.Add( ( byte )0x80 );
			buffer = buffer.Concat( ToBuffer() );
			var checksum = SHA256.Create().HashAndDispose( buffer );
			checksum = SHA256.Create().HashAndDispose( checksum );
			checksum = checksum.Slice( 0, 4 );
			buffer = buffer.Concat( checksum );
			return Base58.Encode( buffer );
		}

		Point ToPublicKeyPoint() {
			return Curve.SecP256k1.G.Multiply( d );
		}

		public PublicKey ToPublicKey() {
			if ( !publicKey.IsNull() ) {
				return publicKey;
			}
			return publicKey = PublicKey.FromPoint( ToPublicKeyPoint() );
		}

		public byte[] ToBuffer() {
			return d.ToBuffer( 32 );
		}

		/** ECIES */
		byte[] GetSharedSecret( PublicKey key, bool legacy = false ) {
			var kb = key.ToUncompressed().ToBuffer().ToArray();
			var kbP = Point.FromAffine( Curve.SecP256k1, BigInteger.FromBuffer( kb.Slice( 1, 33 ) ), BigInteger.FromBuffer( kb.Slice( 33, 65 ) ) );
			var r = ToBuffer();
			var p = kbP.Multiply( BigInteger.FromBuffer( r ) );
			var s = p.AffineX.ToBuffer( 32 );
			if ( s.Length > 32 ) {
				Array.Resize( ref s, 32 );
			}
			// The input to sha512 must be exactly 32-bytes, to match the c++ implementation
			// of GetSharedSecret. Right now S will be shorter if the most significant
			// byte(s) is zero. Pad it backfull 32-bytes
			if ( !legacy && s.Length < 32 ) {
				var padding = new byte[ 32 - s.Length ].Fill( ( byte )0 );
				s = padding.Concat( s );
			}
			// SHA512 used in ECIES
			return SHA512.Create().HashAndDispose( s );
		}

		PrivateKey Child( byte[] offset ) {
			offset = ToPublicKey().ToBuffer().ToArray().Concat( offset );
			offset = SHA256.Create().HashAndDispose( offset );
			var c = BigInteger.FromBuffer( offset );
			if ( c.CompareTo( Curve.SecP256k1.N ) >= 0 ) {
				throw new InvalidOperationException( "Child offset went out of bounds, try again" );
			}
			var derived = d.Addition( c );//.Mod( Curve.SecP256k1.N );
			if ( derived.Sign == 0 ) {
				throw new InvalidOperationException( "Child offset derived to an invalid key, try again" );
			}
			return new PrivateKey( derived );
		}

		public string ToHex() {
			return Tool.ToHex( ToBuffer() );
		}

		public override string ToString() {
			return ToHex();
		}
	}
}