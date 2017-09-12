using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Base.Requests;
using Base.Responses;
using Newtonsoft.Json.Linq;
using Tools;
using UnityEngine;
using WebSocketSharp;


public sealed class ConnectionManager : SingletonMonoBehaviour<ConnectionManager> {

	static public event Action<Connection> OnConnectionChanged;
	static public event Action<Connection> OnConnectionOpened;
	static public event Action<Connection> OnConnectionClosed;
	static public event Action<Connection, Response> OnMessageReceived;

	public const string HTTP = "http://";
	public const string WSS = "wss://";
	public const string WS = "ws://";
	public const string SEPARATOR = "://";
	public const string DOT = ".";


	[SerializeField] float delayBetweenTryConnect = 5f;
	[SerializeField] int tryConnectCount = 5;
	[SerializeField] bool sendFromIndividualThread = false;

	string host = string.Empty;
	float lastTryConnectTime;
	int connectAttempts;
	bool connectProcessing;

	static bool lastServerAvaliableFlag;
	static Connection openConnection;


	void Update() {
		if ( !openConnection.IsNull() ) {
			//openConnection.DequeuOneReceivedMessage();
			openConnection.DequeuReceivedMessages( 20 );
		}
	}

	void OnApplicationQuit() {
		if ( !openConnection.IsNull() ) {
			openConnection.ConnectionOpened -= ConnectionOpened;
			openConnection.ConnectionClosed -= ConnectionClosed;
			openConnection.MessageReceived -= MessageReceived;
			openConnection.Disconnect();
			openConnection.Dispose();
			openConnection = null;
		}
	}

	protected override void OnDestroy() {
		OnConnectionChanged = null;
		OnConnectionOpened = null;
		OnConnectionClosed = null;
		OnMessageReceived = null;
		base.OnDestroy();
	}

	string Url {
		get { return host; }
	}

	public string PingUrl {
		get { return HTTP + "www.google.com/"; }// host + "/ping"; }
	}

	// call when client connect to server successful
	static void ConnectionOpened( Connection connection ) {
		Unity.Console.DebugLog( "Connected to", connection.FullUrl );
		if ( !OnConnectionOpened.IsNull() ) {
			OnConnectionOpened( connection );
		}
	}

	// call when client close connection
	static void ConnectionClosed( Connection connection ) {
		Unity.Console.DebugLog( "Connection closed" );
		if ( !OnConnectionClosed.IsNull() ) {
			OnConnectionClosed( connection );
		}
	}

	// call if received message not find event for processing
	static void MessageReceived( Connection connection, Response msg ) {
		Unity.Console.DebugLog( Unity.Console.SetYellowColor( "Received unbinding message:" ), Unity.Console.SetWhiteColor( msg ) );
		if ( !OnMessageReceived.IsNull() ) {
			OnMessageReceived( connection, msg );
		}
	}

	static public bool IsConnected {
		get { return IsServerAvaliable && (openConnection.ReadyState.Equals( WebSocketState.Connecting ) || openConnection.ReadyState.Equals( WebSocketState.Open )); }
	}

	static public bool IsServerAvaliable {
		get { return !openConnection.IsNull() && lastServerAvaliableFlag; }
	}

	static public WebSocketState ReadyState {
		get { return IsServerAvaliable ? openConnection.ReadyState : WebSocketState.Closed; }
	}

	static public void DoAll( List<Request> requests ) {
		var notSended = new List<Request>( requests );
		foreach ( var request in requests ) {
			if ( IsServerAvaliable && openConnection.Send( request ) ) {
				if ( request.Debug ) {
					request.PrintLog();
				}
				notSended.Remove( request );
			}
		}
		requests.Clear();
		requests.AddRange( notSended );
	}

	static public void Subscribe( string responseTitle, int subscribeId, Action<JToken[]> subscribeCallback, bool debug, bool singleCall = false ) {
		if ( !openConnection.IsNull() && !subscribeCallback.IsNull() ) {
			if ( singleCall ) {
				openConnection.AddRegular( subscribeId, response => {
					if ( debug ) {
						response.PrintLog( responseTitle );
					}
					response.SendNoticeData( subscribeCallback );
					Unsubscribe( response.RequestId );
				} );
			} else {
				openConnection.AddRegular( subscribeId, response => {
					if ( debug ) {
						response.PrintLog( responseTitle );
					}
					response.SendNoticeData( subscribeCallback );
				} );
			}
		}
	}

	static public void Unsubscribe( int subscribeId ) {
		if ( !openConnection.IsNull() ) {
			openConnection.RemoveRegular( subscribeId );
		}
	}

	public bool InitConnect() {
		if ( Url.IsNull() || Url.Equals( string.Empty ) ) {
			return false;
		}
		if ( connectProcessing ) {
			return false;
		}
		if ( connectAttempts >= tryConnectCount ) {
			if ( (Time.realtimeSinceStartup - lastTryConnectTime) <= delayBetweenTryConnect ) {
				return false;
			}
			connectAttempts = 0;
		}
		connectProcessing = true;
		StartCoroutine( TryConnect() );
		return true;
	}

	public void ReconnectTo( string newHost ) {
		if ( newHost.IsNull() || newHost.Equals( string.Empty ) ) {
			return;
		}
		if ( newHost.Equals( host ) ) {
			return;
		}
		host = newHost;
		Disconnect( true );
		lastServerAvaliableFlag = false;
		InitConnect();
	}

	public void Disconnect( bool resetConnection = false ) {
		if ( !openConnection.IsNull() ) {
			openConnection.Disconnect();
		}
		if ( resetConnection ) {
			openConnection = null;
		}
	}

	IEnumerator TryConnect() {
		var parts = Url.Split( new [] { SEPARATOR }, StringSplitOptions.None );
		var scheme = parts.First();
		var host = parts.Last();
		var request = new WWW( HTTP + host );
		yield return request;
		if ( request.url.Equals( HTTP + host ) && (lastServerAvaliableFlag = request.error.IsNull()) ) {
			lastTryConnectTime = Time.realtimeSinceStartup;
			connectAttempts++;
			if ( openConnection.IsNull() ) {
				openConnection = new Connection( scheme + SEPARATOR + host, sendFromIndividualThread );
				openConnection.ConnectionOpened += ConnectionOpened;
				openConnection.ConnectionClosed += ConnectionClosed;
				openConnection.MessageReceived += MessageReceived;
				if ( !OnConnectionChanged.IsNull() ) {
					OnConnectionChanged( openConnection );
				}
			}
			openConnection.Connect();
		} else {
			Unity.Console.DebugError( "SocketIONetworkConnection", "PingServer()", "Host", Url, "doesn't pinging.", "Error -", request.error );
		}
		connectProcessing = false;
	}
}