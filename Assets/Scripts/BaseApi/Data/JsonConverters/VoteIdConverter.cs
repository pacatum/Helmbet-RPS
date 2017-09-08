using System;
using Newtonsoft.Json.Linq;


namespace Base.Data.Json {

	public sealed class VoteIdConverter : JsonCustomConverter<VoteId, JToken> {

		protected override VoteId Deserialize( JToken value, Type objectType ) {
			return ConvertFrom( value );
		}

		protected override JToken Serialize( VoteId value ) {
			return ConvertTo( value );
		}

		static JToken ConvertTo( VoteId voteId ) {
			if ( VoteId.EMPTY.Equals( voteId ) ) {
				return JToken.FromObject( 0 );
			}
			return JToken.FromObject( voteId.ToString() );
		}

		static VoteId ConvertFrom( JToken value ) {
			if ( value.Type.Equals( JTokenType.String ) ) {
				return VoteId.Create( value.ToString() );
			}
			return VoteId.EMPTY;
		}
	}
}