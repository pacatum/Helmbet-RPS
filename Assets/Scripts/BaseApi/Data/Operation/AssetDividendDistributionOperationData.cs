using Buffers;
using Base.Config;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Operations {

	public sealed class AssetDividendDistributionOperationData : OperationData {

		const string FEE_FIELD_KEY = "fee";
		const string DIVIDEND_ASSET_FIELD_KEY = "dividend_asset_id";
		const string ACCOUNT_FIELD_KEY = "account_id";
		const string AMOUNTS_FIELD_KEY = "amount";
		const string EXTENSIONS_FIELD_KEY = "extensions";

		public override AssetData Fee { get; set; }
		public SpaceTypeId DividendAsset { get; private set; }
		public SpaceTypeId Account { get; private set; }
		public AssetData[] Amounts { get; private set; }
		public object[] Extensions { get; private set; }

		public override ChainTypes.Operation Type {
			get { return ChainTypes.Operation.AssetDividendDistribution; }
		}

		public AssetDividendDistributionOperationData() {
			Extensions = new object[ 0 ];
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			Fee.ToBuffer( buffer );
			DividendAsset.ToBuffer( buffer );
			Account.ToBuffer( buffer );
			buffer.WriteArray( Amounts, ( b, item ) => {
				if ( !item.IsNull() ) {
					item.ToBuffer( b );
				}
			} );
			buffer.WriteArray( Extensions, ( b, item ) => {
				if ( !item.IsNull() ) {
					;
				}
			} ); // todo
			return buffer;
		}

		public override string Serialize() {
			var builder = new JSONBuilder();
			builder.WriteKeyValuePair( FEE_FIELD_KEY, Fee );
			builder.WriteKeyValuePair( DIVIDEND_ASSET_FIELD_KEY, DividendAsset );
			builder.WriteKeyValuePair( ACCOUNT_FIELD_KEY, Account );
			builder.WriteKeyValuePair( AMOUNTS_FIELD_KEY, Amounts );
			builder.WriteKeyValuePair( EXTENSIONS_FIELD_KEY, Extensions );
			return builder.Build();
		}

		public static AssetDividendDistributionOperationData Create( JObject value ) {
			var token = value.Root;
			var instance = new AssetDividendDistributionOperationData();
			instance.Fee = value.TryGetValue( FEE_FIELD_KEY, out token ) ? token.ToObject<AssetData>() : AssetData.EMPTY;
			instance.DividendAsset = value.TryGetValue( DIVIDEND_ASSET_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.Account = value.TryGetValue( ACCOUNT_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.Amounts = value.TryGetValue( AMOUNTS_FIELD_KEY, out token ) ? token.ToObject<AssetData[]>() : new AssetData[ 0 ];
			instance.Extensions = value.TryGetValue( EXTENSIONS_FIELD_KEY, out token ) ? token.ToObject<object[]>() : new object[ 0 ];
			return instance;
		}
	}
}