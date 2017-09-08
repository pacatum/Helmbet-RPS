using Buffers;
using Base.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Base.Data.Operations {

	public sealed class VoidOperationResultData : OperationResultData {

		object value;


		public override object Value {
			get { return value; }
		}

		public override ChainTypes.OperationResult Type {
			get { return ChainTypes.OperationResult.Void; }
		}

		public VoidOperationResultData() {
			value = new object();
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			return buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
		}

		public override string Serialize() {
			return JsonConvert.SerializeObject( value );
		}

		public static VoidOperationResultData Create( JToken value ) {
			var instance = new VoidOperationResultData();
			instance.value = value.ToObject<object>();
			return instance;
		}
	}
}