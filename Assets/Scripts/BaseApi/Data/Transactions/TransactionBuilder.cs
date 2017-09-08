using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Base.Config;
using Base.Data.Assets;
using Base.Data.Operations;
using Base.Data.Properties;
using Base.ECC;
using Newtonsoft.Json.Linq;
using Promises;
using Tools;


namespace Base.Data.Transactions {

	public class TransactionBuilder : NullableObject {

		public class SignaturesContainer {

			public PublicKey[] PublicKeys { get; private set; }
			public object[] Addys { get; private set; }

			public SignaturesContainer( PublicKey[] publicKeys, object[] addys ) {
				PublicKeys = publicKeys;
				Addys = addys;
			}
		}


		protected static DateTime headBlockTime = Tool.ZeroTime();

		ushort referenceBlockNumber = ushort.MinValue;
		uint referenceBlockPrefix = uint.MinValue;
		DateTime expiration = Tool.ZeroTime();
		bool signed = false;
		byte[] buffer = new byte[ 0 ];

		readonly List<OperationData> operations = new List<OperationData>();
		readonly List<string> signatures = new List<string>();
		readonly List<KeyPair> signerKeys = new List<KeyPair>();


		public ushort ReferenceBlockNumber {
			get { return referenceBlockNumber; }
		}

		public uint ReferenceBlockPrefix {
			get { return referenceBlockPrefix; }
		}

		public DateTime Expiration {
			get { return expiration; }
		}

		public OperationData[] Operations {
			get { return operations.ToArray(); }
		}

		public string[] Signatures {
			get { return signatures.ToArray(); }
		}

		public TransactionBuilder() { }

		public TransactionBuilder AddOperation( OperationData operation ) {
			if ( IsFinalized ) {
				throw new InvalidOperationException( "AddOperation... Already finalized" );
			}
			if ( operation.Fee.IsNull() ) {
				operation.Fee = AssetData.EMPTY;
			}
			if ( operation.Type.Equals( ChainTypes.Operation.ProposalCreate ) ) {
				var proposalCreateOperation = ( ProposalCreateOperationData )operation;
				if ( proposalCreateOperation.ExpirationTime.IsZero() ) {
					proposalCreateOperation.ExpirationTime = Tool.ZeroTime().AddSeconds( BaseExpirationSeconds + ChainConfig.ExpireInSecondsProposal );
				}
			}
			operations.Add( operation );
			return this;
		}

		// Typically this is called automatically just prior to signing. Once finalized this transaction can not be changed.
		IPromise Finalize() {
			if ( IsFinalized ) {
				return Promise.Rejected( new InvalidOperationException( "Finalize... Already finalized" ) );
			}
			var dynamicGlobalProperties = SpaceTypeId.CreateOne( SpaceType.DynamicGlobalProperties );
			return Repository.GetInPromise( dynamicGlobalProperties, () => ApiManager.Instance.Database.GetObject<DynamicGlobalPropertiesObject>( dynamicGlobalProperties ) ).Then( FinalizeInPromise );
		}

		IPromise FinalizeInPromise( DynamicGlobalPropertiesObject dynamicGlobalProperty ) {
			headBlockTime = dynamicGlobalProperty.Time;
			if ( expiration.IsZero() ) {
				expiration = Tool.ZeroTime().AddSeconds( BaseExpirationSeconds + ChainConfig.ExpireInSeconds );
			}
			referenceBlockNumber = ( ushort )dynamicGlobalProperty.HeadBlockNumber;
			var prefix = Tool.FromHex( dynamicGlobalProperty.HeadBlockId );
			if ( !BitConverter.IsLittleEndian ) {
				Array.Reverse( prefix );
			}
			referenceBlockPrefix = BitConverter.ToUInt32( prefix, 4 );
			buffer = new TransactionData( this ).ToBuffer().ToArray();
			return Promise.Resolved();
		}

		public string Id {
			get {
				if ( !IsFinalized ) {
					throw new InvalidOperationException( "Not finalized" );
				}
				return Tool.ToHex( SHA256.Create().HashAndDispose( buffer ) ).Substring( 0, 40 );
			}
		}

		public DateTime SetExpireSeconds( long seconds ) {
			if ( IsFinalized ) {
				throw new InvalidOperationException( "SetExpireSeconds... Already finalized" );
			}
			return (expiration = Tool.ZeroTime().AddSeconds( BaseExpirationSeconds + seconds ));
		}

		// Wraps this transaction in a proposal_create transaction
		public TransactionBuilder Propose( ProposalCreateOperationData proposalCreateOperation ) {
			if ( IsFinalized ) {
				throw new InvalidOperationException( "Propose... Already finalized" );
			}
			if ( operations.IsNullOrEmpty() ) {
				throw new InvalidOperationException( "Propose... Add operation first" );
			}
			var proposedOperations = new OperationWrapperData[ operations.Count ];
			for ( var i = 0; i < operations.Count; i++ ) {
				proposedOperations[ i ] = new OperationWrapperData { Operation = operations[ i ] };
			}
			operations.Clear();
			signatures.Clear();
			signerKeys.Clear();
			proposalCreateOperation.ProposedOperations = proposedOperations;
			return AddOperation( proposalCreateOperation );
		}

		public bool HasProposedOperation {
			get { return operations.Exists( o => o.Type.Equals( ChainTypes.Operation.ProposalCreate ) ); }
		}

		// Optional: the fees can be obtained from the witness node
		public static IPromise<TransactionBuilder> SetRequiredFees( TransactionBuilder builder, SpaceTypeId asset = null ) {
			var feePool = 0L;
			if ( builder.IsFinalized ) {
				throw new InvalidOperationException( "SetRequiredFees... Already finalized" );
			}
			if ( builder.operations.IsNullOrEmpty() ) {
				throw new InvalidOperationException( "SetRequiredFees... Add operation first" );
			}
			var ops = new OperationData[ builder.operations.Count ];
			for ( var i = 0; i < builder.operations.Count; i++ ) {
				ops[ i ] = builder.operations[ i ].Clone();
			}
			var zeroAsset = SpaceTypeId.CreateOne( SpaceType.Asset );
			if ( asset.IsNull() ) {
				var firstFee = ops[ 0 ].Fee;
				if ( !firstFee.IsNull() && !firstFee.Asset.IsNullOrEmpty() ) {
					asset = firstFee.Asset;
				} else {
					asset = zeroAsset;
				}
			}

			var promises = new List<IPromise<object>>();
			promises.Add( ApiManager.Instance.Database.GetRequiredFees( ops, asset.Id ).Then<object>( feesData => feesData ) );

			if ( !asset.Equals( zeroAsset ) ) {
				// This handles the fallback to paying fees in BTS if the fee pool is empty.
				promises.Add( ApiManager.Instance.Database.GetRequiredFees( ops, zeroAsset.Id ).Then<object>( coreFeesData => coreFeesData ) );
				promises.Add( Repository.GetInPromise( asset, () => ApiManager.Instance.Database.GetAsset( asset.Id ) ).Then<object>( assetObject => assetObject ) );
			}

			return Promise<object>.All( promises.ToArray() ).Then( results => {
				var list = new List<object>( results ).ToArray();

				var feesData = list[ 0 ] as AssetData[];
				var coreFeesData = (list.Length > 1) ? (list[ 1 ] as AssetData[]) : null;
				var assetObject = (list.Length > 2) ? (list[ 2 ] as AssetObject) : null;

				var dynamicPromise = (!asset.Equals( zeroAsset ) && !asset.IsNull()) ?
					ApiManager.Instance.Database.GetObject<AssetDynamicDataObject>( assetObject.DynamicAssetData ) : Promise<AssetDynamicDataObject>.Resolved( null );

				return dynamicPromise.Then( dynamicObject => {
					if ( !asset.Equals( zeroAsset ) ) {
						feePool = !dynamicObject.IsNull() ? dynamicObject.FeePool : 0L;
						var totalFees = 0L;
						for ( var j = 0; j < coreFeesData.Length; j++ ) {
							totalFees += coreFeesData[ j ].Amount;
						}
						if ( totalFees > feePool ) {
							feesData = coreFeesData;
							asset = zeroAsset;
						}
					}

					// Proposed transactions need to be flattened
					var flatAssets = new List<AssetData>();
					Action<object> flatten = null;
					flatten = obj => {
						if ( obj.IsArray() ) {
							var array = obj as IList;
							for ( var k = 0; k < array.Count; k++ ) {
								flatten( array[ k ] );
							}
						} else {
							flatAssets.Add( ( AssetData )obj );
						}
					};
					flatten( feesData.OrEmpty() );

					var assetIndex = 0;

					Action<OperationData> setFee = null;
					setFee = operation => {
						if ( operation.Fee.IsNull() || operation.Fee.Amount == 0L ) {
							operation.Fee = flatAssets[ assetIndex ];
						}
						assetIndex++;
						if ( operation.Type.Equals( ChainTypes.Operation.ProposalCreate ) ) {
							var proposedOperations = (operation as ProposalCreateOperationData).ProposedOperations;
							for ( var y = 0; y < proposedOperations.Length; y++ ) {
								setFee( proposedOperations[ y ].Operation );
							}
						}
					};
					for ( var i = 0; i < builder.operations.Count; i++ ) {
						setFee( builder.operations[ i ] );
					}
				} );
			} ).Then( results => Promise<TransactionBuilder>.Resolved( builder ) );
		}

		public IPromise<SignaturesContainer> GetPotentialSignatures() {
			var signedTransaction = new SignedTransactionData( this );
			return Promise<object[]>.All(
				ApiManager.Instance.Database.GetPotentialSignatures( signedTransaction ).Then<object[]>( keys => keys ),
				ApiManager.Instance.Database.GetPotentialAddressSignatures( signedTransaction ).Then<object[]>( addresses => addresses )
			).Then( results => {
				return new SignaturesContainer( results.First() as PublicKey[], results.Last() as object[] );
			} );
		}

		public IPromise<PublicKey[]> GetRequiredSignatures( PublicKey[] availableKeys ) {
			if ( availableKeys.IsNullOrEmpty() ) {
				return Promise<PublicKey[]>.Resolved( new PublicKey[ 0 ] );
			}
			var signedTransaction = new SignedTransactionData( this );
			return ApiManager.Instance.Database.GetRequiredSignatures( signedTransaction, availableKeys );
		}

		public TransactionBuilder AddSigner( KeyPair key ) {
			if ( signed ) {
				throw new InvalidOperationException( "AddSigner... Already signed" );
			}
			// prevent duplicates
			if ( !signerKeys.Contains( key ) ) {
				signerKeys.Add( key );
			}
			return this;
		}

		void Sign() {
			if ( !IsFinalized ) {
				throw new InvalidOperationException( "Sign... Not finalized" );
			}
			if ( signed ) {
				throw new InvalidOperationException( "Sign... Already signed" );
			}
			if ( signerKeys.IsNullOrEmpty() ) {
				throw new InvalidOperationException( "Sign... Transaction was not signed. Do you have a private key?" );
			}
			foreach ( var key in signerKeys ) {
				signatures.Add( Tool.ToHex( Signature.SignBuffer( Tool.FromHex( ApiManager.ChainId ).Concat( buffer.OrEmpty() ), key.Private ).ToBuffer() ) );
			}
			signerKeys.Clear();
			signed = true;
		}

		public override string Serialize() {
			return new SignedTransactionData( this ).Serialize();
		}

		public bool IsFinalized {
			get { return !buffer.IsNullOrEmpty(); }
		}

		public IPromise Broadcast( Action<JToken[]> resultCallback ) {
			if ( IsFinalized ) {
				return BroadcastTransaction( this, resultCallback );
			}
			return Finalize().Then( () => BroadcastTransaction( this, resultCallback ) );
		}

		static double BaseExpirationSeconds {
			get {
				var headBlockSeconds = Math.Ceiling( headBlockTime.GetTimeFrom1Jan1970AtMilliseconds() / 1000.0 );
				var nowSeconds = Math.Ceiling( DateTime.UtcNow.GetTimeFrom1Jan1970AtMilliseconds() / 1000.0 );
				// The head block time should be updated every 3 seconds.  If it isn't
				// then help the transaction to expire (use headBlockSeconds)
				if ( nowSeconds - headBlockSeconds > 30.0 ) {
					return headBlockSeconds;
				}
				// If the user's clock is very far behind, use the head block time.
				return Math.Max( nowSeconds, headBlockSeconds );
			}
		}

		static IPromise BroadcastTransaction( TransactionBuilder transactionBuilder, Action<JToken[]> resultCallback ) {
			return new Promise( ( resolve, reject ) => {
				if ( !transactionBuilder.signed ) {
					transactionBuilder.Sign();
				}
				if ( !transactionBuilder.IsFinalized ) {
					throw new InvalidOperationException( "Not finalized" );
				}
				if ( transactionBuilder.signatures.IsNullOrEmpty() ) {
					throw new InvalidOperationException( "Not signed" );
				}
				if ( transactionBuilder.operations.IsNullOrEmpty() ) {
					throw new InvalidOperationException( "No operations" );
				}
				ApiManager.Instance.NetworkBroadcast.BroadcastTransactionWithCallback( resultCallback, new SignedTransactionData( transactionBuilder ) ).Then( resolve ).Catch( reject );
			} );
		}
	}
}