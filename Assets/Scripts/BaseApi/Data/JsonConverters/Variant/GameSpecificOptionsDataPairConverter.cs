using System;
using Base.Config;
using Base.Data.Tournaments.GameOptions;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class GameSpecificOptionsDataPairConverter : JsonCustomConverter<GameSpecificOptionsData, JArray> {

		protected override GameSpecificOptionsData Deserialize( JArray value, Type objectType ) {
			if ( value.IsNullOrEmpty() || value.Count != 2 ) {
				return null;
			}
			var type = ( ChainTypes.GameSpecificOptions )Convert.ToInt32( value.First );
			switch ( type ) {
			case ChainTypes.GameSpecificOptions.RockPaperScissorsGameOptions:
				return RockPaperScissorsGameOptionsData.Create( value.Last as JObject );
			default:
				Unity.Console.Error( "Unexpected game specific options type:", type );
				return null;
			}
		}

		protected override JArray Serialize( GameSpecificOptionsData value ) {
			if ( value == null ) {
				return new JArray();
			}
			return new JArray( ( int )value.Type, JObject.Parse( value.ToString() ) );
		}
	}
}