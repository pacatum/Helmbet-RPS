using Base.Data.Operations.Fee;
using Newtonsoft.Json;
using Tools;


namespace Base.Data.Properties {

	public sealed class ChainParametersData : NullableObject {

		[JsonProperty( "current_fees" )]
		public FeeScheduleData CurrentFees { get; private set; }
		[JsonProperty( "block_interval" )]
		public byte BlockInterval { get; private set; }
		[JsonProperty( "maintenance_interval" )]
		public uint MaintenanceInterval { get; private set; }
		[JsonProperty( "maintenance_skip_slots" )]
		public byte MaintenanceSkipSlots { get; private set; }
		[JsonProperty( "committee_proposal_review_period" )]
		public uint CommitteeProposalReviewPeriod { get; private set; }
		[JsonProperty( "maximum_transaction_size" )]
		public uint MaximumTransactionSize { get; private set; }
		[JsonProperty( "maximum_block_size" )]
		public uint MaximumBlockSize { get; private set; }
		[JsonProperty( "maximum_time_until_expiration" )]
		public uint MaximumTimeUntilExpiration { get; private set; }
		[JsonProperty( "maximum_proposal_lifetime" )]
		public uint MaximumProposalLifetime { get; private set; }
		[JsonProperty( "maximum_asset_whitelist_authorities" )]
		public byte MaximumAssetWhitelistAuthorities { get; private set; }
		[JsonProperty( "maximum_asset_feed_publishers" )]
		public byte MaximumAssetFeedPublishers { get; private set; }
		[JsonProperty( "maximum_witness_count" )]
		public ushort MaximumWitnessCount { get; private set; }
		[JsonProperty( "maximum_committee_count" )]
		public ushort MaximumCommitteeCount { get; private set; }
		[JsonProperty( "maximum_authority_membership" )]
		public ushort MaximumAuthorityMembership { get; private set; }
		[JsonProperty( "reserve_percent_of_fee" )]
		public ushort ReservePercentOfFee { get; private set; }
		[JsonProperty( "network_percent_of_fee" )]
		public ushort NetworkPercentOfFee { get; private set; }
		[JsonProperty( "lifetime_referrer_percent_of_fee" )]
		public ushort LifetimeReferrerPercentOfFee { get; private set; }
		[JsonProperty( "cashback_vesting_period_seconds" )]
		public uint CashbackVestingPeriodSeconds { get; private set; }
		[JsonProperty( "cashback_vesting_threshold" )]
		public long CashbackVestingThreshold { get; private set; }
		[JsonProperty( "count_non_member_votes" )]
		public bool CountNonMemberVotes { get; private set; }
		[JsonProperty( "allow_non_member_whitelists" )]
		public bool AllowNonMemberWhitelists { get; private set; }
		[JsonProperty( "witness_pay_per_block" )]
		public long WitnessPayPerBlock { get; private set; }
		[JsonProperty( "witness_pay_vesting_seconds" )]
		public uint WitnessPayVestingSeconds { get; private set; }
		[JsonProperty( "worker_budget_per_day" )]
		public long WorkerBudgetPerDay { get; private set; }
		[JsonProperty( "max_predicate_opcode" )]
		public ushort MaxPredicateOpcode { get; private set; }
		[JsonProperty( "fee_liquidation_threshold" )]
		public long FeeLiquidationThreshold { get; private set; }
		[JsonProperty( "accounts_per_fee_scale" )]
		public ushort AccountsPerFeeScale { get; private set; }
		[JsonProperty( "account_fee_scale_bitshifts" )]
		public byte AccountFeeScaleBitshifts { get; private set; }
		[JsonProperty( "max_authority_depth" )]
		public byte MaxAuthorityDepth { get; private set; }
		[JsonProperty( "witness_schedule_algorithm" )]
		public byte WitnessScheduleAlgorithm { get; private set; }
		[JsonProperty( "min_round_delay" )]
		public uint MinRoundDelay { get; private set; }
		[JsonProperty( "max_round_delay" )]
		public uint MaxRoundDelay { get; private set; }
		[JsonProperty( "min_time_per_commit_move" )]
		public uint MinTimePerCommitMove { get; private set; }
		[JsonProperty( "max_time_per_commit_move" )]
		public uint MaxTimePerCommitMove { get; private set; }
		[JsonProperty( "min_time_per_reveal_move" )]
		public uint MinTimePerRevealMove { get; private set; }
		[JsonProperty( "max_time_per_reveal_move" )]
		public uint MaxTimePerRevealMove { get; private set; }
		[JsonProperty( "rake_fee_percentage" )]
		public ushort RakeFeePercentage { get; private set; }
		[JsonProperty( "maximum_registration_deadline" )]
		public uint MaximumRegistrationDeadline { get; private set; }
		[JsonProperty( "maximum_players_in_tournament" )]
		public ushort MaximumPlayersInTournament { get; private set; }
		[JsonProperty( "maximum_tournament_whitelist_length" )]
		public ushort MaximumTournamentWhitelistLength { get; private set; }
		[JsonProperty( "maximum_tournament_start_time_in_future" )]
		public uint MaximumTournamentStartTimeInFuture { get; private set; }
		[JsonProperty( "maximum_tournament_start_delay" )]
		public uint MaximumTournamentStartDelay { get; private set; }
		[JsonProperty( "maximum_tournament_number_of_wins" )]
		public ushort MaximumTournamentNumberOfWins { get; private set; }
		[JsonProperty( "extensions" )]
		public object[] Extensions { get; private set; }
	}
}