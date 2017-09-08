using System;
using Buffers;
using Base.Data.Json;
using Base.ECC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data {

	[JsonConverter( typeof( MemoDataConverter ) )]
	public sealed class MemoData : ISerializeToBuffer {

		const string FROM_FIELD_KEY = "from";
		const string TO_FIELD_KEY = "to";
		const string NONCE_FIELD_KEY = "nonce";
		const string MESSAGE_FIELD_KEY = "message";

		public PublicKey From { get; set; }
		public PublicKey To { get; set; }
		public ulong Nonce { get; set; }
		public string Message { get; set; }

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			From.ToBuffer( buffer );
			To.ToBuffer( buffer );
			buffer.WriteUInt64( Nonce );
			buffer.WriteBytes( Tool.FromHex( Message ) );
			return buffer;
		}

		public string Serialize() {
			return new JSONBuilder( new JSONDictionary {
				{ FROM_FIELD_KEY,       From },
				{ TO_FIELD_KEY,         To },
				{ NONCE_FIELD_KEY,      Tool.WrapUInt64( Nonce ) },
				{ MESSAGE_FIELD_KEY,    Message }
			} ).Build();
		}

		public override string ToString() {
			return Serialize();
		}

		public static MemoData Create( JObject value ) {
			var token = value.Root;
			var instance = new MemoData();
			instance.From = value.TryGetValue( FROM_FIELD_KEY, out token ) ? token.ToObject<PublicKey>() : null;
			instance.To = value.TryGetValue( TO_FIELD_KEY, out token ) ? token.ToObject<PublicKey>() : null;
			instance.Nonce = Convert.ToUInt64( value.TryGetValue( NONCE_FIELD_KEY, out token ) ? token.ToObject<object>() : 0 );
			instance.Message = value.TryGetValue( MESSAGE_FIELD_KEY, out token ) ? token.ToObject<string>() : string.Empty;
			return instance;
		}
	}
}