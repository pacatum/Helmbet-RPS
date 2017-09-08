using Buffers;
using Base.Config;
using Base.Data.Tournaments.GameMoves;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Operations {

	public sealed class GameMoveOperationData : OperationData {

		const string FEE_FIELD_KEY = "fee";
		const string GAME_ID_FIELD_KEY = "game_id";
		const string PLAYER_ACCOUNT_ID_FIELD_KEY = "player_account_id";
		const string MOVE_FIELD_KEY = "move";
		const string EXTENSIONS_FIELD_KEY = "extensions";

		public override AssetData Fee { get; set; }
		public SpaceTypeId Game { get; set; }
		public SpaceTypeId Player { get; set; }
		public GameSpecificMovesData Move { get; set; }
		public object[] Extensions { get; set; }

		public override ChainTypes.Operation Type {
			get { return ChainTypes.Operation.GameMove; }
		}

		public GameMoveOperationData() {
			Extensions = new object[ 0 ];
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			Fee.ToBuffer( buffer );
			Game.ToBuffer( buffer );
			Player.ToBuffer( buffer );
			Move.ToBuffer( buffer );
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
				{ GAME_ID_FIELD_KEY,                Game },
				{ PLAYER_ACCOUNT_ID_FIELD_KEY,      Player },
				{ MOVE_FIELD_KEY,                   Move },
				{ EXTENSIONS_FIELD_KEY,             Extensions }
			} ).Build();
		}

		public static GameMoveOperationData Create( JObject value ) {
			var token = value.Root;
			var instance = new GameMoveOperationData();
			instance.Fee = value.TryGetValue( FEE_FIELD_KEY, out token ) ? token.ToObject<AssetData>() : AssetData.EMPTY;
			instance.Game = value.TryGetValue( GAME_ID_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.Player = value.TryGetValue( PLAYER_ACCOUNT_ID_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.Move = value.TryGetValue( MOVE_FIELD_KEY, out token ) ? token.ToObject<GameSpecificMovesData>() : null;
			instance.Extensions = value.TryGetValue( EXTENSIONS_FIELD_KEY, out token ) ? token.ToObject<object[]>() : new object[ 0 ];
			return instance;
		}
	}
}