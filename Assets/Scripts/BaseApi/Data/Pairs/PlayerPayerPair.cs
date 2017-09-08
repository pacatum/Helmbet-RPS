using Base.Data.Json;
using Newtonsoft.Json;


namespace Base.Data.Pairs {

	[JsonConverter( typeof( PlayerPayerPairConverter ) )]
	public class PlayerPayerPair {

		public SpaceTypeId Player { get; private set; }
		public SpaceTypeId Payer { get; private set; }

		public PlayerPayerPair( SpaceTypeId player, SpaceTypeId payer ) {
			Player = player;
			Payer = payer;
		}
	}
}