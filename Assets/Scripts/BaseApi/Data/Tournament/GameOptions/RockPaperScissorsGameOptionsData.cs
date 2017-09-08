using Buffers;
using Base.Config;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Tournaments.GameOptions {

	public sealed class RockPaperScissorsGameOptionsData : GameSpecificOptionsData {

		const int RPS_CLASSIC_GESTURES_COUNT = 3;

		const string INSURANCE_ENABLED_FIELD_KEY = "insurance_enabled";
		const string TIME_PER_COMMIT_MOVE_FIELD_KEY = "time_per_commit_move";
		const string TIME_PER_REVEAL_MOVE_FIELD_KEY = "time_per_reveal_move";
		const string NUMBER_OF_GESTURES_FIELD_KEY = "number_of_gestures";

		public bool InsuranceEnabled { get; set; }
		public uint TimePerCommitMove { get; set; }
		public uint TimePerRevealMove { get; set; }
		public byte NumberOfGestures { get; set; }

		public override ChainTypes.GameSpecificOptions Type {
			get { return ChainTypes.GameSpecificOptions.RockPaperScissorsGameOptions; }
		}

		public RockPaperScissorsGameOptionsData() {
			NumberOfGestures = RPS_CLASSIC_GESTURES_COUNT;
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteBool( InsuranceEnabled );
			buffer.WriteUInt32( TimePerCommitMove );
			buffer.WriteUInt32( TimePerRevealMove );
			buffer.WriteUInt8( NumberOfGestures );
			return buffer;
		}

		public override string Serialize() {
			return new JSONBuilder( new JSONDictionary {
				{ INSURANCE_ENABLED_FIELD_KEY,      InsuranceEnabled },
				{ TIME_PER_COMMIT_MOVE_FIELD_KEY,   TimePerCommitMove },
				{ TIME_PER_REVEAL_MOVE_FIELD_KEY,   TimePerRevealMove },
				{ NUMBER_OF_GESTURES_FIELD_KEY,     NumberOfGestures }
			} ).Build();
		}

		public static RockPaperScissorsGameOptionsData Create( JObject value ) {
			var token = value.Root;
			var instance = new RockPaperScissorsGameOptionsData();
			instance.InsuranceEnabled = value.TryGetValue( INSURANCE_ENABLED_FIELD_KEY, out token ) ? token.ToObject<bool>() : false;
			instance.TimePerCommitMove = value.TryGetValue( TIME_PER_COMMIT_MOVE_FIELD_KEY, out token ) ? token.ToObject<uint>() : uint.MinValue;
			instance.TimePerRevealMove = value.TryGetValue( TIME_PER_REVEAL_MOVE_FIELD_KEY, out token ) ? token.ToObject<uint>() : uint.MinValue;
			instance.NumberOfGestures = value.TryGetValue( NUMBER_OF_GESTURES_FIELD_KEY, out token ) ? token.ToObject<byte>() : byte.MinValue;
			return instance;
		}
	}
}