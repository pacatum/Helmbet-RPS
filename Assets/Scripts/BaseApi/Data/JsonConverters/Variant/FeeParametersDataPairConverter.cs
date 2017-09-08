using System;
using Base.Config;
using Base.Data.Operations.Fee;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class FeeParametersDataPairConverter : JsonCustomConverter<FeeParametersData, JArray> {

		protected override FeeParametersData Deserialize( JArray value, Type objectType ) {
			if ( value.IsNullOrEmpty() || value.Count != 2 ) {
				return null;
			}
			var type = ( ChainTypes.FeeParameters )Convert.ToInt32( value.First );
			switch ( type ) {
			case ChainTypes.FeeParameters.TransferOperation:
				return TransferOperationFeeParametersData.Create( value.Last as JObject );
			case ChainTypes.FeeParameters.AccountCreateOperation:
				return AccountCreateOperationFeeParametersData.Create( value.Last as JObject );
			case ChainTypes.FeeParameters.TournamentCreateOperation:
				return TournamentCreateOperationFeeParametersData.Create( value.Last as JObject );
			case ChainTypes.FeeParameters.TournamentJoinOperation:
				return TournamentJoinOperationFeeParametersData.Create( value.Last as JObject );
			case ChainTypes.FeeParameters.GameMoveOperation:
				return GameMoveOperationFeeParametersData.Create( value.Last as JObject );
			case ChainTypes.FeeParameters.ProposalCreateOperation:
				return ProposalCreateOperationFeeParametersData.Create( value.Last as JObject );
			case ChainTypes.FeeParameters.LimitOrderCreateOperation:
			case ChainTypes.FeeParameters.LimitOrderCancelOperation:
			case ChainTypes.FeeParameters.CallOrderUpdateOperation:
			case ChainTypes.FeeParameters.FillOrderOperation:
			case ChainTypes.FeeParameters.AccountUpdateOperation:
			case ChainTypes.FeeParameters.AccountWhitelistOperation:
			case ChainTypes.FeeParameters.AccountUpgradeOperation:
			case ChainTypes.FeeParameters.AccountTransferOperation:
			case ChainTypes.FeeParameters.AssetCreateOperation:
			case ChainTypes.FeeParameters.AssetUpdateOperation:
			case ChainTypes.FeeParameters.AssetUpdateBitassetOperation:
			case ChainTypes.FeeParameters.AssetUpdateFeedProducersOperation:
			case ChainTypes.FeeParameters.AssetIssueOperation:
			case ChainTypes.FeeParameters.AssetReserveOperation:
			case ChainTypes.FeeParameters.AssetFundFeePoolOperation:
			case ChainTypes.FeeParameters.AssetSettleOperation:
			case ChainTypes.FeeParameters.AssetGlobalSettleOperation:
			case ChainTypes.FeeParameters.AssetPublishFeedOperation:
			case ChainTypes.FeeParameters.WitnessCreateOperation:
			case ChainTypes.FeeParameters.WitnessUpdateOperation:
			case ChainTypes.FeeParameters.ProposalUpdateOperation:
			case ChainTypes.FeeParameters.ProposalDeleteOperation:
			case ChainTypes.FeeParameters.WithdrawPermissionCreateOperation:
			case ChainTypes.FeeParameters.WithdrawPermissionUpdateOperation:
			case ChainTypes.FeeParameters.WithdrawPermissionClaimOperation:
			case ChainTypes.FeeParameters.WithdrawPermissionDeleteOperation:
			case ChainTypes.FeeParameters.CommitteeMemberCreateOperation:
			case ChainTypes.FeeParameters.CommitteeMemberUpdateOperation:
			case ChainTypes.FeeParameters.CommitteeMemberUpdateGlobalParametersOperation:
			case ChainTypes.FeeParameters.VestingBalanceCreateOperation:
			case ChainTypes.FeeParameters.VestingBalanceWithdrawOperation:
			case ChainTypes.FeeParameters.WorkerCreateOperation:
			case ChainTypes.FeeParameters.CustomOperation:
			case ChainTypes.FeeParameters.AssertOperation:
			case ChainTypes.FeeParameters.BalanceClaimOperation:
			case ChainTypes.FeeParameters.OverrideTransferOperation:
			case ChainTypes.FeeParameters.TransferToBlindOperation:
			case ChainTypes.FeeParameters.BlindTransferOperation:
			case ChainTypes.FeeParameters.TransferFromBlindOperation:
			case ChainTypes.FeeParameters.AssetSettleCancelOperation:
			case ChainTypes.FeeParameters.AssetClaimFeesOperation:
			case ChainTypes.FeeParameters.FbaDistributeOperation:
			case ChainTypes.FeeParameters.AssetUpdateDividendOperation:
			case ChainTypes.FeeParameters.AssetDividendDistributionOperation:
				// skip
				return null;
			default:
				Unity.Console.Error( "Unexpected fee parameters type:", type );
				return null;
			}
		}

		protected override JArray Serialize( FeeParametersData value ) {
			if ( value == null ) {
				return new JArray();
			}
			return new JArray( ( int )value.Type, JObject.Parse( value.ToString() ) );
		}
	}
}