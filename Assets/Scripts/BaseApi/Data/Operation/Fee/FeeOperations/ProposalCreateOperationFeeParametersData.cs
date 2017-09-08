using System;
using Buffers;
using Base.Config;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Operations.Fee {

	public sealed class ProposalCreateOperationFeeParametersData : FeeParametersData {

		const string FEE_FIELD_KEY = "fee";
		const string PRICE_PER_KBYTE_FIELD_KEY = "price_per_kbyte";

		public ulong Fee { get; set; }
		public uint PricePerKByte { get; set; }

		public override ChainTypes.FeeParameters Type {
			get { return ChainTypes.FeeParameters.ProposalCreateOperation; }
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteUInt64( Fee );
			buffer.WriteUInt32( PricePerKByte );
			return buffer;
		}

		public override string Serialize() {
			return new JSONBuilder( new JSONDictionary {
				{ FEE_FIELD_KEY,                Fee },
				{ PRICE_PER_KBYTE_FIELD_KEY,    PricePerKByte }
			} ).Build();
		}

		public static ProposalCreateOperationFeeParametersData Create( JObject value ) {
			var token = value.Root;
			var instance = new ProposalCreateOperationFeeParametersData();
			instance.Fee = Convert.ToUInt64( value.TryGetValue( FEE_FIELD_KEY, out token ) ? token.ToObject<object>() : 0 );
			instance.PricePerKByte = value.TryGetValue( PRICE_PER_KBYTE_FIELD_KEY, out token ) ? token.ToObject<uint>() : uint.MinValue;
			return instance;
		}
	}
}