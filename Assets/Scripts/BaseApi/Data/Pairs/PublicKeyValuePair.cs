using Base.Data.Json;
using Base.ECC;
using Newtonsoft.Json;


namespace Base.Data.Pairs {

	[JsonConverter( typeof( PublicKeyValuePairConverter ) )]
	public class PublicKeyValuePair {

		public PublicKey PublicKey { get; private set; }
		public ushort Value { get; private set; }

		public PublicKeyValuePair( PublicKey publicKey, ushort value ) {
			PublicKey = publicKey;
			Value = value;
		}

		public bool IsEquelKey( KeyPair key ) {
			return key.Equals( PublicKey );
		}
	}
}