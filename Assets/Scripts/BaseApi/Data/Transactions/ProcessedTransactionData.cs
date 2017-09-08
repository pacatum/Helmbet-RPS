using Base.Data.Operations;
using Newtonsoft.Json;
using Tools;


namespace Base.Data.Transactions {

	public sealed class ProcessedTransactionData : SignedTransactionData {

		[JsonProperty( "operation_results" )]
		public OperationResultData[] OperationResults { get; set; }

		public ProcessedTransactionData() : base() {
			OperationResults = new OperationResultData[ 0 ];
		}
	}


	public class TransactionConfirmation : NullableObject {

		[JsonProperty( "id" )]
		public string Id { get; set; }
		[JsonProperty( "block_num" )]
		public uint BlockNumber { get; set; }
		[JsonProperty( "trx_num" )]
		public uint TransactionNumber { get; set; }
		[JsonProperty( "trx" )]
		public ProcessedTransactionData Transaction { get; set; }
	}
}