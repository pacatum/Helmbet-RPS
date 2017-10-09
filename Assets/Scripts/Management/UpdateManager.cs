using System;
using Base.Api.Database;
using Base.Config;
using Base.Data;
using Base.Data.Operations;
using HtmlAgilityPack;
using Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public sealed class UpdateManager : SingletonMonoBehaviour<UpdateManager> {

	public sealed class VersionInfo : IEquatable<VersionInfo>, IComparable<VersionInfo> {

		sealed class DisplayTextData {
			
			[JsonProperty( "en" )]
			public string EnglishLocaleHTMLValue { get; private set; }
			[JsonProperty( "ja" )]
			public string JapanLocaleHTMLValue { get; private set; }
		}


		const string EMPTY_VERSION_VALUE = "0.0.0";

		const string VERSION_KEY = "version";
		const string DISPLAY_TEXT_KEY = "displayText";

		const string HYPER_REFERENCE_ATTRIBUTE_NAME = "href";

		[JsonProperty( VERSION_KEY )]
		string version;
		[JsonProperty( DISPLAY_TEXT_KEY )]
		DisplayTextData displayText;

		public VersionInfo( string version ) {
			this.version = version;
		}

		public static bool IsInstance( JObject jsonObject ) {
			var properties = new System.Collections.Generic.List<JProperty>( jsonObject.Properties() );
			return properties.Exists( p => VERSION_KEY.Equals( p.Name ) ) && properties.Exists( p => DISPLAY_TEXT_KEY.Equals( p.Name ) );
		}

		public int CompareTo( VersionInfo other ) {
			if ( Object.ReferenceEquals( other, null ) ) {
				return 1;
			}
			var partsThis = version.Or( EMPTY_VERSION_VALUE ).Split( '.' );
			var partsOther = other.version.Or( EMPTY_VERSION_VALUE ).Split( '.' );
			var size = Math.Max( partsThis.Length, partsOther.Length );
			if ( partsThis.Length < size ) {
				partsThis = partsThis.Concat( new string[ size - partsThis.Length ].Fill( "0" ) );
			}
			if ( partsOther.Length < size ) {
				partsOther = partsOther.Concat( new string[ size - partsOther.Length ].Fill( "0" ) );
			}
			for ( var i = 0; i < size; i++ ) {
				var numberThis = Convert.ToInt32( partsThis[ i ] );
				var numberOther = Convert.ToInt32( partsOther[ i ] );
				if ( numberThis > numberOther ) {
					return 1;
				}
				if ( numberOther > numberThis ) {
					return -1;
				}
			}
			return 0;
		}

		public static int Compare( VersionInfo left, VersionInfo right ) {
			return Object.ReferenceEquals( left, right ) ? 0 : (Object.ReferenceEquals( left, null ) ? -1 : left.CompareTo( right ));
		}

		public override bool Equals( object obj ) {
			if ( Object.ReferenceEquals( obj, null ) ) {
				return false;
			}
			if ( Object.ReferenceEquals( this, obj ) ) {
				return true;
			}
			if ( !(obj is VersionInfo) ) {
				return false;
			}
			return Equals( obj as VersionInfo );
		}

		public bool Equals( VersionInfo other ) {
			return !Object.ReferenceEquals( other, null ) && CompareTo( other ) == 0;
		}

		public override int GetHashCode() {
			return Version.GetHashCode();
		}

		public static bool operator ==( VersionInfo left, VersionInfo right ) {
			return Object.ReferenceEquals( left, null ) ? Object.ReferenceEquals( right, null ) : left.Equals( right );
		}

		public static bool operator !=( VersionInfo left, VersionInfo right ) {
			return !(left == right);
		}

		public static bool operator <( VersionInfo left, VersionInfo right ) {
			return Compare( left, right ) < 0;
		}

		public static bool operator >( VersionInfo left, VersionInfo right ) {
			return Compare( left, right ) > 0;
		}

		public string Version {
			get { return version; }
		}

		public string Title {
			get {
				try {
					var document = new HtmlDocument();
					document.LoadHtml( displayText.EnglishLocaleHTMLValue );
					return document.DocumentNode.FirstChild.InnerText;
				} catch {
					return null;
				}
			}
		}

		public string Url {
			get {
				try {
					var document = new HtmlDocument();
					document.LoadHtml( displayText.EnglishLocaleHTMLValue );
					return document.DocumentNode.LastChild.Attributes[ HYPER_REFERENCE_ATTRIBUTE_NAME ].Value;
				} catch {
					return null;
				}
			}
		}
	}


	public const string CURRENT_VERSION = "1.0.0";

	const string DEFAULT_UPDATER = "ppcoreupdates";
	const string LAST_CUSTOM_UPDATER_KEY = "last_custom_updater_key";

	public static event Action<VersionInfo> OnNewVersionAvailable;

	[UnityEngine.SerializeField] float checkNewVersionIntervalSeconds = 300f;

	bool initialized;
	bool work;
	float elapsedTime;
	SpaceTypeId account;
	SpaceTypeId lastOperationHistory = SpaceTypeId.CreateOne( SpaceType.OperationHistory );


	protected override void Awake() {
		ApiManager.OnDatabaseApiInitialized += UpdateManager_OnDatabaseApiInitialized;
		base.Awake();
	}

	void Update() {
		if ( initialized && work ) {
			elapsedTime += UnityEngine.Time.deltaTime;
			if ( elapsedTime >= checkNewVersionIntervalSeconds ) {
				CheckNewVersion();
			}
			elapsedTime %= checkNewVersionIntervalSeconds;
		}
	}

	protected override void OnDestroy() {
		ApiManager.OnDatabaseApiInitialized -= UpdateManager_OnDatabaseApiInitialized;
		base.OnDestroy();
	}

	void UpdateManager_OnDatabaseApiInitialized( DatabaseApi api ) {
		Init();
	}

	void Init() {
		ApiManager.Instance.Database.GetFullAccount( SelectUpdater(), false ).Then( fullAccount => {
			if ( !fullAccount.IsNull() ) {
				account = fullAccount.FullAccount.Account.Id;
				initialized = work = true;
			}
		} );
	}

	string SelectUpdater() {
		return CustomUpdater ?? DEFAULT_UPDATER;
	}

	public string CustomUpdater {
		get {
			if ( UnityEngine.PlayerPrefs.HasKey( LAST_CUSTOM_UPDATER_KEY ) ) {
				return UnityEngine.PlayerPrefs.GetString( LAST_CUSTOM_UPDATER_KEY );
			}
			return null;
		}
		set {
			UnityEngine.PlayerPrefs.SetString( LAST_CUSTOM_UPDATER_KEY, value );
			UnityEngine.PlayerPrefs.Save();
			if ( ApiManager.Instance.Database.IsInitialized ) {
				Init();
			}
		}
	}

	void CheckNewVersion() {
		var fromId = lastOperationHistory.Id;
		ApiManager.Instance.History.GetAccountHistory( account.Id, fromId, 100, 0 ).Then( operations => {
			if ( !operations.IsNullOrEmpty() ) {
				Array.Sort( operations, OperationHistoryObject.Compare );
				var currentVersion = new VersionInfo( CURRENT_VERSION );
				var maxVersion = currentVersion;
				for ( var i = 0; i < operations.Length; i++ ) {
					var operationData = operations[ i ].Operation;
					if ( operationData.Type.Equals( ChainTypes.Operation.Transfer ) ) {
						var memo = (operationData as TransferOperationData).Memo;
						if ( !memo.IsNull() && !memo.Message.IsNullOrEmpty() ) {
							try {
								var token = JToken.Parse( Tool.FromHex2Chars( memo.Message ) );
								if ( token.Type.Equals( JTokenType.Object ) ) {
									var values = token.ToObject<JObject>();
									if ( VersionInfo.IsInstance( values ) ) {
										var versionInfo = values.ToObject<VersionInfo>();
										if ( versionInfo > maxVersion ) {
											maxVersion = versionInfo;
										}
									}
								}
							} catch {
								continue;
							}
						}
					}
				}
				if ( !maxVersion.IsNull() && maxVersion > currentVersion ) {
					if ( !OnNewVersionAvailable.IsNull() ) {
						OnNewVersionAvailable.Invoke( maxVersion );
						work = false;
						elapsedTime = 0f;
					}
				}
				lastOperationHistory = operations.Last().Id;
			}
		} );
	}

	public void ReStart() {
		if ( !work ) {
			work = true;
		}
	}
}