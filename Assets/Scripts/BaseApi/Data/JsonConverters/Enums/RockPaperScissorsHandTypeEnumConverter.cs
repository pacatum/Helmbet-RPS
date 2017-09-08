using System;
using Base.Config;


namespace Base.Data.Json {

	public sealed class RockPaperScissorsHandTypeEnumConverter : JsonCustomConverter<ChainTypes.RockPaperScissorsHandType, byte> {

		protected override ChainTypes.RockPaperScissorsHandType Deserialize( byte value, Type objectType ) {
			return ( ChainTypes.RockPaperScissorsHandType )value;
		}

		protected override byte Serialize( ChainTypes.RockPaperScissorsHandType value ) {
			return ( byte )value;
		}
	}
}