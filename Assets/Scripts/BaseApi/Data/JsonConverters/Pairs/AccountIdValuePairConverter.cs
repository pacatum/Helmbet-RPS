using System;
using Base.Data.Pairs;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class AccountIdValuePairConverter : JsonCustomConverter<AccountIdValuePair, JArray> {

		protected override AccountIdValuePair Deserialize( JArray value, Type objectType ) {
			if ( value.IsNullOrEmpty() || value.Count != 2 ) {
				return null;
			}
			return new AccountIdValuePair( value.First.ToObject<SpaceTypeId>(), Convert.ToUInt16( value.Last ) );
		}

		protected override JArray Serialize( AccountIdValuePair value ) {
			if ( value.IsNull() ) {
				return new JArray();
			}
			return new JArray( JToken.FromObject( value.Account ), value.Value );
		}
	}
}