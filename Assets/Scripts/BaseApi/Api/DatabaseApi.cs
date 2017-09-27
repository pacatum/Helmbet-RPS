using System;
using Base.Config;
using Base.Data;
using Base.Data.Accounts;
using Base.Data.Assets;
using Base.Data.Block;
using Base.Data.Json;
using Base.Data.Operations;
using Base.Data.Pairs;
using Base.Data.Properties;
using Base.Data.Tournaments;
using Base.Data.Transactions;
using Base.ECC;
using Base.Requests;
using Newtonsoft.Json.Linq;
using Promises;
using Tools;


namespace Base.Api.Database {

	public sealed class DatabaseApi : ApiId {

		DatabaseApi( ISender sender ) : base( null, sender ) { }

		public static DatabaseApi Create( ISender sender ) {
			return new DatabaseApi( sender );
		}

		public IPromise<DatabaseApi> Init() {
			return new Promise<int>( ( resolve, reject ) => {
				var debug = false;
				var methodName = "database";
				var parameters = new Parameters { LoginApi.ID, methodName, new object[ 0 ] };
				DoRequest( GenerateNewId(), parameters, resolve, reject, methodName, debug );
			} ).Then( apiId => ( DatabaseApi )Init( apiId ) );
		}

		public IPromise<string> GetChainId() {
			if ( IsInitialized ) {
				return new Promise<string>( ( resolve, reject ) => {
					var debug = false;
					var requestId = GenerateNewId();
					var methodName = "get_chain_id";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[ 0 ] };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetChainId() );
		}

		public IPromise<DynamicGlobalPropertiesObject> GetDynamicGlobalProperties() {
			if ( IsInitialized ) {
				return new Promise<DynamicGlobalPropertiesObject>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "get_dynamic_global_properties";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[ 0 ] };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetDynamicGlobalProperties() );
		}

		public IPromise<GlobalPropertiesObject> GetGlobalProperties() {
			if ( IsInitialized ) {
				return new Promise<GlobalPropertiesObject>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "get_global_properties";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[ 0 ] };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetGlobalProperties() );
		}

		public IPromise<SignedBlockData> GetBlock( int blockNumber ) {
			if ( IsInitialized ) {
				return new Promise<SignedBlockData>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "get_block";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[] { blockNumber } };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetBlock( blockNumber ) );
		}

		public IPromise<TournamentObject[]> GetTournaments( uint fromId, uint maxCount, uint toId ) {
			if ( IsInitialized ) {
				return new Promise<TournamentObject[]>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "get_tournaments";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName,
						new object[] { SpaceTypeId.ToString( SpaceType.Tournament, fromId ), maxCount, SpaceTypeId.ToString( SpaceType.Tournament, toId ) } };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetTournaments( fromId, maxCount, toId ) );
		}

		public IPromise<TournamentObject[]> GetTournamentsInState( ChainTypes.TournamentState state, uint maxCount ) {
			if ( IsInitialized ) {
				return new Promise<TournamentObject[]>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "get_tournaments_in_state";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[] { TournamentStateEnumConverter.ConvertTo( state ), maxCount } };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetTournamentsInState( state, maxCount ) );
		}

		public IPromise<T[]> GetObjects<T>( SpaceTypeId[] objectIds ) {
			if ( IsInitialized ) {
				return new Promise<T[]>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "get_objects";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[] { Array.ConvertAll( objectIds, objectId => objectId.ToString() ) } };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetObjects<T>( objectIds ) );
		}

		public IPromise<T> GetObject<T>( SpaceTypeId objectId ) {
			return GetObjects<T>( new [] { objectId } ).Then( objects => objects.IsNullOrEmpty() ? default( T ) : objects[ 0 ] );
		}

		public IPromise<TournamentObject> GetTournament( uint id ) {
			return GetObject<TournamentObject>( SpaceTypeId.CreateOne( SpaceType.Tournament, id ) );
		}

		public IPromise<TournamentDetailsObject> GetTournamentDetails( uint id ) {
			return GetObject<TournamentDetailsObject>( SpaceTypeId.CreateOne( SpaceType.TournamentDetails, id ) );
		}

		public IPromise<TournamentDetailsObject[]> GetTournamentsDetails( uint[] ids ) {
			return GetObjects<TournamentDetailsObject>( SpaceTypeId.CreateMany( SpaceType.TournamentDetails, ids ) );
		}

		public IPromise<MatchObject> GetMatche( uint id ) {
			return GetObject<MatchObject>( SpaceTypeId.CreateOne( SpaceType.Match, id ) );
		}

		public IPromise<MatchObject[]> GetMatches( uint[] ids ) {
			return GetObjects<MatchObject>( SpaceTypeId.CreateMany( SpaceType.Match, ids ) );
		}

		public IPromise<GameObject> GetGame( uint id ) {
			return GetObject<GameObject>( SpaceTypeId.CreateOne( SpaceType.Game, id ) );
		}

		public IPromise<GameObject[]> GetGames( uint[] ids ) {
			return GetObjects<GameObject>( SpaceTypeId.CreateMany( SpaceType.Game, ids ) );
		}

		public IPromise<AssetObject> GetAsset( uint id = 0 ) {
			return GetObject<AssetObject>( SpaceTypeId.CreateOne( SpaceType.Asset, id ) );
		}

		public IPromise<AssetObject[]> GetAssets( uint[] ids ) {
			return GetObjects<AssetObject>( SpaceTypeId.CreateMany( SpaceType.Asset, ids ) );
		}

		public IPromise<AccountObject> GetAccount( uint id ) {
			return GetObject<AccountObject>( SpaceTypeId.CreateOne( SpaceType.Account, id ) );
		}

		public IPromise<AccountObject[]> GetAccounts( uint[] ids ) {
			return GetObjects<AccountObject>( SpaceTypeId.CreateMany( SpaceType.Account, ids ) );
		}

		public IPromise<UserNameFullAccountDataPair[]> GetFullAccounts( string[] userNamesOrIds, bool subscribe ) {
			if ( IsInitialized ) {
				return new Promise<UserNameFullAccountDataPair[]>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "get_full_accounts";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[] { userNamesOrIds, subscribe } };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetFullAccounts( userNamesOrIds, subscribe ) );
		}

		public IPromise<UserNameFullAccountDataPair> GetFullAccount( string userNameOrId, bool subscribe ) {
			return GetFullAccounts( new [] { userNameOrId }, subscribe ).Then( accounts => accounts.IsNullOrEmpty() ? null : accounts[ 0 ] );
		}

		public IPromise<UserNameAccountIdPair[]> LookupAccounts( string prefixName, uint maxCount ) {
			if ( IsInitialized ) {
				return new Promise<UserNameAccountIdPair[]>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "lookup_accounts";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[] { prefixName.ToLower(), maxCount } };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.LookupAccounts( prefixName, maxCount ) );
		}

		public IPromise<AssetData[]> GetRequiredFees( OperationData[] operations, uint assetId ) {
			if ( IsInitialized ) {
				return new Promise<AssetData[]>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "get_required_fees";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[] { operations, SpaceTypeId.ToString( SpaceType.Asset, assetId ) } };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetRequiredFees( operations, assetId ) );
		}

		public IPromise<AssetData> GetRequiredFee( OperationData operation, uint assetId ) {
			return GetRequiredFees( new [] { operation }, assetId ).Then( fees => fees.IsNullOrEmpty() ? null : fees[ 0 ] );
		}

		public IPromise<PublicKey[]> GetRequiredSignatures( SignedTransactionData transaction, PublicKey[] existKeys ) {
			if ( IsInitialized ) {
				return new Promise<PublicKey[]>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "get_required_signatures";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[] { transaction, Array.ConvertAll( existKeys, key => key.ToString() ) } };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetRequiredSignatures( transaction, existKeys ) );
		}

		public IPromise<PublicKey[]> GetPotentialSignatures( SignedTransactionData transaction ) {
			if ( IsInitialized ) {
				return new Promise<PublicKey[]>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "get_potential_signatures";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[] { transaction } };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetPotentialSignatures( transaction ) );
		}

		public IPromise<string[]> GetPotentialAddressSignatures( SignedTransactionData transaction ) {
			if ( IsInitialized ) {
				return new Promise<string[]>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "get_potential_address_signatures";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[] { transaction } };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetPotentialAddressSignatures( transaction ) );
		}

		public IPromise Subscribe( Action<JToken[]> subscribeResultCallback ) {
			if ( IsInitialized ) {
				return new Promise( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "set_subscribe_callback";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[] { requestId, true } };
					DoRequestVoid( requestId, parameters, () => {
						ConnectionManager.Subscribe( "subscribe by " + requestId, requestId, subscribeResultCallback, debug );
						resolve();
					}, reject, title, debug );
				} );
			}
			return Init().Then( api => api.Subscribe( subscribeResultCallback ) );
		}

		public IPromise<AssetData[]> GetAccountBalances( uint accountId, uint[] assetIds ) {
			if ( IsInitialized ) {
				return new Promise<AssetData[]>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "get_account_balances";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[] { SpaceTypeId.ToString( SpaceType.Account, accountId ), SpaceTypeId.ToStrings( SpaceType.Asset, assetIds ) } };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetAccountBalances( accountId, assetIds ) );
		}

		public IPromise<AssetData> GetAccountBalance( uint accountId, uint assetId = 0 ) {
			return GetAccountBalances( accountId, new [] { assetId } ).Then( balances => balances.IsNullOrEmpty() ? null : balances[ 0 ] );
		}
	}
}