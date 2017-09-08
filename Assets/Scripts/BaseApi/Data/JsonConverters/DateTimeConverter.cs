using System;
using Newtonsoft.Json;


namespace Base.Data.Json {

	public sealed class DateTimeConverter : JsonCustomConverter<DateTime, string> {

		protected override DateTime Deserialize( string value, Type objectType ) {
			return ConvertFrom( value );
		}

		protected override string Serialize( DateTime value ) {
			return ConvertTo( value );
		}

		public static string ConvertTo( DateTime value ) {
			return SpecifyKindToUtc( value ).ToString( "s" );
		}

		public static DateTime ConvertFrom( string value ) {
			return SpecifyKindToUtc( DateTime.Parse( value ) );
		}

		public static DateTime SpecifyKindToUtc( DateTime date ) {
			switch ( date.Kind ) {
			case DateTimeKind.Unspecified:
				return DateTime.SpecifyKind( date, DateTimeKind.Utc );
			case DateTimeKind.Local:
				return date.ToUniversalTime();
			default:
				return date;
			}
		}

		public JsonSerializer GetSerializer() {
			var serializerSettings = new JsonSerializerSettings {
				Converters = new JsonConverter[] { this }
			};
			serializerSettings.TypeNameHandling = TypeNameHandling.Objects;
			return JsonSerializer.Create( serializerSettings );
		}
	}
}