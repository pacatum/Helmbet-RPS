using Buffers;
using Base.Config;
using Base.Data.Tournaments.GameMoves;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Tournaments.GameDetails {

	public sealed class RockPaperScissorsGameDetailsData : GameSpecificDetailsData {

		const string COMMIT_MOVES_FIELD_KEY = "commit_moves";
		const string REVEAL_MOVES_FIELD_KEY = "reveal_moves";

		public RockPaperScissorsThrowCommitData[] CommitMoves { get; set; }
		public RockPaperScissorsThrowRevealData[] RevealMoves { get; set; }

		public override ChainTypes.GameSpecificDetails Type {
			get { return ChainTypes.GameSpecificDetails.RockPaperScissorsGameDetails; }
		}

		public RockPaperScissorsGameDetailsData() {
			CommitMoves = new RockPaperScissorsThrowCommitData[ 0 ];
			RevealMoves = new RockPaperScissorsThrowRevealData[ 0 ];
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteArray( CommitMoves, ( b, item ) => {
				if ( !item.IsNull() ) {
					item.ToBufferRaw( b );
				}
			} );
			buffer.WriteArray( RevealMoves, ( b, item ) => {
				if ( !item.IsNull() ) {
					item.ToBufferRaw( b );
				}
			} );
			return buffer;
		}

		public override string Serialize() {
			return new JSONBuilder()
				.WriteKeyValuesPair( COMMIT_MOVES_FIELD_KEY, CommitMoves, commitMove => commitMove.ToNullableString() )
				.WriteKeyValuesPair( REVEAL_MOVES_FIELD_KEY, RevealMoves, revealMove => revealMove.ToNullableString() )
				.Build();
		}

		public static RockPaperScissorsGameDetailsData Create( JObject value ) {
			var token = value.Root;
			var instance = new RockPaperScissorsGameDetailsData();
			instance.CommitMoves = value.TryGetValue( COMMIT_MOVES_FIELD_KEY, out token ) ? token.ToObject<RockPaperScissorsThrowCommitData[]>() : new RockPaperScissorsThrowCommitData[ 0 ];
			instance.RevealMoves = value.TryGetValue( REVEAL_MOVES_FIELD_KEY, out token ) ? token.ToObject<RockPaperScissorsThrowRevealData[]>() : new RockPaperScissorsThrowRevealData[ 0 ];
			return instance;
		}
	}
}