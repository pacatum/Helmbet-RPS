using System;
using Base.Data.Pairs;
using Base.ECC;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class PublicKeyValuePairConverter : JsonCustomConverter<PublicKeyValuePair, JArray> {

		protected override PublicKeyValuePair Deserialize( JArray value, Type objectType ) {
			if ( value.IsNullOrEmpty() || value.Count != 2 ) {
				return null;
			}
			return new PublicKeyValuePair( value.First.ToObject<PublicKey>(), Convert.ToUInt16( value.Last ) );
		}

		protected override JArray Serialize( PublicKeyValuePair value ) {
			if ( value.IsNull() ) {
				return new JArray();
			}
			return new JArray( JToken.FromObject( value.PublicKey ), value.Value );
		}
	}
}