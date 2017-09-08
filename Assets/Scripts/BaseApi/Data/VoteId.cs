using System;
using System.Text;
using Buffers;
using Base.Config;
using Base.Data.Json;
using Newtonsoft.Json;
using Tools;


namespace Base.Data {

	[JsonConverter( typeof( VoteIdConverter ) )]
	public sealed class VoteId : NullableObject, ISerializeToBuffer, IEquatable<VoteId>, IComparable<VoteId> {

		public readonly static VoteId EMPTY = new VoteId();

		const char TYPE_SEPARATOR = ':';

		readonly byte type = 0;
		readonly uint id = 0;


		public static VoteId Create( ChainTypes.VoteType voteType, uint id = uint.MinValue ) {
			return Create( ToString( voteType, id ) );
		}

		public static VoteId Create( string voteId ) {
			if ( voteId.IsNullOrEmpty() ) {
				throw new NullReferenceException();
			}
			var parts = voteId.Split( TYPE_SEPARATOR );
			if ( parts.Length != 2 ) {
				throw new FormatException( voteId );
			}
			return new VoteId(
				Convert.ToByte( parts[ 0 ] ),
				Convert.ToUInt32( parts[ 1 ] )
			);
		}

		VoteId() { }

		VoteId( byte type, uint id ) {
			this.type = type;
			this.id = id;
		}

		public uint Id {
			get { return id; }
		}

		public ChainTypes.VoteType VoteType {
			get { return ( ChainTypes.VoteType )type; }
		}

		public override string Serialize() {
			var builder = new StringBuilder();
			builder.Append( type );
			builder.Append( TYPE_SEPARATOR );
			builder.Append( id );
			return builder.ToString();
		}

		public override int GetHashCode() {
			return ToString().GetHashCode();
		}

		public override bool Equals( object obj ) {
			if ( this == obj ) {
				return true;
			}
			if ( !(obj is VoteId) ) {
				return false;
			}
			return Equals( ( VoteId )obj );
		}

		public bool Equals( VoteId voteId ) {
			return ToString().Equals( voteId.ToString() );
		}

		public static string ToString( ChainTypes.VoteType voteType, uint id ) {
			return (( byte )voteType).ToString() + TYPE_SEPARATOR + id;
		}

		public static string[] ToStrings( ChainTypes.VoteType voteType, uint[] ids ) {
			var result = new string[ ids.Length ];
			for ( var i = 0; i < ids.Length; i++ ) {
				result[ i ] = ToString( voteType, ids[ i ] );
			}
			return result;
		}

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteUInt32( id << 8 | type );
			return buffer;
		}

		public int CompareTo( VoteId other ) {
			return ( int )id - ( int )other.id;
		}

		public static int CompareTo( VoteId a, VoteId b ) {
			return a.CompareTo( b );
		}
	}
}