using Base.Data.Json;
using Newtonsoft.Json;


namespace Base.Data.Pairs {

	[JsonConverter( typeof( UserNameAccountIdPairConverter ) )]
	public class UserNameAccountIdPair {

		public string UserName { get; private set; }
		public SpaceTypeId Id { get; private set; }

		public UserNameAccountIdPair( string userName, SpaceTypeId id ) {
			UserName = userName;
			Id = id;
		}
	}
}