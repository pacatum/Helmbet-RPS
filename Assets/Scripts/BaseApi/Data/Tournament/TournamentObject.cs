using System;
using Buffers;
using Base.Config;
using Base.Data.Json;
using Base.Data.Tournaments.GameOptions;
using Newtonsoft.Json;
using Tools;


namespace Base.Data.Tournaments {

	// id "1.16.x"
	public sealed class TournamentObject : IdObject {

		[JsonProperty( "creator" )]
		public SpaceTypeId Creator { get; private set; }
		[JsonProperty( "options" )]
		public TournamentOptionsData Options { get; private set; }
		[JsonProperty( "start_time", NullValueHandling = NullValueHandling.Ignore ), JsonConverter( typeof( NullableDateTimeConverter ) )]
		public DateTime? StartTime { get; private set; }
		[JsonProperty( "end_time", NullValueHandling = NullValueHandling.Ignore ), JsonConverter( typeof( NullableDateTimeConverter ) )]
		public DateTime? EndTime { get; private set; }
		[JsonProperty( "prize_pool" )]
		public long PrizePool { get; private set; }
		[JsonProperty( "registered_players" )]
		public uint RegisteredPlayers { get; private set; }
		[JsonProperty( "tournament_details_id" )]
		public SpaceTypeId TournamentDetails { get; private set; }
		[JsonProperty( "state" )]
		public ChainTypes.TournamentState State { get; private set; }
	}

	
	public sealed class TournamentOptionsData : NullableObject, ISerializeToBuffer {

		[JsonProperty( "registration_deadline" ), JsonConverter( typeof( DateTimeConverter ) )]
		public DateTime RegistrationDeadline { get; set; }
		[JsonProperty( "number_of_players" )]
		public uint NumberOfPlayers { get; set; }
		[JsonProperty( "buy_in" )]
		public AssetData BuyIn { get; set; }
		[JsonProperty( "whitelist" )]
		public SpaceTypeId[] Whitelist { get; set; }
		[JsonProperty( "start_time", NullValueHandling = NullValueHandling.Ignore ), JsonConverter( typeof( NullableDateTimeConverter ) )]
		public DateTime? StartTime { get; set; }
		[JsonProperty( "start_delay" )]
		public uint? StartDelay { get; set; }
		[JsonProperty( "round_delay" )]
		public uint RoundDelay { get; set; }
		[JsonProperty( "number_of_wins" )]
		public uint NumberOfWins { get; set; }
		[JsonProperty( "meta" )]
		public CustomData Meta { get; set; }
		[JsonProperty( "game_options" )]
		public GameSpecificOptionsData GameOptions { get; set; }

		public TournamentOptionsData() {
			RegistrationDeadline = Tool.ZeroTime();
			Whitelist = new SpaceTypeId[ 0 ];
		}

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteDateTime( RegistrationDeadline );
			buffer.WriteUInt32( NumberOfPlayers );
			BuyIn.ToBuffer( buffer );
			buffer.WriteArray( Whitelist, ( b, item ) => {
				if ( !item.IsNull() ) {
					item.ToBuffer( b );
				}
			}, SpaceTypeId.Compare );
			buffer.WriteOptionalStruct( StartTime, ( b, value ) => b.WriteDateTime( value ) );
			buffer.WriteOptionalStruct( StartDelay, ( b, value ) => b.WriteUInt32( value ) );
			buffer.WriteUInt32( RoundDelay );
			buffer.WriteUInt32( NumberOfWins );
			Meta.ToBuffer( buffer );
			GameOptions.ToBuffer( buffer );
			return buffer;
		}
	}
}