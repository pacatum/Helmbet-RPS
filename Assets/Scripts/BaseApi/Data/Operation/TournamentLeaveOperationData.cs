using Buffers;
using Base.Config;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Operations {

	public sealed class TournamentLeaveOperationData : OperationData {

		const string FEE_FIELD_KEY = "fee";
		const string CANCELING_ACCOUNT_ID_FIELD_KEY = "canceling_account_id";
		const string PLAYER_ACCOUNT_ID_FIELD_KEY = "player_account_id";
		const string TOURNAMENT_ID_FIELD_KEY = "tournament_id";
		const string EXTENSIONS_FIELD_KEY = "extensions";

		public override AssetData Fee { get; set; }
		public SpaceTypeId Canceling { get; set; }
		public SpaceTypeId Player { get; set; }
		public SpaceTypeId Tournament { get; set; }
		public object[] Extensions { get; set; }

		public override ChainTypes.Operation Type {
			get { return ChainTypes.Operation.TournamentLeave; }
		}

		public TournamentLeaveOperationData() {
			Extensions = new object[ 0 ];
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			Fee.ToBuffer( buffer );
			Canceling.ToBuffer( buffer );
			Player.ToBuffer( buffer );
			Tournament.ToBuffer( buffer );
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
				{ CANCELING_ACCOUNT_ID_FIELD_KEY,   Canceling },
				{ PLAYER_ACCOUNT_ID_FIELD_KEY,      Player },
				{ TOURNAMENT_ID_FIELD_KEY,          Tournament },
				{ EXTENSIONS_FIELD_KEY,             Extensions }
			} ).Build();
		}

		public static TournamentLeaveOperationData Create( JObject value ) {
			var token = value.Root;
			var instance = new TournamentLeaveOperationData();
			instance.Fee = value.TryGetValue( FEE_FIELD_KEY, out token ) ? token.ToObject<AssetData>() : AssetData.EMPTY;
			instance.Canceling = value.TryGetValue( CANCELING_ACCOUNT_ID_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.Player = value.TryGetValue( PLAYER_ACCOUNT_ID_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.Tournament = value.TryGetValue( TOURNAMENT_ID_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.Extensions = value.TryGetValue( EXTENSIONS_FIELD_KEY, out token ) ? token.ToObject<object[]>() : new object[ 0 ];
			return instance;
		}
	}
}