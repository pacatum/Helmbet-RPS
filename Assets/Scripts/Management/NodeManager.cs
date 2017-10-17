using System;
using System.Collections;
using Newtonsoft.Json;
using Tools;
using UnityEngine;


public sealed class NodeManager : SingletonMonoBehaviour<NodeManager> {

	public enum ConnectResult {
		
		NoInternet,
		BadRequest,
		Ok
	}


	public static Action<string> OnSelecteHostChanged;

	const string SELECTED_HOST = "host";
	const string HOSTS_LIST = "hosts_list";

	[SerializeField] string[] defaultHosts = new string[ 0 ];
	[SerializeField] bool resetAtStart;


	public string[] Hosts {
		get {
			if ( !PlayerPrefs.HasKey( HOSTS_LIST ) ) {
				PlayerPrefs.SetString( HOSTS_LIST, JsonConvert.SerializeObject( defaultHosts ?? new string[ 0 ] ) );
			}
			return JsonConvert.DeserializeObject<string[]>( PlayerPrefs.GetString( HOSTS_LIST ) );
		}
		private set {
			PlayerPrefs.SetString( HOSTS_LIST, JsonConvert.SerializeObject( value.OrEmpty() ) );
			PlayerPrefs.Save();
		}
	}

	public string SelecteHost {
		get {
			if ( !PlayerPrefs.HasKey( SELECTED_HOST ) ) {
				PlayerPrefs.SetString( SELECTED_HOST, defaultHosts.IsNullOrEmpty() ? string.Empty : defaultHosts[ 0 ] );
			}
			return PlayerPrefs.GetString( SELECTED_HOST );
		}
		private set {
			PlayerPrefs.SetString( SELECTED_HOST, value );
			PlayerPrefs.Save();
			if ( !OnSelecteHostChanged.IsNull() ) {
				OnSelecteHostChanged( value );
			}
		}
	}

	protected override void Awake() {
		base.Awake();
#if UNITY_EDITOR
		if ( resetAtStart ) {
			ResetAll();
		}
#endif
		var hosts = Hosts;
		foreach ( var defaultHost in defaultHosts ) {
			if ( !hosts.Contains( defaultHost ) ) {
				ResetAll();
				break;
			}
		}
	}

	void Start() {
		InitConnection();
	}

	void ResetAll() {
		Unity.Console.DebugError( "NodeManager", "ResetAll()", "Reset all saved hosts" );
		PlayerPrefs.DeleteKey( HOSTS_LIST );
		PlayerPrefs.DeleteKey( SELECTED_HOST );
		PlayerPrefs.Save();
	}

	void InitConnection() {
		var host = SelecteHost;
		if ( host.IsNull() || (host = host.Trim()).IsNullOrEmpty() ) {
			return;
		}
		var parts = host.Split( new [] { ConnectionManager.SEPARATOR }, StringSplitOptions.None );
		var scheme = parts.First();
		host = parts.Last();
		if ( !Hosts.Contains( ConnectionManager.WSS + host ) && !Hosts.Contains( ConnectionManager.WS + host ) ) {
			return;
		}
		if ( IsDefault( SelecteHost ) ) {
			SelecteHost = defaultHosts.Next( SelecteHost );
		}
		ConnectionManager.Instance.ReconnectTo( SelecteHost );
		ConnectionManager.OnConnectionAttemptsDone -= ConnectionAttemptsDone;
		ConnectionManager.OnConnectionAttemptsDone += ConnectionAttemptsDone;
	}

	void ConnectionAttemptsDone( string host ) {
		if ( IsDefault( SelecteHost ) ) {
			SelecteHost = defaultHosts.Next( SelecteHost );
			ConnectionManager.Instance.ReconnectTo( SelecteHost );
		}
	}

	bool Validation( string host ) {
		if ( !host.StartsWith( ConnectionManager.WSS, StringComparison.Ordinal ) ) {
			return false;
		}
		host = host.Replace( ConnectionManager.WSS, string.Empty );
		if ( host.IsNullOrEmpty() ) {
			return false;
		}
		if ( !host.Contains( ConnectionManager.DOT ) || host.StartsWith( ConnectionManager.DOT, StringComparison.Ordinal ) ) {
			return false;
		}
		return true;
	}

	public bool IsDefault( string host ) {
		return !defaultHosts.IsNullOrEmpty() && defaultHosts.Contains( host );
	}

	public bool ConnectTo( string host, Action<ConnectResult> resultCallback ) {
		if ( host.IsNull() || (host = host.Trim()).IsNullOrEmpty() ) {
			return false;
		}
		host = host.Split( new [] { ConnectionManager.SEPARATOR }, StringSplitOptions.None ).Last();
		if ( !Hosts.Contains( ConnectionManager.WSS + host ) && !Hosts.Contains( ConnectionManager.WS + host ) ) {
			return false;
		}
		StartCoroutine( TryConnectTo( host, resultCallback ) );
		return true;
	}

	IEnumerator TryConnectTo( string host, Action<ConnectResult> resultCallback ) {
		var ping = new WWW( ConnectionManager.PingUrl );
		yield return ping;
		if ( ping.error.IsNull() ) {
			var parts = host.Split( new [] { ConnectionManager.SEPARATOR }, StringSplitOptions.None );
			var scheme = parts.First();
			host = parts.Last();
			ping = new WWW( ConnectionManager.HTTP + host );
			yield return ping;
			if ( ping.error.IsNull() ) {
				host = scheme + ConnectionManager.SEPARATOR + host;
				SelecteHost = host; // save new host only if them exist
				ConnectionManager.Instance.ReconnectTo( host );
				resultCallback.Invoke( ConnectResult.Ok );
			} else {
				resultCallback.Invoke( ConnectResult.BadRequest );
			}
		} else {
			resultCallback.Invoke( ConnectResult.NoInternet );
		}
	}

	public bool AddHost( string newHost ) {
		if ( newHost.IsNull() || (newHost = newHost.Trim()).IsNullOrEmpty() ) {
			return false;
		}
		newHost = ConnectionManager.WSS + newHost.Split( new [] { ConnectionManager.SEPARATOR }, StringSplitOptions.None ).Last();
		if ( !Validation( newHost ) ) {
			return false;
		}
		var currentHosts = Hosts;
		if ( currentHosts.Contains( newHost ) ) {
			return false;
		}
		Hosts = currentHosts.Add( newHost );
		return true;
	}

	public bool RemoveHost( string host ) {
		if ( !defaultHosts.IsNullOrEmpty() && defaultHosts.Contains( host ) ) {
			return false;
		}
		var currentHosts = Hosts;
		if ( !currentHosts.Contains( host ) ) {
			return false;
		}
		Hosts = currentHosts.Remove( host );
		return true;
	}
}