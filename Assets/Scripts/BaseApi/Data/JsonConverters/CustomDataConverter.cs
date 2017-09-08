using System;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class CustomDataConverter : JsonCustomConverter<CustomData, JObject> {

		protected override CustomData Deserialize( JObject value, Type objectType ) {
			if ( value.IsNull() || value.Type.Equals( JTokenType.Null ) ) {
				return null;
			}
			return new CustomData( value );
		}

		protected override JObject Serialize( CustomData value ) {
			return JObject.Parse( value.ToNullableString() );
		}
	}
}