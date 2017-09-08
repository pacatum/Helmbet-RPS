using System;
using Newtonsoft.Json.Linq;


namespace Base.Data.Json {

	public sealed class SpaceTypeIdConverter : JsonCustomConverter<SpaceTypeId, JToken> {

		protected override SpaceTypeId Deserialize( JToken value, Type objectType ) {
			return ConvertFrom( value );
		}

		protected override JToken Serialize( SpaceTypeId value ) {
			return ConvertTo( value );
		}

		static JToken ConvertTo( SpaceTypeId spaceTypeId ) {
			if ( SpaceTypeId.EMPTY.Equals( spaceTypeId ) ) {
				return JToken.FromObject( 0 );
			}
			return JToken.FromObject( spaceTypeId.ToString() );
		}

		static SpaceTypeId ConvertFrom( JToken value ) {
			if ( value.Type.Equals( JTokenType.String ) ) {
				return SpaceTypeId.Create( value.ToString() );
			}
			return SpaceTypeId.EMPTY;
		}
	}
}