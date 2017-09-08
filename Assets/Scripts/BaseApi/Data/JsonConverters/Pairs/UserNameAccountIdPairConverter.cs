using System;
using Base.Data.Pairs;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class UserNameAccountIdPairConverter : JsonCustomConverter<UserNameAccountIdPair, JArray> {

		protected override UserNameAccountIdPair Deserialize( JArray value, Type objectType ) {
			if ( value.IsNullOrEmpty() || value.Count != 2 ) {
				return null;
			}
			return new UserNameAccountIdPair( value.First.ToNullableString(), value.Last.ToObject<SpaceTypeId>() );
		}

		protected override JArray Serialize( UserNameAccountIdPair value ) {
			if ( value.IsNull() ) {
				return new JArray();
			}
			return new JArray( value.UserName, JToken.FromObject( value.Id ) );
		}
	}
}