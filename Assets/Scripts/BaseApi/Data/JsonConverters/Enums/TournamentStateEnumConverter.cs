using System;
using Base.Config;


namespace Base.Data.Json {

	public sealed class TournamentStateEnumConverter : JsonCustomConverter<ChainTypes.TournamentState, string> {

		const string ACCEPTING_REGISTRATIONS = "accepting_registrations";
		const string AWAITING_START = "awaiting_start";
		const string IN_PROGRESS = "in_progress";
		const string REGISTRATION_PERIOD_EXPIRED = "registration_period_expired";
		const string CONCLUDED = "concluded";


		protected override ChainTypes.TournamentState Deserialize( string value, Type objectType ) {
			return ConvertFrom( value );
		}

		protected override string Serialize( ChainTypes.TournamentState value ) {
			return ConvertTo( value );
		}

		public static string ConvertTo( ChainTypes.TournamentState state ) {
			switch ( state ) {
			case ChainTypes.TournamentState.AcceptingRegistrations:
				return ACCEPTING_REGISTRATIONS;
			case ChainTypes.TournamentState.AwaitingStart:
				return AWAITING_START;
			case ChainTypes.TournamentState.InProgress:
				return IN_PROGRESS;
			case ChainTypes.TournamentState.RegistrationPeriodExpired:
				return REGISTRATION_PERIOD_EXPIRED;
			case ChainTypes.TournamentState.Concluded:
				return CONCLUDED;
			}
			throw new ArgumentException( "Unexpected value: " + state );
		}

		public static ChainTypes.TournamentState ConvertFrom( string state ) {
			switch ( state ) {
			case ACCEPTING_REGISTRATIONS:
				return ChainTypes.TournamentState.AcceptingRegistrations;
			case AWAITING_START:
				return ChainTypes.TournamentState.AwaitingStart;
			case IN_PROGRESS:
				return ChainTypes.TournamentState.InProgress;
			case REGISTRATION_PERIOD_EXPIRED:
				return ChainTypes.TournamentState.RegistrationPeriodExpired;
			case CONCLUDED:
				return ChainTypes.TournamentState.Concluded;
			}
			throw new ArgumentException( "Unexpected value: " + state );
		}
	}
}