using System;
using Buffers;
using Base.Data.Pairs;
using Newtonsoft.Json;
using Tools;


namespace Base.Data {

	public enum AccountRole {
		
		Owner,
		Active,
		Memo
	}


	public sealed class AuthorityData : NullableObject, ISerializeToBuffer {

		[JsonProperty( "weight_threshold" )]
		public uint WeightThreshold { get; private set; }
		[JsonProperty( "account_auths" )]
		public AccountIdValuePair[] AccountAuths { get; private set; }
		[JsonProperty( "key_auths" )]
		public PublicKeyValuePair[] KeyAuths { get; private set; }
		[JsonProperty( "address_auths" )]
		public AddressValuePair[] AddressAuths { get; private set; }

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			//buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			//return buffer;
			throw new NotImplementedException();
		}
	}
}