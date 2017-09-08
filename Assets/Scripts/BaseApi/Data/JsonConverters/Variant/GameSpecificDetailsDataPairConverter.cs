using System;
using Base.Config;
using Base.Data.Tournaments.GameDetails;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class GameSpecificDetailsDataPairConverter : JsonCustomConverter<GameSpecificDetailsData, JArray> {

		protected override GameSpecificDetailsData Deserialize( JArray value, Type objectType ) {
			if ( value.IsNullOrEmpty() || value.Count != 2 ) {
				return null;
			}
			var type = ( ChainTypes.GameSpecificDetails )Convert.ToInt32( value.First );
			switch ( type ) {
			case ChainTypes.GameSpecificDetails.RockPaperScissorsGameDetails:
				return RockPaperScissorsGameDetailsData.Create( value.Last as JObject );
			default:
				Unity.Console.Error( "Unexpected game specific details type:", type );
				return null;
			}
		}

		protected override JArray Serialize( GameSpecificDetailsData value ) {
			if ( value == null ) {
				return new JArray();
			}
			return new JArray( ( int )value.Type, JObject.Parse( value.ToString() ) );
		}
	}
}