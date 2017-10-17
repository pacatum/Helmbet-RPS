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

	const string SELECTED_HOST_KEY = "host";
	const string HOSTS_LIST_KEY = "hosts_list";

	[SerializeField] string[] defaultHosts = new string[ 0 ];
	[SerializeField] bool resetAtStart;


	public string[] Urls {
		get {
			if ( !PlayerPrefs.HasKey( HOSTS_LIST_KEY ) ) {
				PlayerPrefs.SetString( HOSTS_LIST_KEY, JsonConvert.SerializeObject( defaultHosts ?? new string[ 0 ] ) );
			}
			return JsonConvert.DeserializeObject<string[]>( PlayerPrefs.GetString( HOSTS_LIST_KEY ) );
		}
		private set {
			PlayerPrefs.SetString( HOSTS_LIST_KEY, JsonConvert.SerializeObject( value.OrEmpty() ) );
			PlayerPrefs.Save();
		}
	}

	public string SelecteUrl {
		get {
			if ( !PlayerPrefs.HasKey( SELECTED_HOST_KEY ) ) {
				PlayerPrefs.SetString( SELECTED_HOST_KEY, defaultHosts.IsNullOrEmpty() ? string.Empty : defaultHosts[ 0 ] );
			}
			return PlayerPrefs.GetString( SELECTED_HOST_KEY );
		}
		private set {
			PlayerPrefs.SetString( SELECTED_HOST_KEY, value );
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
		var urls = Urls;
		foreach ( var defaultHost in defaultHosts ) {
			if ( !urls.Contains( defaultHost ) ) {
				ResetAll();
				break;
			}
		}
	}

	void Start() {
		InitConnection();
	}

	void ResetAll() {
		Unity.Console.DebugError( "NodeManager", "ResetAll()", "Reset all saved hosts." );
		PlayerPrefs.DeleteKey( HOSTS_LIST_KEY );
		PlayerPrefs.DeleteKey( SELECTED_HOST_KEY );
		PlayerPrefs.Save();
	}

	void InitConnection() {
		var url = SelecteUrl;
		if ( url.IsNull() || (url = url.Trim()).IsNullOrEmpty() ) {
			return;
		}
		if ( !Urls.Contains( url ) && !Urls.Contains( url ) ) {
			return;
		}
		if ( IsDefault( SelecteUrl ) ) {
			SelecteUrl = defaultHosts.Next( SelecteUrl );
		}
		ConnectionManager.Instance.ReconnectTo( SelecteUrl );
		ConnectionManager.OnConnectionAttemptsDone -= ConnectionAttemptsDone;
		ConnectionManager.OnConnectionAttemptsDone += ConnectionAttemptsDone;
	}

	void ConnectionAttemptsDone( string url ) {
		if ( IsDefault( url ) ) {
			ConnectionManager.Instance.ReconnectTo( SelecteUrl = defaultHosts.Next( url ) );
		}
	}

	bool Validation( string url ) {
		if ( !url.StartsWith( ConnectionManager.WSS, StringComparison.Ordinal ) ) {
			return false;
		}
		var host = url.Replace( ConnectionManager.WSS, string.Empty );
		if ( host.IsNullOrEmpty() ) {
			return false;
		}
		if ( !host.Contains( ConnectionManager.DOT ) || host.StartsWith( ConnectionManager.DOT, StringComparison.Ordinal ) ) {
			return false;
		}
		return true;
	}

	public bool IsDefault( string url ) {
		return !defaultHosts.IsNullOrEmpty() && defaultHosts.Contains( url );
	}

	public bool ConnectTo( string url, Action<ConnectResult> resultCallback ) {
		if ( url.IsNull() || (url = url.Trim()).IsNullOrEmpty() ) {
			return false;
		}
		if ( !Urls.Contains( url ) && !Urls.Contains( url ) ) {
			return false;
		}
		StartCoroutine( TryConnectTo( url, resultCallback ) );
		return true;
	}

	IEnumerator TryConnectTo( string url, Action<ConnectResult> resultCallback ) {
		var ping = new WWW( ConnectionManager.PingUrl );
		yield return ping;
		if ( ping.error.IsNull() ) {
			ping = new WWW( ConnectionManager.HTTP + url.Split( new [] { ConnectionManager.SEPARATOR }, StringSplitOptions.None ).Last() );
			yield return ping;
			if ( IsDefault( url ) || ping.error.IsNull() ) {
				ConnectionManager.Instance.ReconnectTo( SelecteUrl = url ); // save new host only if them exist
				resultCallback.Invoke( ConnectResult.Ok );
			} else {
				resultCallback.Invoke( ConnectResult.BadRequest );
			}
		} else {
			resultCallback.Invoke( ConnectResult.NoInternet );
		}
	}

	public bool AddHost( string url ) {
		if ( url.IsNull() || (url = url.Trim()).IsNullOrEmpty() ) {
			return false;
		}
		if ( !Validation( ConnectionManager.WSS + url.Split( new [] { ConnectionManager.SEPARATOR }, StringSplitOptions.None ).Last() ) ) {
			return false;
		}
		var currentUrls = Urls;
		if ( currentUrls.Contains( url ) ) {
			return false;
		}
		Urls = currentUrls.Add( url );
		return true;
	}

	public bool RemoveHost( string url ) {
		if ( IsDefault( url ) ) {
			return false;
		}
		var currentUrls = Urls;
		if ( !currentUrls.Contains( url ) ) {
			return false;
		}
		Urls = currentUrls.Remove( url );
		return true;
	}
}