using Buffers;
using Base.ECC;
using Newtonsoft.Json;
using Tools;


namespace Base.Data.Accounts {

	public sealed class AccountOptionsData : NullableObject, ISerializeToBuffer {

		[JsonProperty( "memo_key" )]
		public PublicKey MemoKey { get; private set; }
		[JsonProperty( "voting_account" )]
		public SpaceTypeId VotingAccount { get; private set; }
		[JsonProperty( "num_witness" )]
		public ushort NumWitness { get; private set; }
		[JsonProperty( "num_committee" )]
		public ushort NumCommittee { get; private set; }
		[JsonProperty( "votes" )]
		public VoteId[] Votes { get; private set; }
		[JsonProperty( "extensions" )]
		public object[] Extensions { get; private set; }        // todo

		public bool IsEquelKey( KeyPair key ) {
			return key.Equals( MemoKey );
		}

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			MemoKey.ToBuffer( buffer );
			VotingAccount.ToBuffer( buffer );
			buffer.WriteUInt16( NumWitness );
			buffer.WriteUInt16( NumCommittee );
			buffer.WriteArray( Votes, ( b, item ) => {
				if ( !item.IsNull() ) {
					item.ToBuffer( b );
				}
			}, VoteId.CompareTo );
			buffer.WriteArray( Extensions, ( b, item ) => {
				if ( !item.IsNull() ) {
					;
				}
			} ); // todo
			return buffer;
		}
	}
}