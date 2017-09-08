using System;
using Base.Data.Json;
using Base.Data.Transactions;
using Newtonsoft.Json;
using Tools;


namespace Base.Data.Block {

	public class BlockHeaderData : NullableObject {

		[JsonProperty( "previous" )]
		public string Previous { get; private set; }
		[JsonProperty( "timestamp" ), JsonConverter( typeof( DateTimeConverter ) )]
		public DateTime Timestamp { get; private set; }
		[JsonProperty( "witness" )]
		public SpaceTypeId Witness { get; private set; }
		[JsonProperty( "next_secret_hash" )]
		public string NextSecretHash { get; private set; }
		[JsonProperty( "previous_secret" )]
		public string PreviousSecret { get; private set; }
		[JsonProperty( "transaction_merkle_root" )]
		public string TransactionMerkleRoot { get; private set; }
		[JsonProperty( "extensions" )]
		public object[] Extensions { get; private set; }           // todo
	}


	public class SignedBlockHeaderData : BlockHeaderData {

		[JsonProperty( "witness_signature" )]
		public string WitnessSignature { get; private set; }
	}


	public sealed class SignedBlockData : SignedBlockHeaderData {

		[JsonProperty( "transactions" )]
		public ProcessedTransactionData[] Transactions { get; private set; }
	}
}