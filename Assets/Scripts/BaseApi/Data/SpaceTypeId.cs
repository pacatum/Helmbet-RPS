using System;
using System.Text;
using Buffers;
using Base.Data.Json;
using Newtonsoft.Json;
using Tools;


namespace Base.Data {

	[JsonConverter( typeof( SpaceTypeEnumConverter ) )]
	public enum SpaceType {
		
		Unknown,
		Base,
		Account,
		Asset,
		ForceSettlement,
		CommitteeMember,
		Witness,
		LimitOrder,
		CallOrder,
		Custom,
		Proposal,
		OperationHistory,
		WithdrawPermission,
		VestingBalance,
		Worker,
		Balance,
		Tournament,
		TournamentDetails,
		Match,
		Game,
		GlobalProperties,
		DynamicGlobalProperties,
		AssetDynamicData,
		AssetBitassetData,
		AccountBalance,
		AccountStatistics,
		Transaction,
		BlockSummary,
		AccountTransactionHistory,
		BlindedBalance,
		ChainProperty,
		WitnessSchedule,
		BudgetRecord,
		SpecialAuthority,
		BuyBack,
		FbaAccumulator,
		AssetDividendData,
		PendingDividendPayoutBalanceForHolder,
		DistributedDividendBalanceData
	}


	[JsonConverter( typeof( SpaceTypeIdConverter ) )]
	public sealed class SpaceTypeId : NullableObject, ISerializeToBuffer, IEquatable<SpaceTypeId> , IComparable<SpaceTypeId> {

		public readonly static SpaceTypeId EMPTY = new SpaceTypeId();

		const char TYPE_SEPARATOR = '.';

		readonly byte space = 0;
		readonly byte type = 0;
		readonly uint id = 0;


		public static SpaceTypeId[] CreateMany( SpaceType spaceType, uint[] ids ) {
			var result = new SpaceTypeId[ ids.Length ];
			for ( var i = 0; i < ids.Length; i++ ) {
				result[ i ] = CreateOne( spaceType, ids[ i ] );
			}
			return result;
		}

		public static SpaceTypeId CreateOne( SpaceType spaceType, uint id = uint.MinValue ) {
  
			return Create( ToString( spaceType, id ) );
		}

		public static SpaceTypeId Create( string spaceTypeId ) {
			if ( spaceTypeId.IsNullOrEmpty() ) {
				throw new NullReferenceException();
			}
			var parts = spaceTypeId.Split( TYPE_SEPARATOR );
			if ( parts.Length != 3 ) {
				throw new FormatException( spaceTypeId );
			}
			return new SpaceTypeId(
				Convert.ToByte( parts[ 0 ] ),
				Convert.ToByte( parts[ 1 ] ),
				Convert.ToUInt32( parts[ 2 ] )
			);
		}

		SpaceTypeId() { }

		SpaceTypeId( byte space, byte type, uint id ) {
			this.space = space;
			this.type = type;
			this.id = id;
		}

		public uint Id {
			get { return id; }
		}

		public SpaceType SpaceType {
			get { return SpaceTypeEnumConverter.ConvertFrom( space.ToString() + TYPE_SEPARATOR + type.ToString() ); }
		}

		public override string Serialize() {
			var builder = new StringBuilder();
			builder.Append( space );
			builder.Append( TYPE_SEPARATOR );
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
			if ( !(obj is SpaceTypeId) ) {
				return false;
			}
			return Equals( ( SpaceTypeId )obj );
		}

		public bool Equals( SpaceTypeId spaceTypeId ) {
			return ToString().Equals( spaceTypeId.ToString() );
		}

		public int CompareTo( SpaceTypeId other ) {
			return string.Compare( ToString(), other.ToString(), StringComparison.Ordinal );
		}

		public static int CompareTo( SpaceTypeId a, SpaceTypeId b ) {
			return a.CompareTo( b );
		}

		public static string ToString( SpaceType spaceType, uint id = uint.MinValue ) {
			return SpaceTypeEnumConverter.ConvertTo( spaceType ) + TYPE_SEPARATOR + id;
		}

		public static string[] ToStrings( SpaceType spaceType, uint[] ids ) {
			var result = new string[ ids.Length ];
			for ( var i = 0; i < ids.Length; i++ ) {
				result[ i ] = ToString( spaceType, ids[ i ] );
			}
			return result;
		}

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteVarInt32( ( int )id );
			return buffer;
		}
	}
}