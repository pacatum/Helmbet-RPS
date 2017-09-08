using Buffers;
using Base.Config;
using Base.Data.Json;
using Newtonsoft.Json;
using Tools;


namespace Base.Data.Tournaments.GameOptions {

	[JsonConverter( typeof( GameSpecificOptionsDataPairConverter ) )]
	public abstract class GameSpecificOptionsData : NullableObject, ISerializeToBuffer {

		public abstract ChainTypes.GameSpecificOptions Type { get; }
		public abstract ByteBuffer ToBufferRaw( ByteBuffer buffer = null );

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteVarInt32( ( int )Type );
			return ToBufferRaw( buffer );
		}

		public GameSpecificOptionsData Clone() {
			return ( GameSpecificOptionsData )JsonConvert.DeserializeObject( JsonConvert.SerializeObject( this, new GameSpecificOptionsDataPairConverter() ), GetType() );
		}
	}
}