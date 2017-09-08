using System;
using Base.Data.Json;
using Base.ECC;
using Tools;
using Newtonsoft.Json;


namespace Base.Data.Accounts {

	// id "1.2.x"
	public sealed class AccountObject : IdObject {

		[JsonProperty( "membership_expiration_date" ), JsonConverter( typeof( DateTimeConverter ) )]
		public DateTime MembershipExpirationDate { get; private set; }
		[JsonProperty( "registrar" )]
		public SpaceTypeId Registrar { get; private set; }
		[JsonProperty( "referrer" )]
		public SpaceTypeId Referrer { get; private set; }
		[JsonProperty( "lifetime_referrer" )]
		public SpaceTypeId LifetimeReferrer { get; private set; }
		[JsonProperty( "network_fee_percentage" )]
		public ushort NetworkFeePercentage { get; private set; }
		[JsonProperty( "lifetime_referrer_fee_percentage" )]
		public ushort LifetimeReferrerFeePercentage { get; private set; }
		[JsonProperty( "referrer_rewards_percentage" )]
		public ushort ReferrerRewardsPercentage { get; private set; }
		[JsonProperty( "name" )]
		public string Name { get; private set; }
		[JsonProperty( "owner" )]
		public AuthorityData Owner { get; private set; }
		[JsonProperty( "active" )]
		public AuthorityData Active { get; private set; }
		[JsonProperty( "options" )]
		public AccountOptionsData Options { get; private set; }
		[JsonProperty( "statistics" )]
		public SpaceTypeId Statistics { get; private set; }
		[JsonProperty( "whitelisting_accounts" )]
		public SpaceTypeId[] WhitelistingAccounts { get; private set; }
		[JsonProperty( "whitelisted_accounts" )]
		public SpaceTypeId[] WhitelistedAccounts { get; private set; }
		[JsonProperty( "blacklisted_accounts" )]
		public SpaceTypeId[] BlacklistedAccounts { get; private set; }
		[JsonProperty( "blacklisting_accounts" )]
		public SpaceTypeId[] BlacklistingAccounts { get; private set; }
		[JsonProperty( "cashback_vb", NullValueHandling = NullValueHandling.Ignore )]
		public SpaceTypeId CashbackVestingBalance { get; private set; }
		[JsonProperty( "owner_special_authority" )]
		public object[] OwnerSpecialAuthority { get; private set; }         // todo
		[JsonProperty( "active_special_authority" )]
		public object[] ActiveSpecialAuthority { get; private set; }        // todo
		[JsonProperty( "top_n_control_flags" )]
		public byte TopNControlFlags { get; private set; }
		[JsonProperty( "allowed_assets", NullValueHandling = NullValueHandling.Ignore )]
		public SpaceTypeId[] AllowedAssets { get; private set; }

		public bool IsEquelKey( AccountRole role, KeyPair key ) {
			switch ( role ) {
			case AccountRole.Owner:
				if ( !Owner.IsNull() && !Owner.KeyAuths.IsNull() ) {
					foreach ( var keyAuth in Owner.KeyAuths ) {
						if ( keyAuth.IsEquelKey( key ) ) {
							Unity.Console.Log( Unity.Console.SetGreenColor( "Owner->", key.Public, "\nOwner<-", keyAuth.PublicKey ) );
							return true;
						}
						Unity.Console.Log( Unity.Console.SetRedColor( "Owner->", key.Public, "\nOwner<-", keyAuth.PublicKey ) );
					}
				}
				return false;
			case AccountRole.Active:
				if ( !Active.IsNull() && !Active.KeyAuths.IsNull() ) {
					foreach ( var keyAuth in Active.KeyAuths ) {
						if ( keyAuth.IsEquelKey( key ) ) {
							Unity.Console.Log( Unity.Console.SetGreenColor( "Active->", key.Public, "\nActive<-", keyAuth.PublicKey ) );
							return true;
						}
						Unity.Console.Log( Unity.Console.SetRedColor( "Active->", key.Public, "\nActive<-", keyAuth.PublicKey ) );
					}
				}
				return false;
			case AccountRole.Memo:
				if ( !Options.IsNull() ) {
					if ( Options.IsEquelKey( key ) ) {
						Unity.Console.Log( Unity.Console.SetGreenColor( "Memo->", key.Public, "\nMemo<-", Options.MemoKey ) );
						return true;
					}
					Unity.Console.Log( Unity.Console.SetRedColor( "Memo->", key.Public, "\nMemo<-", Options.MemoKey ) );
				}
				return false;
			default:
				return false;
			}
		}
	}


	public sealed class FullAccountData : NullableObject {

		[JsonProperty( "account" )]
		public AccountObject Account { get; set; }
		[JsonProperty( "statistics" )]
		public AccountStatisticsObject Statistics { get; set; }
		[JsonProperty( "registrar_name" )]
		public string RegistrarName { get; set; }
		[JsonProperty( "referrer_name" )]
		public string ReferrerName { get; set; }
		[JsonProperty( "lifetime_referrer_name" )]
		public string LifetimeReferrerName { get; set; }
		[JsonProperty( "votes" )]
		public object[] Votes { get; set; }                         // todo
		[JsonProperty( "cashback_balance", NullValueHandling = NullValueHandling.Ignore )]
		public object CashbackBalance { get; set; }					// todo
		[JsonProperty( "balances" )]
		public AccountBalanceObject[] Balances { get; set; }
		[JsonProperty( "vesting_balances" )]
		public object[] VestingBalances { get; set; }               // todo
		[JsonProperty( "limit_orders" )]
		public object[] LimitOrders { get; set; }                   // todo
		[JsonProperty( "call_orders" )]
		public object[] CallOrders { get; set; }                    // todo
		[JsonProperty( "proposals" )]
		public object[] Proposals { get; set; }                     // todo
		[JsonProperty( "pending_dividend_payments" )]
		public object[] PendingDividendPayments { get; set; }       // todo
	}
}