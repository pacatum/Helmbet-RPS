using Newtonsoft.Json;
using Base.Data.Json;


namespace Base.Config {

	public static class ChainTypes {

		public enum Space : byte {
			
			RelativeProtocolIds/* */= 0,
			ProtocolIds/*         */= 1,
			ImplementationIds/*   */= 2
		}


		public enum ProtocolType : byte {
			
			Null/*                 */= 0,
			Base/*                 */= 1,
			Account/*              */= 2,
			Asset/*                */= 3,
			ForceSettlement/*      */= 4,
			CommitteeMember/*      */= 5,
			Witness/*              */= 6,
			LimitOrder/*           */= 7,
			CallOrder/*            */= 8,
			Custom/*               */= 9,
			Proposal/*             */= 10,
			OperationHistory/*     */= 11,
			WithdrawPermission/*   */= 12,
			VestingBalance/*       */= 13,
			Worker/*               */= 14,
			Balance/*              */= 15,
			Tournament/*           */= 16,
			TournamentDetails/*    */= 17,
			Match/*                */= 18,
			Game/*                 */= 19
		}


		public enum ImplementationType : byte {
			
			GlobalProperties/*                        */= 0,
			DynamicGlobalProperties/*                 */= 1,
			IndexMeta/*                               */= 2,
			AssetDynamicData/*                        */= 3,
			AssetBitassetData/*                       */= 4,
			AccountBalance/*                          */= 5,
			AccountStatistics/*                       */= 6,
			Transaction/*                             */= 7,
			BlockSummary/*                            */= 8,
			AccountTransactionHistory/*               */= 9,
			BlindedBalance/*                          */= 10,
			ChainProperty/*                           */= 11,
			WitnessSchedule/*                         */= 12,
			BudgetRecord/*                            */= 13,
			SpecialAuthority/*                        */= 14,
			BuyBack/*                                 */= 15,
			FbaAccumulator/*                          */= 16,
			AssetDividendData/*                       */= 17,
			PendingDividendPayoutBalanceForHolder/*   */= 18,
			DistributedDividendBalanceData/*          */= 19
		}


		public enum VoteType : byte {
			
			Committee/*   */= 0,
			Witness/*     */= 1,
			Worker/*      */= 2
		}


		public enum Operation : int {
			
			Transfer/*                                */= 0,
			LimitOrderCreate/*                        */= 1,
			LimitOrderCancel/*                        */= 2,
			CallOrderUpdate/*                         */= 3,
			FillOrder/*                               */= 4,
			AccountCreate/*                           */= 5,
			AccountUpdate/*                           */= 6,
			AccountWhitelist/*                        */= 7,
			AccountUpgrade/*                          */= 8,
			AccountTransfer/*                         */= 9,
			AssetCreate/*                             */= 10,
			AssetUpdate/*                             */= 11,
			AssetUpdateBitasset/*                     */= 12,
			AssetUpdateFeedProducers/*                */= 13,
			AssetIssue/*                              */= 14,
			AssetReserve/*                            */= 15,
			AssetFundFeePool/*                        */= 16,
			AssetSettle/*                             */= 17,
			AssetGlobalSettle/*                       */= 18,
			AssetPublishFeed/*                        */= 19,
			WitnessCreate/*                           */= 20,
			WitnessUpdate/*                           */= 21,
			ProposalCreate/*                          */= 22,
			ProposalUpdate/*                          */= 23,
			ProposalDelete/*                          */= 24,
			WithdrawPermissionCreate/*                */= 25,
			WithdrawPermissionUpdate/*                */= 26,
			WithdrawPermissionClaim/*                 */= 27,
			WithdrawPermissionDelete/*                */= 28,
			CommitteeMemberCreate/*                   */= 29,
			CommitteeMemberUpdate/*                   */= 30,
			CommitteeMemberUpdateGlobalParameters/*   */= 31,
			VestingBalanceCreate/*                    */= 32,
			VestingBalanceWithdraw/*                  */= 33,
			WorkerCreate/*                            */= 34,
			Custom/*                                  */= 35,
			Assert/*                                  */= 36,
			BalanceClaim/*                            */= 37,
			OverrideTransfer/*                        */= 38,
			TransferToBlind/*                         */= 39,
			BlindTransfer/*                           */= 40,
			TransferFromBlind/*                       */= 41,
			AssetSettleCancel/*                       */= 42,
			AssetClaimFees/*                          */= 43,
			FbaDistributeOperation/*                  */= 44,
			TournamentCreate/*                        */= 45,
			TournamentJoin/*                          */= 46,
			GameMove/*                                */= 47,
			AssetUpdateDividend/*                     */= 48,
			AssetDividendDistribution/*               */= 49,
			TournamentPayout/*                        */= 50,
			TournamentLeave/*                         */= 51
		}


		public enum FeeParameters : int {
			
			TransferOperation/*                               */= 0,
			LimitOrderCreateOperation/*                       */= 1,
			LimitOrderCancelOperation/*                       */= 2,
			CallOrderUpdateOperation/*                        */= 3,
			FillOrderOperation/*                              */= 4,
			AccountCreateOperation/*                          */= 5,
			AccountUpdateOperation/*                          */= 6,
			AccountWhitelistOperation/*                       */= 7,
			AccountUpgradeOperation/*                         */= 8,
			AccountTransferOperation/*                        */= 9,
			AssetCreateOperation/*                            */= 10,
			AssetUpdateOperation/*                            */= 11,
			AssetUpdateBitassetOperation/*                    */= 12,
			AssetUpdateFeedProducersOperation/*               */= 13,
			AssetIssueOperation/*                             */= 14,
			AssetReserveOperation/*                           */= 15,
			AssetFundFeePoolOperation/*                       */= 16,
			AssetSettleOperation/*                            */= 17,
			AssetGlobalSettleOperation/*                      */= 18,
			AssetPublishFeedOperation/*                       */= 19,
			WitnessCreateOperation/*                          */= 20,
			WitnessUpdateOperation/*                          */= 21,
			ProposalCreateOperation/*                         */= 22,
			ProposalUpdateOperation/*                         */= 23,
			ProposalDeleteOperation/*                         */= 24,
			WithdrawPermissionCreateOperation/*               */= 25,
			WithdrawPermissionUpdateOperation/*               */= 26,
			WithdrawPermissionClaimOperation/*                */= 27,
			WithdrawPermissionDeleteOperation/*               */= 28,
			CommitteeMemberCreateOperation/*                  */= 29,
			CommitteeMemberUpdateOperation/*                  */= 30,
			CommitteeMemberUpdateGlobalParametersOperation/*  */= 31,
			VestingBalanceCreateOperation/*                   */= 32,
			VestingBalanceWithdrawOperation/*                 */= 33,
			WorkerCreateOperation/*                           */= 34,
			CustomOperation/*                                 */= 35,
			AssertOperation/*                                 */= 36,
			BalanceClaimOperation/*                           */= 37,
			OverrideTransferOperation/*                       */= 38,
			TransferToBlindOperation/*                        */= 39,
			BlindTransferOperation/*                          */= 40,
			TransferFromBlindOperation/*                      */= 41,
			AssetSettleCancelOperation/*                      */= 42,
			AssetClaimFeesOperation/*                         */= 43,
			FbaDistributeOperation/*                          */= 44,
			TournamentCreateOperation/*                       */= 45,
			TournamentJoinOperation/*                         */= 46,
			GameMoveOperation/*                               */= 47,
			AssetUpdateDividendOperation/*                    */= 48,
			AssetDividendDistributionOperation/*              */= 49,
			TournamentPayoutOperation/*                       */= 50,
			TournamentLeaveOperation/*                        */= 51
		}


		public enum OperationResult : int {
			
			Void/*            */= 0,
			SpaceTypeId/*     */= 1,
			Asset/*           */= 2
		}


		public enum VestingPolicyInitializer : int {
			
			Linear/*          */= 0,
			Cdd/*             */= 1
		}


		public enum WorkerInitializer : int {
			
			Refund/*          */= 0,
			VestingBalance/*  */= 1,
			Burn/*            */= 2
		}


		public enum Predicate : int {
			
			AccountName/*     */= 0,
			AssetSymbol/*     */= 1,
			BlockId/*         */= 2
		}

		public enum GameSpecificSettings : int {
			
			RockPaperScissorsGameSettings/*   */= 0
		}

		public enum GameSpecificOptions : int {
			
			RockPaperScissorsGameOptions/*    */= 0
		}

		public enum GameSpecificDetails : int {
			
			RockPaperScissorsGameDetails/*    */= 0
		}

		public enum GameSpecificMoves : int {
			
			RockPaperScissorsThrowCommit/*    */= 0,
			RockPaperScissorsThrowReveal/*    */= 1
		}


		[JsonConverter( typeof( RockPaperScissorsGestureEnumConverter ) )]
		public enum RockPaperScissorsGesture : int {
			
			Rock/*        */= 0,
			Paper/*       */= 1,
			Scissors/*    */= 2,
			Spock/*       */= 3,
			Lizard/*      */= 4
		}


		[JsonConverter( typeof( RockPaperScissorsHandTypeEnumConverter ) )]
		public enum RockPaperScissorsHandType : int {
			
			Man/*         */= 0,
			Woman/*       */= 1,
			Robot/*       */= 2
		}


		[JsonConverter( typeof( RockPaperScissorsHandColorEnumConverter ) )]
		public enum RockPaperScissorsHandColor : int {
			
			Dark/*        */= 0,
			Light/*       */= 1
		}


		[JsonConverter( typeof( MatchStateEnumConverter ) )]
		public enum MatchState {
			
			WaitingOnPreviousMatches,
			InProgress,
			Complete
		}


		[JsonConverter( typeof( GameStateEnumConverter ) )]
		public enum GameState {
			
			ExpectingCommitMoves,
			ExpectingRevealMoves,
			Complete
		}


		[JsonConverter( typeof( TournamentStateEnumConverter ) )]
		public enum TournamentState {
			
			AcceptingRegistrations,
			AwaitingStart,
			InProgress,
			RegistrationPeriodExpired,
			Concluded                   
		}


		[JsonConverter( typeof( PayoutTypeEnumConverter ) )]
		public enum PayoutType : int {
			
			PrizeAward/*      */= 0,
			BuyInRefund/*     */= 1,
			RakeFee/*         */= 2
		}
	}
}