using Base.ECC;
using Newtonsoft.Json;


namespace Base.Data.Witnesses {

	// id "1.6.x"
	public sealed class WitnessObject : IdObject {

		[JsonProperty( "witness_account" )]
		public SpaceTypeId WitnessAccountId { get; private set; }   // "1.2.8"
		[JsonProperty( "last_aslot" )]
		public ulong LastAslot { get; private set; }                // 2193468
		[JsonProperty( "signing_key" )]
		public PublicKey SigningKey { get; private set; }           // "TEST6MRyAjQq8ud7hVNYcfnVPJqcVpscN5So8BhtHuGYqET5GDW5CV"
		[JsonProperty( "next_secret_hash" )]
		public string NextSecretHash { get; private set; }          // "d7d496c9b82459f292f9b7076f8163ab89dc0023"
		[JsonProperty( "previous_secret" )]
		public string PreviousSecret { get; private set; }          // "3479382acaf9b2046827a225954b2cca1ce93de1"
		[JsonProperty( "pay_vb", NullValueHandling = NullValueHandling.Ignore )]
		public SpaceTypeId PayVestingBalance { get; private set; }
		[JsonProperty( "vote_id" )]
		public VoteId Vote { get; private set; }                    // "1:1"
		[JsonProperty( "total_votes" )]
		public ulong TotalVotes { get; private set; }               // 0
		[JsonProperty( "url" )]
		public string Url { get; private set; }                     // ""
		[JsonProperty( "total_missed" )]
		public long TotalMissed { get; private set; }               // 169689
		[JsonProperty( "last_confirmed_block_num" )]
		public uint LastConfirmedBlockNum { get; private set; }     // 326896
	}
}