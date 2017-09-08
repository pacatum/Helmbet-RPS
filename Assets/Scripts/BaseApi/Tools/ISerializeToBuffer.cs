using Buffers;


namespace Base.Data {

	public interface ISerializeToBuffer {

		ByteBuffer ToBuffer( ByteBuffer buffer = null );
	}
}