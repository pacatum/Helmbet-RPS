using System;
using Base.Config;


namespace Base.Data.Json {

	public sealed class SpaceTypeEnumConverter : JsonCustomConverter<SpaceType, string> {

		readonly static string BASE =					string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.Base );
		readonly static string ACCOUNT =				string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.Account );
		readonly static string ASSET =					string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.Asset );
		readonly static string FORCE_SETTLEMENT =		string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.ForceSettlement );
		readonly static string COMMITTEE_MEMBER =		string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.CommitteeMember );
		readonly static string WITNESS =				string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.Witness );
		readonly static string LIMIT_ORDER =			string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.LimitOrder );
		readonly static string CALL_ORDER =				string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.CallOrder );
		readonly static string CUSTOM =					string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.Custom );
		readonly static string PROPOSAL =				string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.Proposal );
		readonly static string OPERATION_HISTORY =		string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.OperationHistory );
		readonly static string WITHDRAW_PERMISSION =	string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.WithdrawPermission );
		readonly static string VESTING_BALANCE =		string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.VestingBalance );
		readonly static string WORKER =					string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.Worker );
		readonly static string BALANCE =				string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.Balance );
		readonly static string TOURNAMENT =				string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.Tournament );
		readonly static string TOURNAMENT_DETAILS =		string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.TournamentDetails );
		readonly static string MATCHE =					string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.Match );
		readonly static string GAME =					string.Format( "{0}.{1}", ( int )ChainTypes.Space.ProtocolIds, ( int )ChainTypes.ProtocolType.Game );

		readonly static string GLOBAL_PROPERTIES =								string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.GlobalProperties );
		readonly static string DYNAMIC_GLOBAL_PROPERTIES =						string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.DynamicGlobalProperties );
		readonly static string ASSET_DYNAMIC_DATA =								string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.AssetDynamicData );
		readonly static string ASSET_BITASSET_DATA =							string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.AssetBitassetData );
		readonly static string ACCOUNT_BALANCE =								string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.AccountBalance );
		readonly static string ACCOUNT_STATISTICS =								string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.AccountStatistics );
		readonly static string TRANSACTION =									string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.Transaction );
		readonly static string BLOCK_SUMMARY =									string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.BlockSummary );
		readonly static string ACCOUNT_TRANSACTION_HISTORY =					string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.AccountTransactionHistory );
		readonly static string BLINDED_BALANCE =								string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.BlindedBalance );
		readonly static string CHAIN_PROPERTY =									string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.ChainProperty );
		readonly static string WITNESS_SCHEDULE =								string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.WitnessSchedule );
		readonly static string BUDGET_RECORD =									string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.BudgetRecord );
		readonly static string SPECIAL_AUTHORITY =								string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.SpecialAuthority );
		readonly static string BUY_BACK =										string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.BuyBack );
		readonly static string FBA_ACCUMULATOR =								string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.FbaAccumulator );
		readonly static string ASSET_DIVIDEND_DATA =							string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.AssetDividendData );
		readonly static string PENDING_DIVIDEND_PAYOUT_BALANCE_FOR_HOLDER =		string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.PendingDividendPayoutBalanceForHolder );
		readonly static string DISTRIBUTED_DIVIDEND_BALANCE_DATA =				string.Format( "{0}.{1}", ( int )ChainTypes.Space.ImplementationIds, ( int )ChainTypes.ImplementationType.DistributedDividendBalanceData );


		protected override SpaceType Deserialize( string value, Type objectType ) {
			return ConvertFrom( value );
		}

		protected override string Serialize( SpaceType value ) {
			return ConvertTo( value );
		}

		public static string ConvertTo( SpaceType spaceType ) {
			switch ( spaceType ) {
			case SpaceType.Base:									return BASE;
			case SpaceType.Account:									return ACCOUNT;
			case SpaceType.Asset:									return ASSET;
			case SpaceType.ForceSettlement:							return FORCE_SETTLEMENT;
			case SpaceType.CommitteeMember:							return COMMITTEE_MEMBER;
			case SpaceType.Witness:									return WITNESS;
			case SpaceType.LimitOrder:								return LIMIT_ORDER;
			case SpaceType.CallOrder:								return CALL_ORDER;
			case SpaceType.Custom:									return CUSTOM;
			case SpaceType.Proposal:								return PROPOSAL;
			case SpaceType.OperationHistory:						return OPERATION_HISTORY;
			case SpaceType.WithdrawPermission:						return WITHDRAW_PERMISSION;
			case SpaceType.VestingBalance:							return VESTING_BALANCE;
			case SpaceType.Worker:									return WORKER;
			case SpaceType.Balance:									return BALANCE;
			case SpaceType.Tournament:								return TOURNAMENT;
			case SpaceType.TournamentDetails:						return TOURNAMENT_DETAILS;
			case SpaceType.Match:									return MATCHE;
			case SpaceType.Game:									return GAME;
			case SpaceType.GlobalProperties:						return GLOBAL_PROPERTIES;
			case SpaceType.DynamicGlobalProperties:					return DYNAMIC_GLOBAL_PROPERTIES;
			case SpaceType.AssetDynamicData:						return ASSET_DYNAMIC_DATA;
			case SpaceType.AssetBitassetData:						return ASSET_BITASSET_DATA;
			case SpaceType.AccountBalance:							return ACCOUNT_BALANCE;
			case SpaceType.AccountStatistics:						return ACCOUNT_STATISTICS;
			case SpaceType.Transaction:								return TRANSACTION;
			case SpaceType.BlockSummary:							return BLOCK_SUMMARY;
			case SpaceType.AccountTransactionHistory:				return ACCOUNT_TRANSACTION_HISTORY;
			case SpaceType.BlindedBalance:							return BLINDED_BALANCE;
			case SpaceType.ChainProperty:							return CHAIN_PROPERTY;
			case SpaceType.WitnessSchedule:							return WITNESS_SCHEDULE;
			case SpaceType.BudgetRecord:							return BUDGET_RECORD;
			case SpaceType.SpecialAuthority:						return SPECIAL_AUTHORITY;
			case SpaceType.BuyBack:									return BUY_BACK;
			case SpaceType.FbaAccumulator:							return FBA_ACCUMULATOR;
			case SpaceType.AssetDividendData:						return ASSET_DIVIDEND_DATA;
			case SpaceType.PendingDividendPayoutBalanceForHolder:	return PENDING_DIVIDEND_PAYOUT_BALANCE_FOR_HOLDER;
			case SpaceType.DistributedDividendBalanceData:			return DISTRIBUTED_DIVIDEND_BALANCE_DATA;
			}
			return string.Empty;
		}

		public static SpaceType ConvertFrom( string spaceType ) {
			if ( BASE.Equals( spaceType ) )												return SpaceType.Base;
			if ( ACCOUNT.Equals( spaceType ) )											return SpaceType.Account;
			if ( ASSET.Equals( spaceType ) )											return SpaceType.Asset;
			if ( FORCE_SETTLEMENT.Equals( spaceType ) )									return SpaceType.ForceSettlement;
			if ( COMMITTEE_MEMBER.Equals( spaceType ) )									return SpaceType.CommitteeMember;
			if ( WITNESS.Equals( spaceType ) )											return SpaceType.Witness;
			if ( LIMIT_ORDER.Equals( spaceType ) )										return SpaceType.LimitOrder;
			if ( CALL_ORDER.Equals( spaceType ) )										return SpaceType.CallOrder;
			if ( CUSTOM.Equals( spaceType ) )											return SpaceType.Custom;
			if ( PROPOSAL.Equals( spaceType ) )											return SpaceType.Proposal;
			if ( OPERATION_HISTORY.Equals( spaceType ) )								return SpaceType.OperationHistory;
			if ( WITHDRAW_PERMISSION.Equals( spaceType ) )								return SpaceType.WithdrawPermission;
			if ( VESTING_BALANCE.Equals( spaceType ) )									return SpaceType.VestingBalance;
			if ( WORKER.Equals( spaceType ) )											return SpaceType.Worker;
			if ( BALANCE.Equals( spaceType ) )											return SpaceType.Balance;
			if ( TOURNAMENT.Equals( spaceType ) )										return SpaceType.Tournament;
			if ( TOURNAMENT_DETAILS.Equals( spaceType ) )								return SpaceType.TournamentDetails;
			if ( MATCHE.Equals( spaceType ) )											return SpaceType.Match;
			if ( GAME.Equals( spaceType ) )												return SpaceType.Game;
			if ( GLOBAL_PROPERTIES.Equals( spaceType ) )								return SpaceType.GlobalProperties;
			if ( DYNAMIC_GLOBAL_PROPERTIES.Equals( spaceType ) )						return SpaceType.DynamicGlobalProperties;
			if ( ASSET_DYNAMIC_DATA.Equals( spaceType ) )								return SpaceType.AssetDynamicData;
			if ( ASSET_BITASSET_DATA.Equals( spaceType ) )								return SpaceType.AssetBitassetData;
			if ( ACCOUNT_BALANCE.Equals( spaceType ) )									return SpaceType.AccountBalance;
			if ( ACCOUNT_STATISTICS.Equals( spaceType ) )								return SpaceType.AccountStatistics;
			if ( TRANSACTION.Equals( spaceType ) )										return SpaceType.Transaction;
			if ( BLOCK_SUMMARY.Equals( spaceType ) )									return SpaceType.BlockSummary;
			if ( ACCOUNT_TRANSACTION_HISTORY.Equals( spaceType ) )						return SpaceType.AccountTransactionHistory;
			if ( BLINDED_BALANCE.Equals( spaceType ) )									return SpaceType.BlindedBalance;
			if ( CHAIN_PROPERTY.Equals( spaceType ) )									return SpaceType.ChainProperty;
			if ( WITNESS_SCHEDULE.Equals( spaceType ) )									return SpaceType.WitnessSchedule;
			if ( BUDGET_RECORD.Equals( spaceType ) )									return SpaceType.BudgetRecord;
			if ( SPECIAL_AUTHORITY.Equals( spaceType ) )								return SpaceType.SpecialAuthority;
			if ( BUY_BACK.Equals( spaceType ) )											return SpaceType.BuyBack;
			if ( FBA_ACCUMULATOR.Equals( spaceType ) )									return SpaceType.FbaAccumulator;
			if ( ASSET_DIVIDEND_DATA.Equals( spaceType ) )								return SpaceType.AssetDividendData;
			if ( PENDING_DIVIDEND_PAYOUT_BALANCE_FOR_HOLDER.Equals( spaceType ) )		return SpaceType.PendingDividendPayoutBalanceForHolder;
			if ( DISTRIBUTED_DIVIDEND_BALANCE_DATA.Equals( spaceType ) )				return SpaceType.DistributedDividendBalanceData;			
			return SpaceType.Unknown;
		}
	}
}