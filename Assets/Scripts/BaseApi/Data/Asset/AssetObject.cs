using Tools;
using Newtonsoft.Json;


namespace Base.Data.Assets {

	// id "1.3.x"
	public sealed class AssetObject : IdObject {

		[JsonProperty( "symbol" )]
		public string Symbol { get; private set; }
		[JsonProperty( "precision" )]
		public byte Precision { get; private set; }
		[JsonProperty( "issuer" )]
		public SpaceTypeId Issuer { get; private set; }
		[JsonProperty( "options" )]
		public AssetOptionsData Options { get; private set; }
		[JsonProperty( "dynamic_asset_data_id" )]
		public SpaceTypeId DynamicAssetData { get; private set; }
		[JsonProperty( "bitasset_data_id", NullValueHandling = NullValueHandling.Ignore )]
		public SpaceTypeId BitassetData { get; private set; }
		[JsonProperty( "buyback_account", NullValueHandling = NullValueHandling.Ignore )]
		public SpaceTypeId BuyBackAccount { get; private set; }
		[JsonProperty( "dividend_data_id", NullValueHandling = NullValueHandling.Ignore )]
		public SpaceTypeId DividendData { get; private set; }
	}


	public sealed class AssetOptionsData : NullableObject {

		[JsonProperty( "max_supply" )]
		public long MaxSupply { get; private set; }
		[JsonProperty( "market_fee_percent" )]
		public ushort MarketFeePercent { get; private set; }
		[JsonProperty( "max_market_fee" )]
		public long MaxMarketFee { get; private set; }
		[JsonProperty( "issuer_permissions" )]
		public ushort IssuerPermissions { get; private set; }
		[JsonProperty( "flags" )]
		public ushort Flags { get; private set; }
		[JsonProperty( "core_exchange_rate" )]
		public PriceData CoreExchangeRate { get; private set; }
		[JsonProperty( "whitelist_authorities" )]
		public SpaceTypeId[] WhitelistAuthorities { get; private set; }
		[JsonProperty( "blacklist_authorities" )]
		public SpaceTypeId[] BlacklistAuthorities { get; private set; }
		[JsonProperty( "whitelist_markets" )]
		public SpaceTypeId[] WhitelistMarkets { get; private set; }
		[JsonProperty( "blacklist_markets" )]
		public SpaceTypeId[] BlacklistMarkets { get; private set; }
		[JsonProperty( "description" )]
		public string Description { get; private set; }
		[JsonProperty( "extensions" )]
		public object[] Extensions { get; private set; }                // todo
	}


	public sealed class PriceData : NullableObject {

		[JsonProperty( "base" )]
		public AssetData Base { get; private set; }
		[JsonProperty( "quote" )]
		public AssetData Quote { get; private set; }
	}
}