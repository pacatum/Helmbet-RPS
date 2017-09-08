using System;
using Base.Data.Transactions;
using Base.Requests;
using Newtonsoft.Json.Linq;
using Promises;


namespace Base.Api.Database {

	public sealed class NetworkBroadcastApi : ApiId {

		NetworkBroadcastApi( ISender sender ) : base( null, sender ) { }

		public static NetworkBroadcastApi Create( ISender sender ) {
			return new NetworkBroadcastApi( sender );
		}

		public IPromise<NetworkBroadcastApi> Init() {
			return new Promise<int>( ( resolve, reject ) => {
				var debug = false;
				var methodName = "network_broadcast";
				var parameters = new Parameters { LoginApi.ID, methodName, new object[ 0 ] };
				DoRequest( GenerateNewId(), parameters, resolve, reject, methodName, debug );
			} ).Then( apiId => ( NetworkBroadcastApi )Init( apiId ) );
		}

		public IPromise BroadcastTransactionWithCallback( Action<JToken[]> transactionResultCallback, SignedTransactionData transaction ) {
			if ( IsInitialized ) {
				return new Promise( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "broadcast_transaction_with_callback";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName, new object[] { requestId, transaction } };
					DoRequestVoid( requestId, parameters, () => {
						ConnectionManager.Subscribe( "broadcast by " + requestId, requestId, transactionResultCallback, debug, true );
						resolve();
					}, reject, title, debug );
				} );
			}
			return Init().Then( api => api.BroadcastTransactionWithCallback( transactionResultCallback, transaction ) );
		}
	}
}