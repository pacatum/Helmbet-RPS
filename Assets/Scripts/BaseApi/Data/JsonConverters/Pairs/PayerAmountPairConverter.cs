using System;
using Base.Data.Pairs;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class PayerAmountPairConverter : JsonCustomConverter<PayerAmountPair, JArray> {

		protected override PayerAmountPair Deserialize( JArray value, Type objectType ) {
			if ( value.IsNullOrEmpty() || value.Count != 2 ) {
				return null;
			}
			return new PayerAmountPair( value.First.ToObject<SpaceTypeId>(), Convert.ToInt64( value.Last ) );
		}

		protected override JArray Serialize( PayerAmountPair value ) {
			if ( value.IsNull() ) {
				return new JArray();
			}
			return new JArray( JToken.FromObject( value.Payer ), value.Amount );
		}
	}
}