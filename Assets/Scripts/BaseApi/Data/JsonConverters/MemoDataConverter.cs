using System;
using Newtonsoft.Json.Linq;


namespace Base.Data.Json {

	public sealed class MemoDataConverter : JsonCustomConverter<MemoData, JObject> {

		protected override MemoData Deserialize( JObject value, Type objectType ) {
			return MemoData.Create( value );
		}

		protected override JObject Serialize( MemoData value ) {
			return JObject.Parse( value.ToString() );
		}
	}
}