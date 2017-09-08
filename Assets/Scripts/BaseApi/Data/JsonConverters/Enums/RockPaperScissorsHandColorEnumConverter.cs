using System;
using Base.Config;


namespace Base.Data.Json {

	public sealed class RockPaperScissorsHandColorEnumConverter : JsonCustomConverter<ChainTypes.RockPaperScissorsHandColor, byte> {

		protected override ChainTypes.RockPaperScissorsHandColor Deserialize( byte value, Type objectType ) {
			return ( ChainTypes.RockPaperScissorsHandColor )value;
		}

		protected override byte Serialize( ChainTypes.RockPaperScissorsHandColor value ) {
			return ( byte )value;
		}
	}
}