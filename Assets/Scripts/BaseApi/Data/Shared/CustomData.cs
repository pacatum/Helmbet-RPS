using System;
using System.Collections;
using System.Collections.Generic;
using Buffers;
using Base.Data.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data {

	[JsonConverter( typeof( CustomDataConverter ) )] // only JTokenType.Object will be converted
	public sealed class CustomData : NullableObject, ISerializeToBuffer {

		public Dictionary<string, object> Fields { get; private set; }

		public CustomData() {
			Fields = new Dictionary<string, object>();
		}

		public CustomData( JObject value ) {
			Fields = new Dictionary<string, object>();
			foreach ( var property in value.Properties() ) {
				var token = property.Value;
				Fields[ property.Name ] = token.Type.Equals( JTokenType.Object ) ? new CustomData( token.ToObject<JObject>() ) : token.ToObject<object>();
			}
		}

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteVarInt32( Fields.Count );
			foreach ( var fields in Fields ) {
				buffer.WriteString( fields.Key );
				WriteToBuffer( buffer, fields.Value );
			}
			return buffer;
		}

		public static void WriteToBuffer( ByteBuffer buffer, object value ) {
			if ( !value.IsNull() ) {
				var type = value.GetType();
				if ( type.IsNumeric() ) {
					if ( type.IsFloating() ) {
						buffer.WriteUInt8( 3 );
						buffer.WriteDouble( Convert.ToDouble( value ) );
					} else {
						if ( Convert.ToDouble( value ) < 0.0 ) {
							buffer.WriteUInt8( 1 );
							buffer.WriteInt64( Convert.ToInt64( value ) );
						} else {
							buffer.WriteUInt8( 2 );
							buffer.WriteUInt64( Convert.ToUInt64( value ) );
						}
					}
				} else if ( type.Equals( typeof( bool ) ) ) {
					buffer.WriteUInt8( 4 );
					buffer.WriteBool( Convert.ToBoolean( value ) );
				} else if ( type.Equals( typeof( string ) ) ) {
					buffer.WriteUInt8( 5 );
					buffer.WriteString( Convert.ToString( value ) );
				} else if ( type.IsArray ) {
					buffer.WriteUInt8( 6 );
					buffer.WriteArray( (( IList )value).ToArray(), WriteToBuffer );
				} else if ( value is ISerializeToBuffer ) {
					buffer.WriteUInt8( 7 );
					(value as ISerializeToBuffer).ToBuffer( buffer );
				} else {
					throw new InvalidOperationException( "Not support type: " + type.Name );
				}
			}
		}

		public override string Serialize() {
			return new JSONBuilder( new JSONDictionary( Fields ) ).Build();
		}
	}
}