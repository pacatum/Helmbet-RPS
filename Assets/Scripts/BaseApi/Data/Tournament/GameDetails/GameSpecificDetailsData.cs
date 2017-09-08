using Buffers;
using Base.Config;
using Base.Data.Json;
using Newtonsoft.Json;
using Tools;


namespace Base.Data.Tournaments.GameDetails {

	[JsonConverter( typeof( GameSpecificDetailsDataPairConverter ) )]
	public abstract class GameSpecificDetailsData : NullableObject, ISerializeToBuffer {

		public abstract ChainTypes.GameSpecificDetails Type { get; }
		public abstract ByteBuffer ToBufferRaw( ByteBuffer buffer = null );

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteVarInt32( ( int )Type );
			return ToBufferRaw( buffer );
		}

		public GameSpecificDetailsData Clone() {
			return ( GameSpecificDetailsData )JsonConvert.DeserializeObject( JsonConvert.SerializeObject( this, new GameSpecificDetailsDataPairConverter() ), GetType() );
		}
	}
}