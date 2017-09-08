using Newtonsoft.Json;


namespace Base.Data.Accounts {

	// id "2.6.x"
	public sealed class AccountStatisticsObject : IdObject {

		[JsonProperty( "owner" )]
		public SpaceTypeId Owner { get; private set; }
		[JsonProperty( "most_recent_op" )]
		public SpaceTypeId MostRecentOperation { get; private set; }
		[JsonProperty( "total_ops" )]
		public uint TotalOperations { get; private set; }
		[JsonProperty( "total_core_in_orders" )]
		public long TotalCoreInOrders { get; private set; }
		[JsonProperty( "lifetime_fees_paid" )]
		public long LifetimeFeesPaid { get; private set; }
		[JsonProperty( "pending_fees" )]
		public long PendingFees { get; private set; }
		[JsonProperty( "pending_vested_fees" )]
		public long PendingVestedFees { get; private set; }
	}
}