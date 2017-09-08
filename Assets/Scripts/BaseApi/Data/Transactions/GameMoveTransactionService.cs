using System;
using System.Security.Cryptography;
using Base.Config;
using Base.Data;
using Base.Data.Operations;
using Base.Data.Tournaments;
using Base.Data.Tournaments.GameMoves;
using Base.Data.Transactions;
using Newtonsoft.Json.Linq;
using Promises;
using Tools;


namespace Base.Transactions.GameMoves {

	public static class GameMoveTransactionService {

		class CommitMoveData {

			public SpaceTypeId game;
			public SpaceTypeId match;
			public SpaceTypeId player;
			public ulong nonce1;
			public byte[] hash;
		}


		class RevealMoveData {

			public SpaceTypeId game;
			public SpaceTypeId match;
			public SpaceTypeId player;
			public ulong nonce2;
			public ChainTypes.RockPaperScissorsGesture gesture;
		}


		public class CommitAndReveal {

			readonly ulong nonce1;
			readonly ulong nonce2;
			readonly byte[] hash;
			readonly ChainTypes.RockPaperScissorsGesture gesture;
			readonly SpaceTypeId player;

			GameObject game;

			public bool IsCommited { get; private set; }
			public bool IsRevealed { get; private set; }


			public CommitAndReveal( ChainTypes.RockPaperScissorsGesture gesture, SpaceTypeId player ) {
				this.gesture = gesture;
				this.player = player;
				nonce1 = GetRandomULong();
				nonce2 = GetRandomULong();
				hash = SHA256.Create().HashAndDispose( Tool.ToBuffer( buffer => {
					buffer.WriteUInt64( nonce1 );
					buffer.WriteUInt64( nonce2 );
					buffer.WriteEnum( ( int )gesture );
				} ).ToArray() );
				IsCommited = false;
				IsRevealed = false;
			}

			static ulong GetRandomULong() {
				var rnd = new Random( UnityEngine.Random.Range( int.MinValue, int.MaxValue ) );
				var buffer = new byte[ 8 ];
				rnd.NextBytes( buffer );
				return BitConverter.ToUInt64( buffer, 0 );
			}

			public void Commit( GameObject game ) {
				if ( !IsCommited ) {
					this.game = game;
					var data = new CommitMoveData {
						game = game.Id,
						match = game.Match,
						player = player,
						nonce1 = nonce1,
						hash = hash
					};
					GenerateCommitMoveOperation( data ).Then( operation => CommitMove( operation ) );
					IsCommited = true;
				}
			}

			public void Reveal() {
				if ( IsCommited && !IsRevealed ) {
					var data = new RevealMoveData {
						game = game.Id,
						match = game.Match,
						player = player,
						nonce2 = nonce2,
						gesture = gesture
					};
					GenerateRevealMoveOperation( data ).Then( operation => RevealMove( operation ) );
					IsRevealed = true;
				}
			}
		}


		public static event Action<SpaceTypeId> OnCommitMoveResult;
		public static event Action<SpaceTypeId> OnRevealMoveResult;


		static IPromise<GameMoveOperationData> GenerateCommitMoveOperation( CommitMoveData data ) {
			return Repository.GetInPromise( data.match, () => ApiManager.Instance.Database.GetMatche( data.match.Id ) ).Then( match =>
				Repository.GetInPromise( match.Tournament, () => ApiManager.Instance.Database.GetTournament( match.Tournament.Id ) ).Then( tournament => new GameMoveOperationData {
					Fee = new AssetData( 0L, tournament.Options.BuyIn.Asset ),
					Game = data.game,
					Player = data.player,
					Move = new RockPaperScissorsThrowCommitData( data.nonce1, data.hash )
				} )
			);
		}

		static IPromise CommitMove( GameMoveOperationData operation ) {
			return AuthorizationManager.Instance.ProcessTransaction( new TransactionBuilder().AddOperation( operation ), CommitMoveResult );
		}

		static void CommitMoveResult( JToken[] commitMoveResult ) {
			var results = Array.ConvertAll( commitMoveResult, jO => jO.ToObject<TransactionConfirmation>() );
			foreach ( var result in results ) {
				if ( !result.Transaction.OperationResults.IsNullOrEmpty() ) {
					var operationResult = result.Transaction.OperationResults[ 0 ];
					if ( operationResult.Type.Equals( ChainTypes.OperationResult.Void ) ) {
						if ( !result.Transaction.Operations.IsNullOrEmpty() ) {
							var operation = result.Transaction.Operations[ 0 ];
							if ( operation.Type.Equals( ChainTypes.Operation.GameMove ) ) {
								var gameMoveOperation = operation as GameMoveOperationData;
								if ( gameMoveOperation.Move.Type.Equals( ChainTypes.GameSpecificMoves.RockPaperScissorsThrowCommit ) ) {
									if ( !OnCommitMoveResult.IsNull() ) {
										OnCommitMoveResult( gameMoveOperation.Game );
									}
								}
							}
						}
					}
				}
			}
		}

		static IPromise<GameMoveOperationData> GenerateRevealMoveOperation( RevealMoveData data ) {
			return Repository.GetInPromise( data.match, () => ApiManager.Instance.Database.GetMatche( data.match.Id ) ).Then( match =>
				Repository.GetInPromise( match.Tournament, () => ApiManager.Instance.Database.GetTournament( match.Tournament.Id ) ).Then( tournament => new GameMoveOperationData {
					Fee = new AssetData( 0L, tournament.Options.BuyIn.Asset ),
					Game = data.game,
					Player = data.player,
					Move = new RockPaperScissorsThrowRevealData( data.nonce2, data.gesture )
				} )
			);
		}

		static IPromise RevealMove( GameMoveOperationData operation ) {
			return AuthorizationManager.Instance.ProcessTransaction( new TransactionBuilder().AddOperation( operation ), RevealMoveResult );
		}

		static void RevealMoveResult( JToken[] revealMoveResult ) {
			var results = Array.ConvertAll( revealMoveResult, jO => jO.ToObject<TransactionConfirmation>() );
			foreach ( var result in results ) {
				if ( !result.Transaction.OperationResults.IsNullOrEmpty() ) {
					var operationResult = result.Transaction.OperationResults[ 0 ];
					if ( operationResult.Type.Equals( ChainTypes.OperationResult.Void ) ) {
						if ( !result.Transaction.Operations.IsNullOrEmpty() ) {
							var operation = result.Transaction.Operations[ 0 ];
							if ( operation.Type.Equals( ChainTypes.Operation.GameMove ) ) {
								var gameMoveOperation = operation as GameMoveOperationData;
								if ( gameMoveOperation.Move.Type.Equals( ChainTypes.GameSpecificMoves.RockPaperScissorsThrowReveal ) ) {
									if ( !OnRevealMoveResult.IsNull() ) {
										OnRevealMoveResult( gameMoveOperation.Game );
									}
								}
							}
						}
					}
				}
			}
		}
	}
}