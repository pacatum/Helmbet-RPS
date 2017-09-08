using System;
using Base.Config;


namespace Base.Data.Json {

	public sealed class MatchStateEnumConverter : JsonCustomConverter<ChainTypes.MatchState, string> {

		const string WAITING_ON_PREVIOUS_MATCHES = "waiting_on_previous_matches";
		const string MATCH_IN_PROGRESS = "match_in_progress";
		const string MATCH_COMPLETE = "match_complete";


		protected override ChainTypes.MatchState Deserialize( string value, Type objectType ) {
			return ConvertFrom( value );
		}

		protected override string Serialize( ChainTypes.MatchState value ) {
			return ConvertTo( value );
		}

		static string ConvertTo( ChainTypes.MatchState state ) {
			switch ( state ) {
			case ChainTypes.MatchState.WaitingOnPreviousMatches:
				return WAITING_ON_PREVIOUS_MATCHES;
			case ChainTypes.MatchState.InProgress:
				return MATCH_IN_PROGRESS;
			case ChainTypes.MatchState.Complete:
				return MATCH_COMPLETE;
			}
			throw new ArgumentException( "Unexpected value: " + state );
		}

		static ChainTypes.MatchState ConvertFrom( string state ) {
			switch ( state ) {
			case WAITING_ON_PREVIOUS_MATCHES:
				return ChainTypes.MatchState.WaitingOnPreviousMatches;
			case MATCH_IN_PROGRESS:
				return ChainTypes.MatchState.InProgress;
			case MATCH_COMPLETE:
				return ChainTypes.MatchState.Complete;
			}
			throw new ArgumentException( "Unexpected value: " + state );
		}
	}
}