using System;
using Buffers;
using Base.Data.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data {

	[JsonConverter( typeof( AssetDataConverter ) )]
	public sealed class AssetData : NullableObject, ISerializeToBuffer {

		public readonly static AssetData EMPTY = new AssetData( 0, SpaceTypeId.EMPTY );

		const string AMOUNT_FIELD_KEY = "amount";
		const string ASSET_ID_FIELD_KEY = "asset_id";

		public long Amount { get; private set; }
		public SpaceTypeId Asset { get; private set; }

		public AssetData( long amount, SpaceTypeId asset ) {
			Amount = amount;
			Asset = asset;
		}

		public AssetData( JObject value ) {
			var token = value.Root;
			Amount = Convert.ToInt64( value.TryGetValue( AMOUNT_FIELD_KEY, out token ) ? token.ToObject<object>() : 0 );
			Asset = value.TryGetValue( ASSET_ID_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
		}

		public override string Serialize() {
			return new JSONBuilder( new JSONDictionary {
				{ AMOUNT_FIELD_KEY,		Amount },
				{ ASSET_ID_FIELD_KEY,  	Asset }
			} ).Build();
		}

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteInt64( Amount );
			Asset.ToBuffer( buffer );
			return buffer;
		}
	}
}