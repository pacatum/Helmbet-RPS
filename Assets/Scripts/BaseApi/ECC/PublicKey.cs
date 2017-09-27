using System;
using System.Security.Cryptography;
using BigI;
using Buffers;
using ECurve;
using Base.Config;
using Base.Data;
using Base.Data.Json;
using Newtonsoft.Json;
using SimpleBase;
using Tools;


namespace Base.ECC {

	[JsonConverter( typeof( PublicKeyConverter ) )]
	public sealed class PublicKey : ISerializeToBuffer, IEquatable<PublicKey>, IComparable<PublicKey> {

		readonly Point q = null;


		public Point Q {
			get { return q; }
		}

		PublicKey() { }

		PublicKey( Point q ) {
			this.q = q;
		}

		public static PublicKey FromBuffer( byte[] buffer ) {
			if ( Tool.ToHex( buffer ).Equals( new string( '0', 33 * 2 ) ) ) {
				return new PublicKey( null );
			}
			return new PublicKey( Point.DecodeFrom( Curve.SecP256k1, buffer ) );
		}

		byte[] ToArray( bool compressed = true ) {
			if ( q.IsNull() ) {
				return new byte[ 33 ].Fill( ( byte )0 );
			}
			return q.GetEncoded( compressed );
		}

		public static PublicKey FromPoint( Point point ) {
			return new PublicKey( point );
		}

		public PublicKey ToUncompressed() {
			var buffer = q.GetEncoded( false );
			var point = Point.DecodeFrom( Curve.SecP256k1, buffer );
			return FromPoint( point );
		}

		public byte[] ToBlockchainAddress() {
			var hash = SHA512.Create().HashAndDispose( ToArray() );
			return RIPEMD160.Create().HashAndDispose( hash );
		}

		public override string ToString() {
			return ToPublicKeyString();
		}

		public string ToPublicKeyString( string addressPrefix = null ) {
			if ( addressPrefix.IsNull() ) {
				addressPrefix = ChainConfig.AddressPrefix;
			}
			var buffer = ToArray();
			var checksum = RIPEMD160.Create().HashAndDispose( buffer );
			buffer = buffer.Concat( checksum.Slice( 0, 4 ) );
			return addressPrefix + Base58.Encode( buffer );
		}

		public static PublicKey FromPublicKeyString( string publicKey, string addressPrefix = null ) {
			try {
				return FromStringOrThrow( publicKey, addressPrefix );
			} catch {
				return null;
			}
		}

		static PublicKey FromStringOrThrow( string publicKey, string addressPrefix = null ) {
			if ( addressPrefix.IsNull() ) {
				addressPrefix = ChainConfig.AddressPrefix;
			}
			var prefix = publicKey.Substring( 0, addressPrefix.Length );

			Assert.Equal(
				addressPrefix, prefix,
				string.Format( "Expecting key to begin with {0}, instead got {1}", addressPrefix, prefix )
			);

			publicKey = publicKey.Substring( addressPrefix.Length );
			var buffer = Base58.Decode( publicKey );

			var checksum = buffer.Slice( buffer.Length - 4 );
			buffer = buffer.Slice( 0, buffer.Length - 4 );

			var newChecksum = RIPEMD160.Create().HashAndDispose( buffer );
			newChecksum = newChecksum.Slice( 0, 4 );

			var isEqual = checksum.DeepEqual( newChecksum ); // invalid checksum
			if ( !isEqual ) {
				throw new InvalidOperationException( "Checksum did not match" );
			}
			return FromBuffer( buffer );
		}

		public string ToAddressString( string addressPrefix = null ) {
			if ( addressPrefix.IsNull() ) {
				addressPrefix = ChainConfig.AddressPrefix;
			}
			var buffer = ToArray();
			var hash = SHA512.Create().HashAndDispose( buffer );
			hash = RIPEMD160.Create().HashAndDispose( hash );
			var checksum = RIPEMD160.Create().HashAndDispose( hash );
			hash = hash.Concat( checksum.Slice( 0, 4 ) );
			return addressPrefix + Base58.Encode( hash );
		}

		public string ToPtsAddy() {
			var buffer = ToArray();
			var hash = SHA256.Create().HashAndDispose( buffer );
			hash = RIPEMD160.Create().HashAndDispose( hash );
			hash = new byte[] { 0x38 }.Concat( hash ); // version 56(decimal)
			var checksum = SHA256.Create().HashAndDispose( hash );
			checksum = SHA256.Create().HashAndDispose( checksum );
			hash = hash.Concat( checksum.Slice( 0, 4 ) );
			return Base58.Encode( hash );
		}

		PublicKey Child( byte[] offset ) {
			Assert.Equal( offset.Length, 32, "offset length" );
			offset = ToArray().Concat( offset );
			offset = SHA256.Create().HashAndDispose( offset );
			var c = BigInteger.FromBuffer( offset );
			if ( c.CompareTo( Curve.SecP256k1.N ) >= 0 ) {
				throw new InvalidOperationException( "Child offset went out of bounds, try again" );
			}
			var cG = Curve.SecP256k1.G.Multiply( c );
			var qPrime = q.Addition( cG );
			if ( Curve.SecP256k1.IsInfinity( qPrime ) ) {
				throw new InvalidOperationException( "Child offset derived to an invalid key, try again" );
			}
			return FromPoint( qPrime );
		}

		public static PublicKey FromHex( string hex ) {
			return FromBuffer( Tool.FromHex( hex ) );
		}

		public string ToHex() {
			return Tool.ToHex( ToArray() );
		}

		public override int GetHashCode() {
			return ToString().GetHashCode();
		}

		public override bool Equals( object obj ) {
			if ( this == obj ) {
				return true;
			}
			if ( !(obj is PublicKey) ) {
				return false;
			}
			return Equals( ( PublicKey )obj );
		}

		public bool Equals( PublicKey other ) {
			return ToString().Equals( other.ToNullableString() );
		}

		public int CompareTo( PublicKey other ) {
			return string.Compare( ToAddressString(), other.ToAddressString(), StringComparison.Ordinal );
		}

		public static int Compare( PublicKey a, PublicKey b ) {
			return a.CompareTo( b );
		}

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			return (buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING )).WriteBytes( ToArray(), false );
		}
	}
}