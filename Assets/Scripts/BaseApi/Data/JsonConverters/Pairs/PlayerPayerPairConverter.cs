using System;
using Base.Data.Pairs;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class PlayerPayerPairConverter : JsonCustomConverter<PlayerPayerPair, JArray> {

		protected override PlayerPayerPair Deserialize( JArray value, Type objectType ) {
			if ( value.IsNullOrEmpty() || value.Count != 2 ) {
				return null;
			}
			return new PlayerPayerPair( value.First.ToObject<SpaceTypeId>(), value.Last.ToObject<SpaceTypeId>() );
		}

		protected override JArray Serialize( PlayerPayerPair value ) {
			if ( value.IsNull() ) {
				return new JArray();
			}
			return new JArray( JToken.FromObject( value.Player ), JToken.FromObject( value.Payer ) );
		}
	}
}