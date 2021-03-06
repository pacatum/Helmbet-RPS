﻿using System;
using System.Collections.Generic;
using System.Threading;
using Base.Eventing;
using Base.Requests;
using Base.Responses;
using Tools;
using WebSocketSharp;


namespace Base {

	public sealed class Connection : IDisposable {

		public event Action<Connection> ConnectionOpened;
		public event Action<Connection> ConnectionClosed;
		public event Action<Connection, Response> MessageReceived;

		Queue<string> sendingQueue;
		Queue<Response> receivedQueue;
		Uri uri;
		WebSocket webSocket;
		CallbackControl callbackManager;

		readonly ManualResetEvent connectionOpenEvent = new ManualResetEvent( false );
		readonly object connectionLocker = new object();
		readonly object webSocketLocker = new object();


		public WebSocketState ReadyState {
			get { return !webSocket.IsNull() ? webSocket.ReadyState : WebSocketState.Closed; }
		}

		public string Url {
			get { return uri.OriginalString; }
		}

		public string FullUrl {
			get { return !webSocket.IsNull() ? webSocket.Url.ToString() : string.Empty; }
		}

		public Connection( string url, bool sendFromThread ) {
			uri = new Uri( url );
			callbackManager = new CallbackControl();
			receivedQueue = new Queue<Response>();
			if ( sendFromThread ) {
				new Thread( ThreadDequeuSendMessages ).Start( sendingQueue = new Queue<string>() );
			}
		}


		#region Connect
		public void Connect() {
			lock ( connectionLocker ) {
				if ( webSocket.IsNull() || (!webSocket.ReadyState.Equals( WebSocketState.Connecting ) && !webSocket.ReadyState.Equals( WebSocketState.Open )) ) {
					try {
						connectionOpenEvent.Reset(); // set wait connection
						OpenWebSocket( uri );
					} catch ( Exception ex ) {
						Unity.Console.DebugError( "Client::Connect() Exception:", ex.Message );
					}
				}
			}
		}

		public bool IsConnected {
			get { return !webSocket.IsNull() && webSocket.ReadyState.Equals( WebSocketState.Open ); }
		}
		#endregion


		#region RegisterEvent
		public void AddRegular( int eventId, Action<Response> action ) {
			callbackManager.AddRegularCallback( eventId, action );
		}

		public void RemoveRegular( int eventId ) {
			callbackManager.RemoveRegularCallback( eventId );
		}

		public void ResetRequest( int eventId ) {
			callbackManager.ResetRequestCallback( eventId );
		}
		#endregion


		#region Send
		public bool SendFromThread {
			get { return !sendingQueue.IsNull(); }
		}

		public bool Send( Request request ) {
			if ( SendFromThread ) {
				if ( request is RequestCallback ) {
					callbackManager.SetRequestCallback( request as RequestCallback );
				}
				sendingQueue.Enqueue( request.ToString() );
				return true;
			}
			if ( IsConnected ) {
				if ( request is RequestCallback ) {
					callbackManager.SetRequestCallback( request as RequestCallback );
				}
				webSocket.Send( request.ToString() );
				return true;
			}
			return false;
		}

		void ThreadDequeuSendMessages( object parameter ) {
			var queue = parameter as Queue<string>;
			while ( SendFromThread ) {
				if ( IsConnected ) {
					try {
						if ( queue.Count > 0 ) {
							var message = queue.Dequeue();
							if ( !message.IsNull() ) {
								webSocket.Send( message );
							}
						}
					} catch ( Exception ex ) {
						Unity.Console.DebugError( "Client::ThreadDequeuSendMessages() Exception:", ex.Message );
					}
				} else {
					connectionOpenEvent.WaitOne( 2000 ); // wait connect signal or wait 2 sec
				}
			}
			Unity.Console.DebugError( "Client::ThreadDequeuSendMessages() Thread done" );
		}
		#endregion


		#region Receive
		public int ReceivedQueueCount {
			get { return receivedQueue.Count; }
		}

		public int DequeuOneReceivedMessage() {
			var dequeuCount = 0;
			try {
				if ( receivedQueue.Count > 0 ) {
					var message = receivedQueue.Dequeue();
					dequeuCount++;
					if ( !callbackManager.InvokeCallback( message ) ) {
						if ( !MessageReceived.IsNull() ) {
							MessageReceived( this, message );
						}
					}
				}
			} catch ( Exception ex ) {
				Unity.Console.DebugError( "Client::DequeuOneReceivedMessage() Exception:", ex.Message );
			}
			return dequeuCount;
		}

		public int DequeuReceivedMessages( int maxDequeuCount ) {
			var dequeuCount = 0;
			try {
				while ( (receivedQueue.Count > 0) && (dequeuCount < maxDequeuCount) ) {
					var message = receivedQueue.Dequeue();
					dequeuCount++;
					if ( !callbackManager.InvokeCallback( message ) ) {
						if ( !MessageReceived.IsNull() ) {
							MessageReceived( this, message );
						}
					}
				}
			} catch ( Exception ex ) {
				Unity.Console.DebugError( "Client::DequeuReceivedMessages() Exception:", ex.Message );
			}
			return dequeuCount;
		}

		public int DequeuAllReceivedMessages() {
			var dequeuCount = 0;
			try {
				while ( receivedQueue.Count > 0 ) {
					var message = receivedQueue.Dequeue();
					dequeuCount++;
					if ( !callbackManager.InvokeCallback( message ) ) {
						if ( MessageReceived.IsNull() ) {
							MessageReceived( this, message );
						}
					}
				}
			} catch ( Exception ex ) {
				Unity.Console.DebugError( "Client::DequeuAllReceivedMessages() Exception:", ex.Message );
			}
			return dequeuCount;
		}
		#endregion


		#region Close
		public void Disconnect() {
			Close();
		}

		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
			Unity.Console.DebugLog( "Client::Dispose() Dispose client" );
		}

		void Close() {
			Unity.Console.DebugLog( "Client::Close() Close client" );
			CloseWebSocket();
			if ( SendFromThread ) {
				sendingQueue.Clear();
			}
			if ( !ConnectionClosed.IsNull() ) {
				ConnectionClosed( this );
			}
		}

		void Dispose( bool disposing ) {
			if ( disposing ) {
				connectionOpenEvent.Close();
				CloseWebSocket();
				uri = null;
				if ( !callbackManager.IsNull() ) {
					callbackManager.Dispose();
					callbackManager = null;
				}
				if ( !sendingQueue.IsNull() ) {
					sendingQueue.Clear();
					sendingQueue = null;
				}
				if ( !receivedQueue.IsNull() ) {
					receivedQueue.Clear();
					receivedQueue = null;
				}
				connectionOpenEvent.Close();
			}
		}
		#endregion


		#region WebSocket
		void OpenWebSocket( Uri serverUri ) {
			lock ( webSocketLocker ) {
				if ( !webSocket.IsNull() ) {
					webSocket.OnMessage -= WebSocketMessageReceived;
					webSocket.OnError -= WebSocketError;
					webSocket.OnOpen -= WebSocketOpened;
					webSocket.OnClose -= WebSocketClose;
					if ( webSocket.ReadyState.Equals( WebSocketState.Connecting ) || webSocket.ReadyState.Equals( WebSocketState.Open ) ) {
						webSocket.CloseAsync();
					}
				}
				var wsScheme = serverUri.Scheme;
				if ( !wsScheme.Equals( "wss" ) && !wsScheme.Equals( "ws" ) ) {
					wsScheme = serverUri.Scheme.Equals( Uri.UriSchemeHttps ) ? "wss" : "ws";
				}
				webSocket = new WebSocket( string.Format( "{0}://{1}/ws", wsScheme, serverUri.Host + ((serverUri.Port > 0) ? (":" + serverUri.Port) : string.Empty) ) );
				webSocket.SslConfiguration.ServerCertificateValidationCallback = ( sender, certificate, chain, sslPolicyErrors ) => true;
				webSocket.OnOpen += WebSocketOpened;
				webSocket.OnClose += WebSocketClose;
				webSocket.OnMessage += WebSocketMessageReceived;
				webSocket.OnError += WebSocketError;
				webSocket.ConnectAsync();
			}
		}

		void CloseWebSocket() {
			lock ( webSocketLocker ) {
				if ( !webSocket.IsNull() ) {
					webSocket.OnMessage -= WebSocketMessageReceived;
					webSocket.OnError -= WebSocketError;
					webSocket.OnOpen -= WebSocketOpened;
					webSocket.OnClose -= WebSocketClose;
					if ( webSocket.ReadyState.Equals( WebSocketState.Connecting ) || webSocket.ReadyState.Equals( WebSocketState.Open ) ) {
						webSocket.CloseAsync();
					}
					webSocket = null;
				}
			}
		}

		void WebSocketOpened( object sender, EventArgs e ) {
			connectionOpenEvent.Set(); // send signal about connection done
			if ( !ConnectionOpened.IsNull() ) {
				ConnectionOpened( this );
			}
			receivedQueue.Enqueue( Response.Open( Url ) );
		}

		void WebSocketMessageReceived( object sender, MessageEventArgs e ) {
//			Unity.Console.DebugLog( "Client::WebSocketMessageReceived() Message:", Unity.Console.SetWhiteColor( e.Data ) );
			receivedQueue.Enqueue( Response.Parse( e.Data ) );
		}

		void WebSocketError( object sender, ErrorEventArgs e ) {
			Unity.Console.DebugError( "Client::WebSocketError() Exception:", Unity.Console.SetRedColor( e.Message ), e.Exception.Message );
			Close();
		}

		void WebSocketClose( object sender, CloseEventArgs e ) {
			var builder = new System.Text.StringBuilder();
			builder.Append( e.Reason ).Append( ' ' ).Append( e.Code );
			switch ( e.Code ) {
			case 1000:
				builder.Append( " Normal closure." );
				break;
			case 1001:
				builder.Append( " An endpoint is going away." );
				break;
			case 1002:
				builder.Append( " An endpoint is terminating the connection due to a protocol error." );
				break;
			case 1003:
				builder.Append( " An endpoint is terminating the connection because it has received a type of data it cannot accept." );
				break;
			case 1004:
				builder.Append( " Reserved." );
				break;
			case 1005:
				builder.Append( " No status code was actually present." );
				break;
			case 1006:
				builder.Append( " The connection was closed abnormally." );
				break;
			case 1007:
				builder.Append( " The endpoint is terminating the connection because a message was received that contained inconsistent data." );
				break;
			case 1008:
				builder.Append( " The endpoint is terminating the connection because it received a message that violates its policy." );
				break;
			case 1009:
				builder.Append( " The endpoint is terminating the connection because a data frame was received that is too large." );
				break;
			case 1010:
				builder.Append( " The client is terminating the connection because it expected the server to negotiate one or more extension, but the server didn't." );
				break;
			case 1011:
				builder.Append( " The server is terminating the connection because it encountered an unexpected condition that prevented it from fulfilling the request." );
				break;
			case 1012:
				builder.Append( " The server is terminating the connection because it is restarting." );
				break;
			case 1013:
				builder.Append( " The server is terminating the connection due to a temporary condition." );
				break;
			case 1015:
				builder.Append( " The connection was closed due to a failure to perform a TLS handshake (e.g., the server certificate can't be verified)." );
				break;
			default:
				builder.Append( " Unknown error." );
				break;
			}
			builder.Append( ' ' ).Append( "Url: " ).Append( FullUrl );
			Unity.Console.DebugWarning( "Client::WebSocketClose() Reason:", builder );
			receivedQueue.Enqueue( Response.Close( builder.ToString() ) );
		}
		#endregion
	}
}