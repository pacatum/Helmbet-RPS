using Buffers;
using Base.Config;
using Newtonsoft.Json.Linq;


namespace Base.Data.Operations {

	public sealed class SpaceTypeIdOperationResultData : OperationResultData {

		SpaceTypeId value;


		public override object Value {
			get { return value; }
		}

		public override ChainTypes.OperationResult Type {
			get { return ChainTypes.OperationResult.SpaceTypeId; }
		}

		public SpaceTypeIdOperationResultData() {
			value = SpaceTypeId.EMPTY;
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			return value.ToBuffer( buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING ) );
		}

		public override string Serialize() {
			return value.Serialize();
		}

		public static SpaceTypeIdOperationResultData Create( JToken value ) {
			var instance = new SpaceTypeIdOperationResultData();
			instance.value = value.ToObject<SpaceTypeId>();
			return instance;
		}
	}
}