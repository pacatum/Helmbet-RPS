using System;
using Buffers;
using Base.Config;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Tournaments.GameMoves {

	public class RockPaperScissorsThrowCommitData : GameSpecificMovesData {

		const string NONCE1_FIELD_KEY = "nonce1";
		const string THROW_HASH_FIELD_KEY = "throw_hash";

		public ulong Nonce1 { get; private set; }
		public string ThrowHash { get; private set; }

		public override ChainTypes.GameSpecificMoves Type {
			get { return ChainTypes.GameSpecificMoves.RockPaperScissorsThrowCommit; }
		}

		public RockPaperScissorsThrowCommitData() {
			ThrowHash = string.Empty;
		}

		public RockPaperScissorsThrowCommitData( ulong nonce1, byte[] throwHash ) {
			if ( throwHash.Length != 32 ) {
				throw new ArgumentException( "Expecting 32 bytes throwHash, instead got " + throwHash.Length );
			}
			Nonce1 = nonce1;
			ThrowHash = Tool.ToHex( throwHash );
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteUInt64( Nonce1 );
			buffer.WriteBytes( Tool.FromHex( ThrowHash ), false );
			return buffer;
		}

		public override string Serialize() {
			return new JSONBuilder( new JSONDictionary {
				{ NONCE1_FIELD_KEY,         Tool.WrapUInt64( Nonce1 ) },
				{ THROW_HASH_FIELD_KEY,     ThrowHash }
			} ).Build();
		}

		public static RockPaperScissorsThrowCommitData Create( JObject value ) {
			var token = value.Root;
			var instance = new RockPaperScissorsThrowCommitData();
			instance.Nonce1 = Convert.ToUInt64( value.TryGetValue( NONCE1_FIELD_KEY, out token ) ? token.ToObject<object>() : 0 );
			instance.ThrowHash = value.TryGetValue( THROW_HASH_FIELD_KEY, out token ) ? token.ToObject<string>() : string.Empty;
			return instance;
		}
	}
}