using System;
using Base.Data.Json;
using Newtonsoft.Json;


namespace Base.Data.Properties {

	// id "2.1.x"
	public sealed class DynamicGlobalPropertiesObject : IdObject {

		[JsonProperty( "random" )]
		public string Random { get; private set; }
		[JsonProperty( "head_block_number" )]
		public uint HeadBlockNumber { get; private set; }
		[JsonProperty( "head_block_id" )]
		public string HeadBlockId { get; private set; }
		[JsonProperty( "time" ), JsonConverter( typeof( DateTimeConverter ) )]
		public DateTime Time { get; private set; }
		[JsonProperty( "current_witness" )]
		public SpaceTypeId CurrentWitness { get; private set; }
		[JsonProperty( "next_maintenance_time" ), JsonConverter( typeof( DateTimeConverter ) )]
		public DateTime NextMaintenanceTime { get; private set; }
		[JsonProperty( "last_budget_time" ), JsonConverter( typeof( DateTimeConverter ) )]
		public DateTime LastBudgetTime { get; private set; }
		[JsonProperty( "witness_budget" )]
		public long WitnessBudget { get; private set; }
		[JsonProperty( "accounts_registered_this_interval" )]
		public uint AccountsRegisteredThisInterval { get; private set; }
		[JsonProperty( "recently_missed_count" )]
		public uint RecentlyMissedCount { get; private set; }
		[JsonProperty( "current_aslot" )]
		public ulong CurrentAslot { get; private set; }
		[JsonProperty( "recent_slots_filled" )]
		public string RecentSlotsFilled { get; private set; }
		[JsonProperty( "dynamic_flags" )]
		public uint DynamicFlags { get; private set; }
		[JsonProperty( "last_irreversible_block_num" )]
		public uint LastIrreversibleBlockNum { get; private set; }
	}
}