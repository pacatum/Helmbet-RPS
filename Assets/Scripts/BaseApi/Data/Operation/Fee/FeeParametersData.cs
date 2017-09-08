using Buffers;
using Base.Config;
using Base.Data.Json;
using Newtonsoft.Json;
using Tools;


namespace Base.Data.Operations.Fee {

	[JsonConverter( typeof( FeeParametersDataPairConverter ) )]
	public abstract class FeeParametersData : NullableObject, ISerializeToBuffer {

		public abstract ChainTypes.FeeParameters Type { get; }
		public abstract ByteBuffer ToBufferRaw( ByteBuffer buffer = null );

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteVarInt32( ( int )Type );
			return ToBufferRaw( buffer );
		}

		public FeeParametersData Clone() {
			return ( FeeParametersData )JsonConvert.DeserializeObject( JsonConvert.SerializeObject( this, new FeeParametersDataPairConverter() ), GetType() );
		}
	}
}