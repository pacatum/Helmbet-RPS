using System;
using Buffers;
using Base.Config;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Operations.Fee {

	public sealed class AccountCreateOperationFeeParametersData : FeeParametersData {

		const string BASIC_FEE_FIELD_KEY = "basic_fee";
		const string PREMIUM_FEE_FIELD_KEY = "premium_fee";
		const string PRICE_PER_KBYTE_FIELD_KEY = "price_per_kbyte";

		public ulong BasicFee { get; set; }
		public ulong PremiumFee { get; set; }
		public uint PricePerKByte { get; set; }

		public override ChainTypes.FeeParameters Type {
			get { return ChainTypes.FeeParameters.AccountCreateOperation; }
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteUInt64( BasicFee );
			buffer.WriteUInt64( PremiumFee );
			buffer.WriteUInt32( PricePerKByte );
			return buffer;
		}

		public override string Serialize() {
			return new JSONBuilder( new JSONDictionary {
				{ BASIC_FEE_FIELD_KEY,         	BasicFee },
				{ PREMIUM_FEE_FIELD_KEY,      	PremiumFee },
				{ PRICE_PER_KBYTE_FIELD_KEY,    PricePerKByte }
			} ).Build();
		}

		public static AccountCreateOperationFeeParametersData Create( JObject value ) {
			var token = value.Root;
			var instance = new AccountCreateOperationFeeParametersData();
			instance.BasicFee = Convert.ToUInt64( value.TryGetValue( BASIC_FEE_FIELD_KEY, out token ) ? token.ToObject<object>() : 0 );
			instance.PremiumFee = Convert.ToUInt64( value.TryGetValue( PREMIUM_FEE_FIELD_KEY, out token ) ? token.ToObject<object>() : 0 );
			instance.PricePerKByte = value.TryGetValue( PRICE_PER_KBYTE_FIELD_KEY, out token ) ? token.ToObject<uint>() : uint.MinValue;
			return instance;
		}
	}
}