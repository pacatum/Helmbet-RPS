using System;
using System.Collections;
using Newtonsoft.Json;
using Tools;
using UnityEngine;


public class NodeManager : SingletonMonoBehaviour<NodeManager> {

	public enum ConnectResult {
		
		NoInternet,
		BadRequest,
		Ok
	}


	const string SELECTED_HOST = "host";
	const string HOSTS_LIST = "hosts_list";

	[SerializeField] string defaultHost = "http://";
	[SerializeField] bool resetAtStart;


	public string[] Hosts {
		get {
			if ( !PlayerPrefs.HasKey( HOSTS_LIST ) ) {
				PlayerPrefs.SetString( HOSTS_LIST, JsonConvert.SerializeObject( new [] { defaultHost } ) );
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
				PlayerPrefs.SetString( SELECTED_HOST, defaultHost );
			}
			return PlayerPrefs.GetString( SELECTED_HOST );
		}
		private set {
			PlayerPrefs.SetString( SELECTED_HOST, value );
			PlayerPrefs.Save();
		}
	}

	protected override void Awake() {
		base.Awake();
		if ( resetAtStart ) {
			ResetAll();
		}
		InitConnection();
	}

	void ResetAll() {
#if UNITY_EDITOR
		PlayerPrefs.DeleteKey( HOSTS_LIST );
		PlayerPrefs.DeleteKey( SELECTED_HOST );
		PlayerPrefs.Save();
#endif
	}

	void InitConnection() {
		ConnectTo( SelecteHost, result => { } );
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
		return defaultHost.Equals( host );
	}

	public bool ConnectTo( string host, Action<ConnectResult> resultCallback ) {
		if ( host.IsNull() || (host = host.Trim()).IsNullOrEmpty() ) {
			return false;
		}
		host = ConnectionManager.WSS + host.Split( new [] { ConnectionManager.SEPARATOR }, StringSplitOptions.None ).Last();
		if ( !Hosts.Contains( host ) ) {
			return false;
		}
		StartCoroutine( TryConnectTo( host, resultCallback ) );
		return true;
	}

	IEnumerator TryConnectTo( string host, Action<ConnectResult> resultCallback ) {
		var ping = new WWW( ConnectionManager.Instance.PingUrl );
		yield return ping;
		if ( ping.error.IsNull() ) {
			host = host.Split( new [] { ConnectionManager.SEPARATOR }, StringSplitOptions.None ).Last();
			ping = new WWW( ConnectionManager.HTTP + host );
			yield return ping;
			if ( ping.error.IsNull() ) {
				host = ConnectionManager.WSS + host;
				ConnectionManager.Instance.ReconnectTo( SelecteHost = host );
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
		if ( defaultHost.Equals( host ) ) {
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