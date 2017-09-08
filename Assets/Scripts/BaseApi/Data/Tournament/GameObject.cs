using System;
using Base.Config;
using Base.Data.Json;
using Base.Data.Tournaments.GameDetails;
using Newtonsoft.Json;


namespace Base.Data.Tournaments {

	// id "1.19.x"
	public sealed class GameObject : IdObject {

		[JsonProperty( "match_id" )]
		public SpaceTypeId Match { get; private set; }
		[JsonProperty( "players" )]
		public SpaceTypeId[] Players { get; private set; }
		[JsonProperty( "winners" )]
		public SpaceTypeId[] Winners { get; private set; }
		[JsonProperty( "game_details" )]
		public GameSpecificDetailsData GameDetails { get; private set; }
		[JsonProperty( "next_timeout", NullValueHandling = NullValueHandling.Ignore ), JsonConverter( typeof( NullableDateTimeConverter ) )]
		public DateTime? NextTimeout { get; private set; }
		[JsonProperty( "state" )]
		public ChainTypes.GameState State { get; private set; }
	}
}