using System;
using Base.Config;


namespace Base.Data.Json {

	public sealed class PayoutTypeEnumConverter : JsonCustomConverter<ChainTypes.PayoutType, string> {

		const string PRIZE_AWARD = "prize_award";
		const string BUY_IN_REFUND = "buyin_refund";
		const string RAKE_FEE = "rake_fee";


		protected override ChainTypes.PayoutType Deserialize( string value, Type objectType ) {
			return ConvertFrom( value );
		}

		protected override string Serialize( ChainTypes.PayoutType value ) {
			return ConvertTo( value );
		}

		public static string ConvertTo( ChainTypes.PayoutType state ) {
			switch ( state ) {
			case ChainTypes.PayoutType.PrizeAward:
				return PRIZE_AWARD;
			case ChainTypes.PayoutType.BuyInRefund:
				return BUY_IN_REFUND;
			case ChainTypes.PayoutType.RakeFee:
				return RAKE_FEE;
			}
			throw new ArgumentException( "Unexpected value: " + state );
		}

		public static ChainTypes.PayoutType ConvertFrom( string state ) {
			switch ( state ) {
			case PRIZE_AWARD:
				return ChainTypes.PayoutType.PrizeAward;
			case BUY_IN_REFUND:
				return ChainTypes.PayoutType.BuyInRefund;
			case RAKE_FEE:
				return ChainTypes.PayoutType.RakeFee;
			}
			throw new ArgumentException( "Unexpected value: " + state );
		}
	}
}