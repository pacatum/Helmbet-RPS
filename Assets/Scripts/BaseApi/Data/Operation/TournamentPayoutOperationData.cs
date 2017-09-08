using Buffers;
using Base.Config;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Operations {

	public sealed class TournamentPayoutOperationData : OperationData {

		const string FEE_FIELD_KEY = "fee";
		const string PAYOUT_ACCOUNT_ID_FIELD_KEY = "payout_account_id";
		const string TOURNAMENT_ID_FIELD_KEY = "tournament_id";
		const string PAYOUT_AMOUNT_FIELD_KEY = "payout_amount";
		const string TYPE_FIELD_KEY = "type";
		const string EXTENSIONS_FIELD_KEY = "extensions";

		public override AssetData Fee { get; set; }
		public SpaceTypeId Payout { get; set; }
		public SpaceTypeId Tournament { get; set; }
		public AssetData PayoutAmount { get; set; }
		public ChainTypes.PayoutType PayoutType { get; set; }
		public object[] Extensions { get; set; }

		public override ChainTypes.Operation Type {
			get { return ChainTypes.Operation.TournamentPayout; }
		}

		public TournamentPayoutOperationData() {
			Extensions = new object[ 0 ];
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			Fee.ToBuffer( buffer );
			Payout.ToBuffer( buffer );
			Tournament.ToBuffer( buffer );
			PayoutAmount.ToBuffer( buffer );
			buffer.WriteEnum( ( int )PayoutType );
			buffer.WriteArray( Extensions, ( b, item ) => {
				if ( !item.IsNull() ) {
					;
				}
			} ); // todo
			return buffer;
		}

		public override string Serialize() {
			return new JSONBuilder( new JSONDictionary {
				{ FEE_FIELD_KEY,                    Fee },
				{ PAYOUT_ACCOUNT_ID_FIELD_KEY,      Payout },
				{ TOURNAMENT_ID_FIELD_KEY,          Tournament },
				{ PAYOUT_AMOUNT_FIELD_KEY,          PayoutAmount },
				{ TYPE_FIELD_KEY,                   PayoutType },
				{ EXTENSIONS_FIELD_KEY,             Extensions }
			} ).Build();
		}

		public static TournamentPayoutOperationData Create( JObject value ) {
			var token = value.Root;
			var instance = new TournamentPayoutOperationData();
			instance.Fee = value.TryGetValue( FEE_FIELD_KEY, out token ) ? token.ToObject<AssetData>() : AssetData.EMPTY;
			instance.Payout = value.TryGetValue( PAYOUT_ACCOUNT_ID_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.Tournament = value.TryGetValue( TOURNAMENT_ID_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.PayoutAmount = value.TryGetValue( PAYOUT_AMOUNT_FIELD_KEY, out token ) ? token.ToObject<AssetData>() : AssetData.EMPTY;
			instance.PayoutType = value.TryGetValue( TYPE_FIELD_KEY, out token ) ? token.ToObject<ChainTypes.PayoutType>() : ( ChainTypes.PayoutType )(-1);
			instance.Extensions = value.TryGetValue( EXTENSIONS_FIELD_KEY, out token ) ? token.ToObject<object[]>() : new object[ 0 ];
			return instance;
		}
	}
}