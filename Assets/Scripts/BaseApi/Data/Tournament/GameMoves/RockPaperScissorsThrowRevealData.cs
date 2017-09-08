using System;
using Buffers;
using Base.Config;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Tournaments.GameMoves {

	public class RockPaperScissorsThrowRevealData : GameSpecificMovesData {

		const string NONCE2_FIELD_KEY = "nonce2";
		const string GESTURE_FIELD_KEY = "gesture";

		public ulong Nonce2 { get; private set; }
		public ChainTypes.RockPaperScissorsGesture Gesture { get; private set; }

		public override ChainTypes.GameSpecificMoves Type {
			get { return ChainTypes.GameSpecificMoves.RockPaperScissorsThrowReveal; }
		}

		public RockPaperScissorsThrowRevealData() { }

		public RockPaperScissorsThrowRevealData( ulong nonce2, ChainTypes.RockPaperScissorsGesture gesture ) {
			Nonce2 = nonce2;
			Gesture = gesture;
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteUInt64( Nonce2 );
			buffer.WriteEnum( ( int )Gesture );
			return buffer;
		}

		public override string Serialize() {
			return new JSONBuilder( new JSONDictionary {
				{ NONCE2_FIELD_KEY,    	Tool.WrapUInt64( Nonce2 ) },
				{ GESTURE_FIELD_KEY,	Gesture }
			} ).Build();
		}

		public static RockPaperScissorsThrowRevealData Create( JObject value ) {
			var token = value.Root;
			var instance = new RockPaperScissorsThrowRevealData();
			instance.Nonce2 = Convert.ToUInt64( value.TryGetValue( NONCE2_FIELD_KEY, out token ) ? token.ToObject<object>() : 0 );
			instance.Gesture = value.TryGetValue( GESTURE_FIELD_KEY, out token ) ? token.ToObject<ChainTypes.RockPaperScissorsGesture>() : ( ChainTypes.RockPaperScissorsGesture )(-1);
			return instance;
		}
	}
}