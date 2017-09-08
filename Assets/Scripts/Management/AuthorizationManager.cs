using System;
using System.Collections.Generic;
using Base;
using Base.Api.Database;
using Base.Data;
using Base.Data.Accounts;
using Base.Data.Pairs;
using Base.Data.Transactions;
using Base.ECC;
using Newtonsoft.Json.Linq;
using Promises;
using Tools;


public sealed class AuthorizationManager : SingletonMonoBehaviour<AuthorizationManager> {

	public class AuthorizationData {

		public Keys Keys { get; private set; }
		public UserNameFullAccountDataPair UserNameData { get; private set; }

		public AuthorizationData( Keys keys, UserNameFullAccountDataPair userNameData ) {
			Keys = keys;
			UserNameData = userNameData;
		}

		public void UpdateAccountData( IdObject idObject ) {
			if ( idObject.Id.Equals( UserNameData.FullAccount.Account.Id ) ) {
				UserNameData.FullAccount.Account = ( AccountObject )idObject;
			} else
			if ( idObject.Id.Equals( UserNameData.FullAccount.Statistics.Id ) ) {
				UserNameData.FullAccount.Statistics = ( AccountStatisticsObject )idObject;
			} else
			if ( idObject.Id.SpaceType.Equals( SpaceType.AccountBalance ) ) {
				for ( var i = 0; i < UserNameData.FullAccount.Balances.OrEmpty().Length; i++ ) {
					if ( idObject.Id.Equals( UserNameData.FullAccount.Balances[ i ].Id ) ) {
						UserNameData.FullAccount.Balances[ i ] = ( AccountBalanceObject )idObject;
						break;
					}
				}
			}
		}
	}


	public static event Action<AuthorizationData> OnAuthorizationChanged;

	AuthorizationData authorization;

	public AuthorizationData Authorization {
		get { return authorization; }
		private set {
			if ( authorization != value ) {
				authorization = value;
				if ( !OnAuthorizationChanged.IsNull() ) {
					OnAuthorizationChanged.Invoke( authorization );
				}
			}
		}
	}

	protected override void Awake() {
		ApiManager.OnDatabaseApiInitialized += ApiManager_OnDatabaseApiInitialized;
		base.Awake();
	}

	protected override void OnDestroy() {
		ApiManager.OnDatabaseApiInitialized -= ApiManager_OnDatabaseApiInitialized;
		base.OnDestroy();
	}

	void ApiManager_OnDatabaseApiInitialized( DatabaseApi api ) {

	}

	public IPromise<bool> AuthorizationBy( string userName, string password ) {
		return ApiManager.Instance.Database.GetFullAccount( userName.Trim(), false ).Then( result => {
			var validKeys = Keys.FromSeed( userName, password ).CheckAuthorization( result.FullAccount.Account );
			if ( !validKeys.IsNull() ) {
				if ( !Authorization.IsNull() ) {
					Repository.OnObjectUpdate -= Authorization.UpdateAccountData;
				}
				Authorization = new AuthorizationData( validKeys, result );
				Repository.OnObjectUpdate += Authorization.UpdateAccountData;
				return Promise<bool>.Resolved( true );
			}
			return Promise<bool>.Resolved( false );
		} );
	}

	public void ResetAuthorization() {
		if ( !Authorization.IsNull() ) {
			Repository.OnObjectUpdate -= Authorization.UpdateAccountData;
		}
		Authorization = null;

	}

	public IPromise ProcessTransaction( TransactionBuilder builder, Action<JToken[]> resultCallback ) {
		if ( !IsAuthorized ) {
			return Promise.Rejected( new InvalidOperationException( "Isn't Authorized!" ) );
		}
		var existPublicKeys = Authorization.Keys.PublicKeys;
		return new Promise( ( resolve, reject ) => TransactionBuilder.SetRequiredFees( builder ).Then( b => b.GetPotentialSignatures().Then( signatures => {
			var availableKeys = new List<PublicKey>();
			foreach ( var existPublicKey in existPublicKeys ) {
				if ( !availableKeys.Contains( existPublicKey ) && Array.IndexOf( signatures.PublicKeys, existPublicKey ) != -1 ) {
					availableKeys.Add( existPublicKey );
				}
			}
			if ( availableKeys.IsNullOrEmpty() ) {
				reject( new InvalidOperationException( "Available key doesn't find!" ) );
			}
			return b.GetRequiredSignatures( availableKeys.ToArray() ).Then( requiredPublicKeys => {
				if ( requiredPublicKeys.IsNullOrEmpty() ) {
					reject( new InvalidOperationException( "Required key doesn't find!" ) );
				}
				if ( !IsAuthorized ) {
					reject( new InvalidOperationException( "Isn't Authorized!" ) );
				}
				var selectedPublicKey = requiredPublicKeys.First(); // select key
				b.AddSigner( new KeyPair( Authorization.Keys[ selectedPublicKey ] ) ).Broadcast( resultCallback ).Then( resolve ).Catch( reject );
			} ).Catch( reject );
		} ).Catch( reject ) ).Catch( reject ) );
	}

	public bool IsAuthorized {
		get { return !Authorization.IsNull(); }
	}

	public UserNameFullAccountDataPair UserData {
		get { return IsAuthorized ? Authorization.UserNameData : null; }
	}
}