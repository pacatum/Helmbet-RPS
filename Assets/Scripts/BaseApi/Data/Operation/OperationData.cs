using Buffers;
using Base.Config;
using Base.Data.Json;
using Newtonsoft.Json;
using Tools;


namespace Base.Data.Operations {

	[JsonConverter( typeof( OperationDataPairConverter ) )]
	public abstract class OperationData : NullableObject, ISerializeToBuffer {

		public abstract AssetData Fee { get; set; }
		public abstract ChainTypes.Operation Type { get; }
		public abstract ByteBuffer ToBufferRaw( ByteBuffer buffer = null );

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteVarInt32( ( int )Type );
			return ToBufferRaw( buffer );
		}

		public OperationData Clone() {
			return ( OperationData )JsonConvert.DeserializeObject( JsonConvert.SerializeObject( this, new OperationDataPairConverter() ), GetType() );
		}

		public T Clone<T>() where T : OperationData {
			return JsonConvert.DeserializeObject<T>( JsonConvert.SerializeObject( this ) );
		}
	}
}