using System;
using Base.Data.Accounts;
using Base.Data.Pairs;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class UserNameFullAccountDataPairConverter : JsonCustomConverter<UserNameFullAccountDataPair, JArray> {

		protected override UserNameFullAccountDataPair Deserialize( JArray value, Type objectType ) {
			if ( value.IsNullOrEmpty() || value.Count != 2 ) {
				return null;
			}
			return new UserNameFullAccountDataPair( value.First.ToNullableString(), value.Last.ToObject<FullAccountData>() );
		}

		protected override JArray Serialize( UserNameFullAccountDataPair value ) {
			if ( value.IsNull() ) {
				return new JArray();
			}
			return new JArray( value.UserName, JToken.FromObject( value.FullAccount ) );
		}
	}
}