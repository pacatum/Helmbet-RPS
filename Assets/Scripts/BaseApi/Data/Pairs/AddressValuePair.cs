using Base.Data.Json;
using Newtonsoft.Json;


namespace Base.Data.Pairs {

	[JsonConverter( typeof( AddressValuePairConverter ) )]
	public class AddressValuePair {

		public string Address { get; private set; }
		public ushort Value { get; private set; }

		public AddressValuePair( string address, ushort value ) {
			Address = address;
			Value = value;
		}
	}
}