using Base.Data.Json;
using Newtonsoft.Json;


namespace Base.Data.Pairs {

	[JsonConverter( typeof( PayerAmountPairConverter ) )]
	public class PayerAmountPair {

		public SpaceTypeId Payer { get; private set; }
		public long Amount { get; private set; }

		public PayerAmountPair( SpaceTypeId payer, long amount ) {
			Payer = payer;
			Amount = amount;
		}
	}
}