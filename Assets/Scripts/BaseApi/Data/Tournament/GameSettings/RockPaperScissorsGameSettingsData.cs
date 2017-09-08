using Buffers;
using Base.Config;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Tournaments.GameSettings {

	public sealed class RockPaperScissorsGameSettingsData : GameSpecificSettingsData {

		const string HAND_TYPE_FIELD_KEY = "hand_type";
		const string HAND_COLOR_FIELD_KEY = "hand_color";

		public ChainTypes.RockPaperScissorsHandType HandType { get; set; }
		public ChainTypes.RockPaperScissorsHandColor HandColor { get; set; }

		public override ChainTypes.GameSpecificSettings Type {
			get { return ChainTypes.GameSpecificSettings.RockPaperScissorsGameSettings; }
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteEnum( ( int )HandType );
			buffer.WriteEnum( ( int )HandColor );
			return buffer;
		}

		public override string Serialize() {
			return new JSONBuilder( new JSONDictionary {
				{ HAND_TYPE_FIELD_KEY,      HandType },
				{ HAND_COLOR_FIELD_KEY,     HandColor }
			} ).Build();
		}

		public static RockPaperScissorsGameSettingsData Create( JObject value ) {
			var token = value.Root;
			var instance = new RockPaperScissorsGameSettingsData();
			instance.HandType = value.TryGetValue( HAND_TYPE_FIELD_KEY, out token ) ? token.ToObject<ChainTypes.RockPaperScissorsHandType>() : ( ChainTypes.RockPaperScissorsHandType )(-1);
			instance.HandColor = value.TryGetValue( HAND_COLOR_FIELD_KEY, out token ) ? token.ToObject<ChainTypes.RockPaperScissorsHandColor>() : ( ChainTypes.RockPaperScissorsHandColor )(-1);
			return instance;
		}
	}
}