using System;
using Buffers;
using Base.Config;
using Base.Data.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Operations {

	public sealed class ProposalCreateOperationData : OperationData {

		const string FEE_FIELD_KEY = "fee";
		const string FEE_PAYING_ACCOUNT_FIELD_KEY = "fee_paying_account";
		const string EXPIRATION_TIME_FIELD_KEY = "expiration_time";
		const string PROPOSED_OPS_FIELD_KEY = "proposed_ops";
		const string REVIEW_PERIOD_SECONDS_FIELD_KEY = "review_period_seconds";
		const string EXTENSIONS_FIELD_KEY = "extensions";

		public override AssetData Fee { get; set; }
		public SpaceTypeId FeePayingAccount { get; set; }
		public DateTime ExpirationTime { get; set; }
		public OperationWrapperData[] ProposedOperations { get; set; }
		public uint? ReviewPeriodSeconds { get; set; }
		public object[] Extensions { get; set; }

		public override ChainTypes.Operation Type {
			get { return ChainTypes.Operation.ProposalCreate; }
		}

		public ProposalCreateOperationData() {
			ProposedOperations = new OperationWrapperData[ 0 ];
			ExpirationTime = Tool.ZeroTime();
			Extensions = new object[ 0 ];
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			//buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			//return buffer;
			throw new NotImplementedException();
		}

		public override string Serialize() {
			var builder = new JSONBuilder();
			builder.WriteKeyValuePair( FEE_FIELD_KEY, Fee );
			builder.WriteKeyValuePair( FEE_PAYING_ACCOUNT_FIELD_KEY, FeePayingAccount );
			builder.WriteKeyValuePair( EXPIRATION_TIME_FIELD_KEY, DateTimeConverter.ConvertTo( ExpirationTime ) );
			builder.WriteKeyValuePair( PROPOSED_OPS_FIELD_KEY, ProposedOperations );
			builder.WriteOptionalStructKeyValuePair( REVIEW_PERIOD_SECONDS_FIELD_KEY, ReviewPeriodSeconds );
			builder.WriteKeyValuePair( EXTENSIONS_FIELD_KEY, Extensions );
			return builder.Build();
		}

		public static ProposalCreateOperationData Create( JObject value ) {
			var token = value.Root;
			var instance = new ProposalCreateOperationData();
			instance.Fee = value.TryGetValue( FEE_FIELD_KEY, out token ) ? token.ToObject<AssetData>() : AssetData.EMPTY;
			instance.FeePayingAccount = value.TryGetValue( FEE_PAYING_ACCOUNT_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.ExpirationTime = value.TryGetValue( EXPIRATION_TIME_FIELD_KEY, out token ) ? token.ToObject<DateTime>( new DateTimeConverter().GetSerializer() ) : Tool.ZeroTime();
			instance.ProposedOperations = value.TryGetValue( PROPOSED_OPS_FIELD_KEY, out token ) ? token.ToObject<OperationWrapperData[]>() : new OperationWrapperData[ 0 ];
			instance.ReviewPeriodSeconds = value.TryGetValue( REVIEW_PERIOD_SECONDS_FIELD_KEY, out token ) ? new uint?( token.ToObject<uint>() ) : null; // optional
			instance.Extensions = value.TryGetValue( EXTENSIONS_FIELD_KEY, out token ) ? token.ToObject<object[]>() : new object[ 0 ];
			return instance;
		}
	}

	
	public sealed class OperationWrapperData : NullableObject, ISerializeToBuffer {

		[JsonProperty( "op" )]
		public OperationData Operation { get; set; }

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			//buffer = buffer ?? new Buffer( Buffer.LITTLE_ENDING );
			//return buffer;
			throw new NotImplementedException();
		}
	}
}