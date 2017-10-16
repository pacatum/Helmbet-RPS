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
	static public event Action<string> OnConnectionAttemptsDone;

	const int MAX_PROCESSING_RECEIVED_MESSAGE_PER_UPDATE = 20;

	public const string HTTP = "http://";
	public const string WSS = "wss://";
	public const string WS = "ws://";
	public const string SEPARATOR = "://";
	public const string DOT = ".";

	[SerializeField] bool pingHostBeforeConnecting = true;
	[SerializeField] float delayBetweenTryConnect = 5f;
	[SerializeField] int tryConnectCount = 5;
	[SerializeField] bool sendFromIndividualThread = false;

	string url = string.Empty;
	float lastTryConnectTime;
	int connectAttempts;
	bool connectProcessing;

	static bool lastServerAvaliableFlag;
	static Connection openConnection;


	public static string PingUrl {
		get { return HTTP + "www.google.com/"; }
	}

	void Update() {
		if ( !openConnection.IsNull() ) {
			openConnection.DequeuReceivedMessages( MAX_PROCESSING_RECEIVED_MESSAGE_PER_UPDATE );
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
		if ( url.IsNull() || url.Equals( string.Empty ) ) {
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
			if ( !OnConnectionAttemptsDone.IsNull() ) {
				OnConnectionAttemptsDone( url );
				return false;
			}
		}
		if ( pingHostBeforeConnecting ) {
			connectProcessing = true;
			StartCoroutine( PingAndConnect( url ) );
		} else {
			lastServerAvaliableFlag = true;
			Connect( url );
			lastTryConnectTime = Time.realtimeSinceStartup;
			connectAttempts++;
		}
		return true;
	}

	public void ReconnectTo( string newUrl ) {
		if ( newUrl.IsNull() || newUrl.Equals( string.Empty ) ) {
			return;
		}
		if ( newUrl.Equals( url ) ) {
			return;
		}
		url = newUrl;
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

	IEnumerator PingAndConnect( string targetHost ) {
		var parts = targetHost.Split( new [] { SEPARATOR }, StringSplitOptions.None );
		var scheme = parts.First();
		var host = parts.Last();
		var pindHostRequest = new WWW( HTTP + host );
		yield return pindHostRequest;
		if ( (lastServerAvaliableFlag = pindHostRequest.error.IsNull()) ) {
			Connect( scheme + SEPARATOR + host );
		} else {
			Unity.Console.DebugError( "ConnectionManager", "TryConnect()", "Host", host, "doesn't pinging.", "Error -", pindHostRequest.error );
		}
		lastTryConnectTime = Time.realtimeSinceStartup;
		connectAttempts++;
		connectProcessing = false;
	}

	void Connect( string targetHost ) {
		if ( openConnection.IsNull() ) {
			openConnection = new Connection( targetHost, sendFromIndividualThread );
			openConnection.ConnectionOpened += ConnectionOpened;
			openConnection.ConnectionClosed += ConnectionClosed;
			openConnection.MessageReceived += MessageReceived;
			if ( !OnConnectionChanged.IsNull() ) {
				OnConnectionChanged( openConnection );
			}
		}
		openConnection.Connect();
	}
}