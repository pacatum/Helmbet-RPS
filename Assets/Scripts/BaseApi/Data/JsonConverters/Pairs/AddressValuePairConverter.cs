using System;
using Base.Data.Pairs;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class AddressValuePairConverter : JsonCustomConverter<AddressValuePair, JArray> {

		protected override AddressValuePair Deserialize( JArray value, Type objectType ) {
			if ( value.IsNullOrEmpty() || value.Count != 2 ) {
				return null;
			}
			return new AddressValuePair( value.First.ToNullableString(), Convert.ToUInt16( value.Last ) );
		}

		protected override JArray Serialize( AddressValuePair value ) {
			if ( value.IsNull() ) {
				return new JArray();
			}
			return new JArray( JToken.FromObject( value.Address ), value.Value );
		}
	}
}