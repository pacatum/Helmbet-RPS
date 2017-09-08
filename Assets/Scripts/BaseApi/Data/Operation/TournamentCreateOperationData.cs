using Buffers;
using Base.Config;
using Base.Data.Tournaments;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Operations {

	public sealed class TournamentCreateOperationData : OperationData {

		const string FEE_FIELD_KEY = "fee";
		const string CREATOR_FIELD_KEY = "creator";
		const string OPTIONS_FIELD_KEY = "options";
		const string EXTENSIONS_FIELD_KEY = "extensions";

		public override AssetData Fee { get; set; }
		public SpaceTypeId Creator { get; set; }
		public TournamentOptionsData Options { get; set; }
		public object[] Extensions { get; set; }

		public override ChainTypes.Operation Type {
			get { return ChainTypes.Operation.TournamentCreate; }
		}

		public TournamentCreateOperationData() {
			Extensions = new object[ 0 ];
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			Fee.ToBuffer( buffer );
			Creator.ToBuffer( buffer );
			Options.ToBuffer( buffer );
			buffer.WriteArray( Extensions, ( b, item ) => {
				if ( !item.IsNull() ) {
					;
				}
			} ); // todo
			return buffer;
		}

		public override string Serialize() {
			return new JSONBuilder( new JSONDictionary {
				{ FEE_FIELD_KEY,            Fee },
				{ CREATOR_FIELD_KEY,        Creator },
				{ OPTIONS_FIELD_KEY,        Options },
				{ EXTENSIONS_FIELD_KEY,     Extensions }
			} ).Build();
		}

		public static TournamentCreateOperationData Create( JObject value ) {
			var token = value.Root;
			var instance = new TournamentCreateOperationData();
			instance.Fee = value.TryGetValue( FEE_FIELD_KEY, out token ) ? token.ToObject<AssetData>() : AssetData.EMPTY;
			instance.Creator = value.TryGetValue( CREATOR_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.Options = value.TryGetValue( OPTIONS_FIELD_KEY, out token ) ? token.ToObject<TournamentOptionsData>() : null;
			instance.Extensions = value.TryGetValue( EXTENSIONS_FIELD_KEY, out token ) ? token.ToObject<object[]>() : new object[ 0 ];
			return instance;
		}
	}
}