using Buffers;
using Base.Config;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Operations {

	public sealed class TournamentJoinOperationData : OperationData {

		const string FEE_FIELD_KEY = "fee";
		const string PAYER_ACCOUNT_ID_FIELD_KEY = "payer_account_id";
		const string PLAYER_ACCOUNT_ID_FIELD_KEY = "player_account_id";
		const string TOURNAMENT_ID_FIELD_KEY = "tournament_id";
		const string BUY_IN_FIELD_KEY = "buy_in";
		const string EXTENSIONS_FIELD_KEY = "extensions";

		public override AssetData Fee { get; set; }
		public SpaceTypeId Payer { get; set; }
		public SpaceTypeId Player { get; set; }
		public SpaceTypeId Tournament { get; set; }
		public AssetData BuyIn { get; set; }
		public object[] Extensions { get; set; }

		public override ChainTypes.Operation Type {
			get { return ChainTypes.Operation.TournamentJoin; }
		}

		public TournamentJoinOperationData() {
			Extensions = new object[ 0 ];
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			Fee.ToBuffer( buffer );
			Payer.ToBuffer( buffer );
			Player.ToBuffer( buffer );
			Tournament.ToBuffer( buffer );
			BuyIn.ToBuffer( buffer );
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
				{ PAYER_ACCOUNT_ID_FIELD_KEY,       Payer },
				{ PLAYER_ACCOUNT_ID_FIELD_KEY,      Player },
				{ TOURNAMENT_ID_FIELD_KEY,          Tournament },
				{ BUY_IN_FIELD_KEY,                 BuyIn },
				{ EXTENSIONS_FIELD_KEY,             Extensions }
			} ).Build();
		}

		public static TournamentJoinOperationData Create( JObject value ) {
			var token = value.Root;
			var instance = new TournamentJoinOperationData();
			instance.Fee = value.TryGetValue( FEE_FIELD_KEY, out token ) ? token.ToObject<AssetData>() : AssetData.EMPTY;
			instance.Payer = value.TryGetValue( PAYER_ACCOUNT_ID_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.Player = value.TryGetValue( PLAYER_ACCOUNT_ID_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.Tournament = value.TryGetValue( TOURNAMENT_ID_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.BuyIn = value.TryGetValue( BUY_IN_FIELD_KEY, out token ) ? token.ToObject<AssetData>() : AssetData.EMPTY;
			instance.Extensions = value.TryGetValue( EXTENSIONS_FIELD_KEY, out token ) ? token.ToObject<object[]>() : new object[ 0 ];
			return instance;
		}
	}
}