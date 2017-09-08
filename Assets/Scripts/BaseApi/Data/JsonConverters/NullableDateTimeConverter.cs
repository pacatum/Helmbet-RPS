using System;
using Tools;


namespace Base.Data.Json {

	public sealed class NullableDateTimeConverter : JsonCustomConverter<DateTime?, string> {

		protected override DateTime? Deserialize( string value, Type objectType ) {
			return value.IsNullOrEmpty() ? null : ( DateTime? )DateTimeConverter.ConvertFrom( value );
		}

		protected override string Serialize( DateTime? value ) {
			return value.HasValue ? DateTimeConverter.ConvertTo( value.Value ) : null;
		}
	}
}