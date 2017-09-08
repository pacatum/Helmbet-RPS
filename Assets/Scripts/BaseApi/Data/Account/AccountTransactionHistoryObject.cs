using Newtonsoft.Json;


namespace Base.Data.Accounts {

	// id "1.2.x"
	public sealed class AccountTransactionHistoryObject : IdObject {

		[JsonProperty( "account" )]
		public SpaceTypeId Account { get; private set; }
		[JsonProperty( "operation_id" )]
		public SpaceTypeId Operation { get; private set; }
		[JsonProperty( "sequence" )]
		public uint Sequence { get; private set; }
		[JsonProperty( "next" )]
		public SpaceTypeId Next { get; private set; }
	}
}