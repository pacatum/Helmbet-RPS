using Newtonsoft.Json;


namespace Base.Data.Operations {

	// id "1.11.x"
	public sealed class OperationHistoryObject : IdObject {

		[JsonProperty( "op" )]
		public OperationData Operation { get; private set; }
		[JsonProperty( "result" )]
		public OperationResultData Result { get; private set; }
		[JsonProperty( "block_num" )]
		public uint BlockNumber { get; private set; }
		[JsonProperty( "trx_in_block" )]
		public ushort TransactionInBlock { get; private set; }
		[JsonProperty( "op_in_trx" )]
		public ushort OperationInTransaction { get; private set; }
		[JsonProperty( "virtual_op" )]
		public ushort VirtualOperation { get; private set; }
	}
}