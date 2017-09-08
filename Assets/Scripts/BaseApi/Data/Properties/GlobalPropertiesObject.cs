using Newtonsoft.Json;


namespace Base.Data.Properties {

	// id "2.0.x"
	public sealed class GlobalPropertiesObject : IdObject {

		[JsonProperty( "parameters" )]
		public ChainParametersData Parameters { get; private set; }
		[JsonProperty( "pending_parameters", NullValueHandling = NullValueHandling.Ignore )]
		public ChainParametersData PendingParameters { get; private set; }
		[JsonProperty( "next_available_vote_id" )]
		public uint NextAvailableVoteId { get; private set; }
		[JsonProperty( "active_committee_members" )]
		public SpaceTypeId[] ActiveCommitteeMembers { get; private set; }
		[JsonProperty( "active_witnesses" )]
		public SpaceTypeId[] ActiveWitnesses { get; private set; }
	}
}