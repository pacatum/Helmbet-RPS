using Base.Data.Json;
using Newtonsoft.Json;


namespace Base.Data.Pairs {

	[JsonConverter( typeof( AccountIdValuePairConverter ) )]
	public class AccountIdValuePair {

		public SpaceTypeId Account { get; private set; }
		public ushort Value { get; private set; }

		public AccountIdValuePair( SpaceTypeId account, ushort value ) {
			Account = account;
			Value = value;
		}
	}
}