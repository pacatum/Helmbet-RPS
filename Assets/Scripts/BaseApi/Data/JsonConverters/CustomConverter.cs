using System;
using Newtonsoft.Json;


namespace Base.Data.Json {

	public abstract class JsonCustomConverter<OUT, IN> : JsonConverter {

		protected abstract OUT Deserialize( IN value, Type objectType );
		protected abstract IN Serialize( OUT value );

		// CanConvert use for (de)serialization
		// JsonConvert.SerializeObject( T object ) ==> call check CanConvert( typeof( T ) )...
		// JsonConvert.DeserializeObject<T>( string json ) ==> call check CanConvert( typeof( T ) )...
		public override bool CanConvert( Type objectType ) {
			return typeof( OUT ).IsAssignableFrom( objectType );
		}

		public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer ) {
			return Deserialize( serializer.Deserialize<IN>( reader ), objectType );
		}

		public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer ) {
			serializer.Serialize( writer, Serialize( ( OUT )value ) );
		}
	}
}