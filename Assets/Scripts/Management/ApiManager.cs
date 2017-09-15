using System;
using System.Collections.Generic;
using Base;
using Base.Api;
using Base.Api.Database;
using Base.Config;
using Base.Requests;
using Base.Responses;
using Promises;
using Tools;
using WebSocketSharp;


public sealed class ApiManager : SingletonMonoBehaviour<ApiManager>, ISender {

	public static event Action<string> OnConnectionOpened;
	public static event Action<string> OnConnectionClosed;

	public static event Action OnAllApiInitialized;
	public static event Action<DatabaseApi> OnDatabaseApiInitialized;
	public static event Action<NetworkBroadcastApi> OnNetworkBroadcastApiInitialized;
	public static event Action<HistoryApi> OnHistoryApiInitialized;
	public static event Action<CryptoApi> OnCryptoApiInitialized;

	static string chainId = string.Empty;
	static RequestIdentificator identificators;

	readonly static List<Request> requestBuffer = new List<Request>();

	[UnityEngine.SerializeField]
	bool sendByUpdate = false;

	DatabaseApi database;
	NetworkBroadcastApi networkBroadcast;
	HistoryApi history;
	CryptoApi crypto;


	public static string ChainId {
		get { return chainId.OrEmpty(); }
	}

	public static bool CanDoRequest {
		get { return !DoesInstanceExist().IsNull() && ConnectionManager.IsConnected; }
	}

	public DatabaseApi Database {
		get { return database ?? (database = DatabaseApi.Create( this )); }
	}

	public NetworkBroadcastApi NetworkBroadcast {
		get { return networkBroadcast ?? (networkBroadcast = NetworkBroadcastApi.Create( this )); }
	}

	public HistoryApi History {
		get { return history ?? (history = HistoryApi.Create( this )); }
	}

	public CryptoApi Crypto {
		get { return crypto ?? (crypto = CryptoApi.Create( this )); }
	}


	#region UnityCallbacks
	protected override void Awake() {
		identificators = new RequestIdentificator( 0 );
		ConnectionManager.OnConnectionChanged += InitRegularCallbacks;
		base.Awake();
	}

	void Update() {
		UpConnection();
		if ( sendByUpdate && CanSend ) {
			ConnectionManager.DoAll( requestBuffer );
		}
	}

	void UpConnection() {
		if ( ConnectionManager.ReadyState.Equals( WebSocketState.Closed ) || ConnectionManager.ReadyState.Equals( WebSocketState.Closing ) ) {
			ConnectionManager.Instance.InitConnect();
		}
	}

	bool CanSend {
		get { return ConnectionManager.ReadyState.Equals( WebSocketState.Open ); }
	}

	protected override void OnDestroy() {
		ConnectionManager.OnConnectionChanged -= InitRegularCallbacks;
		base.OnDestroy();
	}

	void OnApplicationPause( bool state ) {
	}

	void OnApplicationQuit() {
		requestBuffer.Clear();
	}
	#endregion


	#region Initialization
	void ConnectionOpened( Response response ) {
		Unity.Console.DebugLog( "ApiManager class", Unity.Console.SetMagentaColor( "Regular Callback:" ), "ConnectionOpened()" );
		InitializeApi( LoginApi.Create( this ) );
		response.SendResultData<string>( reason => {
			if ( OnConnectionOpened != null ) {
				OnConnectionOpened.Invoke( reason );
			}
		} );
	}

	void ConnectionClosed( Response response ) {
		Unity.Console.DebugLog( "ApiManager class", Unity.Console.SetMagentaColor( "Regular Callback:" ), "ConnectionClosed()" );
		ResetApi();
		response.SendResultData<string>( reason => {
			if ( OnConnectionClosed != null ) {
				OnConnectionClosed.Invoke( reason );
			}
		} );
	}

	void ResetApi() {
		database = null;
		networkBroadcast = null;
		history = null;
		crypto = null;
	}

	void InitializeDone() {
		if ( !OnAllApiInitialized.IsNull() ) {
			OnAllApiInitialized.Invoke();
		}
	}

	void InitializeApi( LoginApi api ) {
		api.Login( string.Empty, string.Empty ).Then( loginResult => {
			if ( loginResult ) {
				Promise.All(
					Database.Init().Then( DatabaseApiInitialized ),
					NetworkBroadcast.Init().Then( NetworkBroadcastApiInitialized ),
					History.Init().Then( HistoryApiInitialized ),
					Crypto.Init().Then( CryptoApiInitialized )
				).Then( ( Action )InitializeDone );
			} else {
				Unity.Console.DebugLog( "RequestManager class", Unity.Console.SetRedColor( "Login Failed!" ), "Login()" );
			}
		} );
	}

	IPromise DatabaseApiInitialized( DatabaseApi api ) {
		return api.GetChainId().Then( ( Action<string> )SetChainId ).Then( result => {
			return Repository.SubscribeToNotice( api ).Then( () => {
				if ( !OnDatabaseApiInitialized.IsNull() ) {
					OnDatabaseApiInitialized.Invoke( api );
				}
				return Promise.Resolved();
			} );
		} );
	}

	IPromise NetworkBroadcastApiInitialized( NetworkBroadcastApi api ) {
		return new Promise( ( resolved, rejected ) => {
			if ( !OnNetworkBroadcastApiInitialized.IsNull() ) {
				OnNetworkBroadcastApiInitialized.Invoke( api );
			}
			resolved();
		} );
	}

	IPromise HistoryApiInitialized( HistoryApi api ) {
		return new Promise( ( resolved, rejected ) => {
			if ( !OnHistoryApiInitialized.IsNull() ) {
				OnHistoryApiInitialized.Invoke( api );
			}
			resolved();
		} );
	}

	IPromise CryptoApiInitialized( CryptoApi api ) {
		return new Promise( ( resolved, rejected ) => {
			if ( !OnCryptoApiInitialized.IsNull() ) {
				OnCryptoApiInitialized.Invoke( api );
			}
			resolved();
		} );
	}
	#endregion


	#region Request/Response
	public void Send( Request request ) {
		requestBuffer.Add( request );
		if ( !sendByUpdate && CanSend ) {
			ConnectionManager.DoAll( requestBuffer );
		}
	}

	public RequestIdentificator Identificators {
		get { return identificators; }
	}

	void InitRegularCallbacks( Connection connection ) {
		connection.AddRegular( identificators.OpenId, ConnectionOpened );
		connection.AddRegular( identificators.CloseId, ConnectionClosed );
	}
	#endregion

	
	static void SetChainId( string newChainId ) {
		newChainId = newChainId.OrEmpty();
		chainId = newChainId;
		ChainConfig.SetChainId( newChainId );
	}
}