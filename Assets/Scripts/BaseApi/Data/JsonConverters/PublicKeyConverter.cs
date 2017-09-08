using System;
using Base.ECC;


namespace Base.Data.Json {

	public sealed class PublicKeyConverter : JsonCustomConverter<PublicKey, string> {

		protected override PublicKey Deserialize( string value, Type objectType ) {
			return PublicKey.FromPublicKeyString( value );
		}

		protected override string Serialize( PublicKey value ) {
			return value.ToString();
		}
	}
}