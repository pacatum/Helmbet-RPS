using System;
using Buffers;
using Base.Data.Json;
using Base.Data.Operations;
using Newtonsoft.Json;
using Tools;


namespace Base.Data.Transactions {

	//var transaction = new Serializer( "transaction", {
	//	ref_block_num: uint16,
	//	ref_block_prefix: uint32,
	//	expiration: time_point_sec,
	//	operations: array( operation ),
	//	extensions: set( future_extensions )
	//} );
	public class TransactionData : NullableObject {

		[JsonProperty( "ref_block_num" )]
		public ushort ReferenceBlockNumber { get; set; }
		[JsonProperty( "ref_block_prefix" )]
		public uint ReferenceBlockPrefix { get; set; }
		[JsonProperty( "expiration" ), JsonConverter( typeof( DateTimeConverter ) )]
		public DateTime Expiration { get; set; }
		[JsonProperty( "operations" )]
		public OperationData[] Operations { get; set; }
		[JsonProperty( "extensions" )]
		public object[] Extensions { get; set; }

		public TransactionData() {
			Expiration = Tool.ZeroTime();
			Operations = new OperationData[ 0 ];
			Extensions = new object[ 0 ];
		}

		public TransactionData( TransactionBuilder builder ) : this() {
			ReferenceBlockNumber = builder.ReferenceBlockNumber;
			ReferenceBlockPrefix = builder.ReferenceBlockPrefix;
			Expiration = builder.Expiration;
			Operations = builder.Operations;
		}

		public ByteBuffer ToBuffer( ByteBuffer buffer = null ) {
			buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			buffer.WriteUInt16( ReferenceBlockNumber );
			buffer.WriteUInt32( ReferenceBlockPrefix );
			buffer.WriteDateTime( Expiration );
			buffer.WriteArray( Operations, ( b, item ) => {
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
	}
}