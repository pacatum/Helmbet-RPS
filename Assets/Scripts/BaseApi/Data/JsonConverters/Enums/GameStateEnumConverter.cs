using System;
using Base.Config;


namespace Base.Data.Json {

	public sealed class GameStateEnumConverter : JsonCustomConverter<ChainTypes.GameState, string> {

		const string EXPECTING_COMMIT_MOVES = "expecting_commit_moves";
		const string EXPECTING_REVEAL_MOVES = "expecting_reveal_moves";
		const string GAME_COMPLETE = "game_complete";


		protected override ChainTypes.GameState Deserialize( string value, Type objectType ) {
			return ConvertFrom( value );
		}

		protected override string Serialize( ChainTypes.GameState value ) {
			return ConvertTo( value );
		}

		static string ConvertTo( ChainTypes.GameState state ) {
			switch ( state ) {
			case ChainTypes.GameState.ExpectingCommitMoves:
				return EXPECTING_COMMIT_MOVES;
			case ChainTypes.GameState.ExpectingRevealMoves:
				return EXPECTING_REVEAL_MOVES;
			case ChainTypes.GameState.Complete:
				return GAME_COMPLETE;
			}
			throw new ArgumentException( "Unexpected value: " + state );
		}

		static ChainTypes.GameState ConvertFrom( string state ) {
			switch ( state ) {
			case EXPECTING_COMMIT_MOVES:
				return ChainTypes.GameState.ExpectingCommitMoves;
			case EXPECTING_REVEAL_MOVES:
				return ChainTypes.GameState.ExpectingRevealMoves;
			case GAME_COMPLETE:
				return ChainTypes.GameState.Complete;
			}
			throw new ArgumentException( "Unexpected value: " + state );
		}
	}
}