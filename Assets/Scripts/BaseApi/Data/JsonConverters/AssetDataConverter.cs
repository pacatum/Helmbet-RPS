using System;
using Newtonsoft.Json.Linq;


namespace Base.Data.Json {

	public sealed class AssetDataConverter : JsonCustomConverter<AssetData, JObject> {

		protected override AssetData Deserialize( JObject value, Type objectType ) {
			return new AssetData( value );
		}

		protected override JObject Serialize( AssetData value ) {
			return JObject.Parse( value.ToString() );
		}
	}
}