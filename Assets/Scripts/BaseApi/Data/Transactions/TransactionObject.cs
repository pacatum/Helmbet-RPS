using Newtonsoft.Json;


namespace Base.Data.Transactions {

	// id "2.7.x"
	public sealed class TransactionObject : IdObject {

		[JsonProperty( "trx" )]
		public SignedTransactionData Transaction { get; set; }
		[JsonProperty( "trx_id" )]
		public string TransactionId { get; set; }
	}
}