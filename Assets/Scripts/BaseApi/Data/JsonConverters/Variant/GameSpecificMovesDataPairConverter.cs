using System;
using Base.Config;
using Base.Data.Tournaments.GameMoves;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class GameSpecificMovesDataPairConverter : JsonCustomConverter<GameSpecificMovesData, JContainer> {

		protected override GameSpecificMovesData Deserialize( JContainer value, Type objectType ) {
			if ( value.IsNullOrEmpty() ) {
				return null;
			}
			if ( value.Type.Equals( JTokenType.Array ) ) {
				if ( value.Count != 2 ) {
					return null;
				}
				var type = ( ChainTypes.GameSpecificMoves )Convert.ToInt32( value.First );
				switch ( type ) {
				case ChainTypes.GameSpecificMoves.RockPaperScissorsThrowCommit:
					objectType = typeof( RockPaperScissorsThrowCommitData );
					break;
				case ChainTypes.GameSpecificMoves.RockPaperScissorsThrowReveal:
					objectType = typeof( RockPaperScissorsThrowRevealData );
					break;
				default:
					Unity.Console.Error( "Unexpected game specific moves type:", type );
					return null;
				}
				value = value.Last as JObject;
			}
			if ( objectType.Equals( typeof( RockPaperScissorsThrowCommitData ) ) ) {
				return RockPaperScissorsThrowCommitData.Create( value as JObject );
			}
			if ( objectType.Equals( typeof( RockPaperScissorsThrowRevealData ) ) ) {
				return RockPaperScissorsThrowRevealData.Create( value as JObject );
			}
			Unity.Console.Error( "Unexpected game specific moves type:", objectType );
			return null;
		}

		protected override JContainer Serialize( GameSpecificMovesData value ) {
			if ( value == null ) {
				return new JArray();
			}
			return new JArray( ( int )value.Type, JObject.Parse( value.ToString() ) );
		}
	}
}