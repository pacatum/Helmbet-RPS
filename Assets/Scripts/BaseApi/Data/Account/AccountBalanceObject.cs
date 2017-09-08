using Newtonsoft.Json;


namespace Base.Data.Accounts {

	// id "2.5.x"
	public sealed class AccountBalanceObject : IdObject {

		[JsonProperty( "owner" )]
		public SpaceTypeId Owner { get; private set; }
		[JsonProperty( "asset_type" )]
		public SpaceTypeId AssetType { get; private set; }
		[JsonProperty( "balance" )]
		public long Balance { get; private set; }
	}
}