using System;
using Base.Config;
using Base.Data.Tournaments.GameSettings;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class GameSpecificSettingsDataPairConverter : JsonCustomConverter<GameSpecificSettingsData, JArray> {

		protected override GameSpecificSettingsData Deserialize( JArray value, Type objectType ) {
			if ( value.IsNullOrEmpty() || value.Count != 2 ) {
				return null;
			}
			var type = ( ChainTypes.GameSpecificSettings )Convert.ToInt32( value.First );
			switch ( type ) {
			case ChainTypes.GameSpecificSettings.RockPaperScissorsGameSettings:
				return RockPaperScissorsGameSettingsData.Create( value.Last as JObject );
			default:
				Unity.Console.Error( "Unexpected game specific settings type:", type );
				return null;
			}
		}

		protected override JArray Serialize( GameSpecificSettingsData value ) {
			if ( value == null ) {
				return new JArray();
			}
			return new JArray( ( int )value.Type, JObject.Parse( value.ToString() ) );
		}
	}
}