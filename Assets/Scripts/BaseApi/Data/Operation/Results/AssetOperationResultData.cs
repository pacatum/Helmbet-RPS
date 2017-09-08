using Buffers;
using Base.Config;
using Newtonsoft.Json.Linq;


namespace Base.Data.Operations {

	public sealed class AssetOperationResultData : OperationResultData {

		AssetData value;


		public override object Value {
			get { return value; }
		}

		public override ChainTypes.OperationResult Type {
			get { return ChainTypes.OperationResult.Asset; }
		}

		public AssetOperationResultData() {
			value = AssetData.EMPTY;
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			return value.ToBuffer( buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING ) );
		}

		public override string Serialize() {
			return value.Serialize();
		}

		public static AssetOperationResultData Create( JToken value ) {
			var instance = new AssetOperationResultData();
			instance.value = value.ToObject<AssetData>();
			return instance;
		}
	}
}