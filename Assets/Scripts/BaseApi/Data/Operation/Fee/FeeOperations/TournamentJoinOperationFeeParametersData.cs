using System;
using Buffers;
using Base.Config;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Operations.Fee {

	public sealed class TournamentJoinOperationFeeParametersData : FeeParametersData {

		const string FEE_FIELD_KEY = "fee";

		public ulong Fee { get; set; }

		public override ChainTypes.FeeParameters Type {
			get { return ChainTypes.FeeParameters.TournamentJoinOperation; }
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteUInt64( Fee );
			return buffer;
		}

		public override string Serialize() {
			return new JSONBuilder( new JSONDictionary {
				{ FEE_FIELD_KEY,                Fee }
			} ).Build();
		}

		public static TournamentJoinOperationFeeParametersData Create( JObject value ) {
			var token = value.Root;
			var instance = new TournamentJoinOperationFeeParametersData();
			instance.Fee = Convert.ToUInt64( value.TryGetValue( FEE_FIELD_KEY, out token ) ? token.ToObject<object>() : 0 );
			return instance;
		}
	}
}