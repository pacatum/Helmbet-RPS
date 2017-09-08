using Base.Data.Accounts;
using Base.Data.Json;
using Newtonsoft.Json;


namespace Base.Data.Pairs {

	[JsonConverter( typeof( UserNameFullAccountDataPairConverter ) )]
	public class UserNameFullAccountDataPair {

		public string UserName { get; private set; }
		public FullAccountData FullAccount { get; private set; }

		public UserNameFullAccountDataPair( string userName, FullAccountData fullAccount ) {
			UserName = userName;
			FullAccount = fullAccount;
		}
	}
}