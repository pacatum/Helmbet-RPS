using System;
using Base.Data.Json;
using Newtonsoft.Json;
using Base.Config;


namespace Base.Data.Tournaments {

	// id "1.18.x"
	public sealed class MatchObject : IdObject {

		[JsonProperty( "tournament_id" )]
		public SpaceTypeId Tournament { get; private set; }
		[JsonProperty( "players" )]
		public SpaceTypeId[] Players { get; private set; }
		[JsonProperty( "games" )]
		public SpaceTypeId[] Games { get; private set; }
		[JsonProperty( "game_winners" )]
		public SpaceTypeId[][] GameWinners { get; private set; }
		[JsonProperty( "number_of_wins" )]
		public uint[] NumberOfWins { get; private set; }
		[JsonProperty( "number_of_ties" )]
		public uint NumberOfTies { get; private set; }
		[JsonProperty( "match_winners" )]
		public SpaceTypeId[] MatchWinners { get; private set; }
		[JsonProperty( "start_time" ), JsonConverter( typeof( DateTimeConverter ) )]
		public DateTime StartTime { get; private set; }
		[JsonProperty( "end_time", NullValueHandling = NullValueHandling.Ignore ), JsonConverter( typeof( NullableDateTimeConverter ) )]
		public DateTime? EndTime { get; private set; }
		[JsonProperty( "state" )]
		public ChainTypes.MatchState State { get; private set; }
	}
}