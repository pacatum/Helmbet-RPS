using Base.Data.Pairs;
using Newtonsoft.Json;


namespace Base.Data.Tournaments {

	// id "1.17.x"
	public sealed class TournamentDetailsObject : IdObject {

		[JsonProperty( "tournament_id" )]
		public SpaceTypeId Tournament { get; private set; }
		[JsonProperty( "registered_players" )]
		public SpaceTypeId[] RegisteredPlayers { get; private set; }
		[JsonProperty( "payers" )]
		public PayerAmountPair[] Payers { get; private set; }
		[JsonProperty( "players_payers" )]
		public PlayerPayerPair[] PlayersPayers { get; private set; }
		[JsonProperty( "matches" )]
		public SpaceTypeId[] Matches { get; private set; }
	}
}