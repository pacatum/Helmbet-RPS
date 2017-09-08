using Buffers;
using Base.Config;
using Base.Data.Json;
using Newtonsoft.Json;
using Tools;


namespace Base.Data.Tournaments.GameSettings {

	[JsonConverter( typeof( GameSpecificSettingsDataPairConverter ) )]
	public abstract class GameSpecificSettingsData : NullableObject, ISerializeToBuffer {

		public abstract ChainTypes.GameSpecificSettings Type { get; }
		public abstract ByteBuffer ToBufferRaw( ByteBuffer buffer = null );

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteVarInt32( ( int )Type );
			return ToBufferRaw( buffer );
		}

		public GameSpecificSettingsData Clone() {
			return ( GameSpecificSettingsData )JsonConvert.DeserializeObject( JsonConvert.SerializeObject( this, new GameSpecificSettingsDataPairConverter() ), GetType() );
		}
	}
}