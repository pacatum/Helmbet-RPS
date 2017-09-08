using Buffers;
using Base.Config;
using Base.Data.Json;
using Newtonsoft.Json;
using Tools;


namespace Base.Data.Tournaments.GameMoves {

	[JsonConverter( typeof( GameSpecificMovesDataPairConverter ) )]
	public abstract class GameSpecificMovesData : NullableObject, ISerializeToBuffer {

		public abstract ChainTypes.GameSpecificMoves Type { get; }
		public abstract ByteBuffer ToBufferRaw( ByteBuffer buffer = null );

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteVarInt32( ( int )Type );
			return ToBufferRaw( buffer );
		}

		public GameSpecificMovesData Clone() {
			return ( GameSpecificMovesData )JsonConvert.DeserializeObject( JsonConvert.SerializeObject( this, new GameSpecificMovesDataPairConverter() ), GetType() );
		}
	}
}