using System;
using Base.Config;
using Base.Data;
using Base.Data.Assets;
using Base.Data.Operations;
using Base.Data.Tournaments;
using Base.Data.Tournaments.GameOptions;
using Base.Data.Transactions;
using Newtonsoft.Json.Linq;
using Promises;
using Tools;


namespace Base.Transactions.Tournaments {

	public class CreateTournamentData {

		public uint buyInAssetId;
		public double buyInAmount;
		public SpaceTypeId[] whitelist;
		public SpaceTypeId account;
		public DateTime registrationDeadline;
		public uint numberOfPlayers;
		public uint numberOfWins;
		public DateTime? startTime;
		public uint? startDelay; // in seconds
		public uint roundDelay; // in seconds
		public bool insuranceEnabled;
		public uint timePerCommitMove;
		public uint timePerRevealMove;
	}


	public class JoinToTournamentData {

		public SpaceTypeId account;
		public TournamentObject tournament;
	}


	public class LeaveFromTournamentData {

		public SpaceTypeId account;
		public TournamentObject tournament;
	}


	public static class TournamentTransactionService {

		public static event Action<SpaceTypeId> OnCreateTournamentResult;
		public static event Action<SpaceTypeId> OnJoinToTournamentResult;
		public static event Action<SpaceTypeId> OnLeaveFromTournamentResult;


		public static IPromise<TournamentCreateOperationData> GenerateCreateTournamentOperation( CreateTournamentData data ) {
			Func<AssetObject, IPromise<TournamentCreateOperationData>> GenerateOperation = assetObject => Promise<TournamentCreateOperationData>.Resolved( new TournamentCreateOperationData {
				Fee = new AssetData( 0L, assetObject.Id ),
				Creator = data.account,
				Options = new TournamentOptionsData {
					RegistrationDeadline = data.registrationDeadline,
					NumberOfPlayers = data.numberOfPlayers,
					BuyIn = new AssetData( ( long )(data.buyInAmount * Math.Pow( 10, assetObject.Precision )), assetObject.Id ),
					Whitelist = data.whitelist.OrEmpty(),
					StartTime = data.startTime,
					StartDelay = data.startDelay,
					RoundDelay = data.roundDelay,
					NumberOfWins = data.numberOfWins,
					Meta = new CustomData(),
					GameOptions = new RockPaperScissorsGameOptionsData {
						InsuranceEnabled = data.insuranceEnabled,
						TimePerCommitMove = data.timePerCommitMove,
						TimePerRevealMove = data.timePerRevealMove
					}
				}
			} );
			var asset = SpaceTypeId.CreateOne( SpaceType.Asset, data.buyInAssetId );
			return Repository.GetInPromise( asset, () => ApiManager.Instance.Database.GetAsset( asset.Id ) ).Then( GenerateOperation );
		}

		public static IPromise CreateTournament( TournamentCreateOperationData operation ) {
			return AuthorizationManager.Instance.ProcessTransaction( new TransactionBuilder().AddOperation( operation ), CreateTournamentResult );
		}

		static void CreateTournamentResult( JToken[] createTournamentResults ) {
			var results = Array.ConvertAll( createTournamentResults, jO => jO.ToObject<TransactionConfirmation>() );
			foreach ( var result in results ) {
				if ( !result.Transaction.OperationResults.IsNullOrEmpty() ) {
					var operationResult = result.Transaction.OperationResults[ 0 ];
					if ( operationResult.Type.Equals( ChainTypes.OperationResult.SpaceTypeId ) ) {
						if ( !OnCreateTournamentResult.IsNull() ) {
							OnCreateTournamentResult( ( SpaceTypeId )operationResult.Value );
						}
					}
				}
			}
		}

		public static IPromise<TournamentJoinOperationData> GenerateJoinToTournamentOperation( JoinToTournamentData data ) {
			Func<AssetObject, IPromise<TournamentJoinOperationData>> GenerateOperation = assetObject => Promise<TournamentJoinOperationData>.Resolved( new TournamentJoinOperationData {
				Fee = new AssetData( 0L, assetObject.Id ),
				Payer = data.account,
				Player = data.account,
				Tournament = data.tournament.Id,
				BuyIn = data.tournament.Options.BuyIn
			} );
			var asset = data.tournament.Options.BuyIn.Asset;
			return Repository.GetInPromise( asset, () => ApiManager.Instance.Database.GetAsset( asset.Id ) ).Then( GenerateOperation );
		}

		public static IPromise JoinToTournament( TournamentJoinOperationData operation ) {
			return AuthorizationManager.Instance.ProcessTransaction( new TransactionBuilder().AddOperation( operation ), JoinToTournamentResult );
		}

		static void JoinToTournamentResult( JToken[] joinToTournamentResults ) {
			var results = Array.ConvertAll( joinToTournamentResults, jO => jO.ToObject<TransactionConfirmation>() );
			foreach ( var result in results ) {
				if ( !result.Transaction.OperationResults.IsNullOrEmpty() ) {
					var operationResult = result.Transaction.OperationResults[ 0 ];
					if ( operationResult.Type.Equals( ChainTypes.OperationResult.Void ) ) {
						if ( !result.Transaction.Operations.IsNullOrEmpty() ) {
							var operation = result.Transaction.Operations[ 0 ];
							if ( operation.Type.Equals( ChainTypes.Operation.TournamentJoin ) ) {
								if ( !OnJoinToTournamentResult.IsNull() ) {
									OnJoinToTournamentResult( (operation as TournamentJoinOperationData).Tournament );
								}
							}
						}
					}
				}
			}
		}

		public static IPromise<TournamentLeaveOperationData> GenerateLeaveFromTournamentOperation( LeaveFromTournamentData data ) {
			Func<AssetObject, IPromise<TournamentLeaveOperationData>> GenerateOperation = assetObject => Promise<TournamentLeaveOperationData>.Resolved( new TournamentLeaveOperationData {
				Fee = new AssetData( 0L, assetObject.Id ),
				Canceling = data.account,
				Player = data.account,
				Tournament = data.tournament.Id,
			} );
			var asset = data.tournament.Options.BuyIn.Asset;
			return Repository.GetInPromise( asset, () => ApiManager.Instance.Database.GetAsset( asset.Id ) ).Then( GenerateOperation );
		}

		public static IPromise LeaveFromTournament( TournamentLeaveOperationData operation ) {
			return AuthorizationManager.Instance.ProcessTransaction( new TransactionBuilder().AddOperation( operation ), LeaveFromTournamentResult );
		}

		static void LeaveFromTournamentResult( JToken[] joinToTournamentResults ) {
			var results = Array.ConvertAll( joinToTournamentResults, jO => jO.ToObject<TransactionConfirmation>() );
			foreach ( var result in results ) {
				if ( !result.Transaction.OperationResults.IsNullOrEmpty() ) {
					var operationResult = result.Transaction.OperationResults[ 0 ];
					if ( operationResult.Type.Equals( ChainTypes.OperationResult.Void ) ) {
						if ( !result.Transaction.Operations.IsNullOrEmpty() ) {
							var operation = result.Transaction.Operations[ 0 ];
							if ( operation.Type.Equals( ChainTypes.Operation.TournamentLeave ) ) {
								if ( !OnLeaveFromTournamentResult.IsNull() ) {
									OnLeaveFromTournamentResult( (operation as TournamentLeaveOperationData).Tournament );
								}
							}
						}
					}
				}
			}
		}
	}
}