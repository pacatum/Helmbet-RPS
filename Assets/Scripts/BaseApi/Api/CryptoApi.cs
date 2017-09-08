using Base.Requests;
using Promises;


namespace Base.Api.Database {

	public sealed class CryptoApi : ApiId {

		CryptoApi( ISender sender ) : base( null, sender ) { }

		public static CryptoApi Create( ISender sender ) {
			return new CryptoApi( sender );
		}

		public IPromise<CryptoApi> Init() {
			return new Promise<int>( ( resolve, reject ) => {
				var debug = false;
				var methodName = "crypto";
				var parameters = new Parameters { LoginApi.ID, methodName, new object[ 0 ] };
				DoRequest( GenerateNewId(), parameters, resolve, reject, methodName, debug );
			} ).Then( apiId => ( CryptoApi )Init( apiId ) );
		}
	}
}