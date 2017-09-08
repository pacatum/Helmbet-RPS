using Base.Requests;
using Promises;


namespace Base.Api.Database {

	public sealed class LoginApi : ApiId {

		public const int ID = 1;

		LoginApi( ISender sender ) : base( ID, sender ) { }

		public static LoginApi Create( ISender sender ) {
			return new LoginApi( sender );
		}

		public IPromise<bool> Login( string userName, string password ) {
			return new Promise<bool>( ( resolve, reject ) => {
				var debug = false;
				var methodName = "login";
				var parameters = new Parameters { Id.Value, methodName, new object[] { userName, password } };
				DoRequest( GenerateNewId(), parameters, resolve, reject, methodName, debug );
			} );
		}
	}
}