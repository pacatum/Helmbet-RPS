using Buffers;
using Base.Config;
using Base.Data.Json;
using Newtonsoft.Json;
using Tools;


namespace Base.Data.Operations {

	[JsonConverter( typeof( OperationResultDataPairConverter ) )]
	public abstract class OperationResultData : NullableObject, ISerializeToBuffer {

		public abstract object Value { get; }
		public abstract ChainTypes.OperationResult Type { get; }
		public abstract ByteBuffer ToBufferRaw( ByteBuffer buffer = null );

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			return ToBufferRaw( buffer );
		}

		public OperationResultData Clone() {
			return ( OperationResultData )JsonConvert.DeserializeObject( Serialize(), GetType() );
		}
	}
}