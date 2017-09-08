using Newtonsoft.Json;


namespace Base.Data.Assets {

	// id "2.3.x"
	public sealed class AssetDynamicDataObject : IdObject {

		[JsonProperty( "current_supply" )]
		public long CurrentSupply { get; private set; }
		[JsonProperty( "confidential_supply" )]
		public long ConfidentialSupply { get; private set; }
		[JsonProperty( "accumulated_fees" )]
		public long AccumulatedFees { get; private set; }
		[JsonProperty( "fee_pool" )]
		public long FeePool { get; private set; }
	}
}