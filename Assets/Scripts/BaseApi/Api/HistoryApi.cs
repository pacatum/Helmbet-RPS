using Base.Data;
using Base.Data.Operations;
using Base.Requests;
using Promises;


namespace Base.Api.Database {

	public sealed class HistoryApi : ApiId {

		HistoryApi( ISender sender ) : base( null, sender ) { }

		public static HistoryApi Create( ISender sender ) {
			return new HistoryApi( sender );
		}

		public IPromise<HistoryApi> Init() {
			return new Promise<int>( ( resolve, reject ) => {
				var debug = false;
				var methodName = "history";
				var parameters = new Parameters { LoginApi.ID, methodName, new object[ 0 ] };
				DoRequest( GenerateNewId(), parameters, resolve, reject, methodName, debug );
			} ).Then( apiId => ( HistoryApi )Init( apiId ) );
		}

		public IPromise<OperationHistoryObject[]> GetAccountHistory( uint accountId, uint fromId, uint maxCount, uint toId ) {
			if ( IsInitialized ) {
				return new Promise<OperationHistoryObject[]>( ( resolve, reject ) => {
					var debug = true;
					var requestId = GenerateNewId();
					var methodName = "get_account_history";
					var title = methodName + " " + requestId;
					var parameters = new Parameters { Id.Value, methodName,
						new object[] { SpaceTypeId.ToString( SpaceType.Account, accountId ), SpaceTypeId.ToString( SpaceType.OperationHistory, fromId ), maxCount, SpaceTypeId.ToString( SpaceType.OperationHistory, toId ) } };
					DoRequest( requestId, parameters, resolve, reject, title, debug );
				} );
			}
			return Init().Then( api => api.GetAccountHistory( accountId, fromId, maxCount, toId ) );
		}
	}
}