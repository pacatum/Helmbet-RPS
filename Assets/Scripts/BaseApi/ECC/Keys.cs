using System;
using System.Collections.Generic;
using Base.Data;
using Base.Data.Accounts;


namespace Base.ECC {

	public class Keys : IDisposable {

		const string OWNER_KEY = "owner";
		const string ACTIVE_KEY = "active";
		const string MEMO_KEY = "memo";

		readonly Dictionary<AccountRole, KeyPair> keys = new Dictionary<AccountRole, KeyPair>();


		Keys() { }

		Keys( Dictionary<AccountRole, KeyPair> keys ) {
			this.keys = keys;
		}

		public static Keys FromSeed( string userName, string password, bool activeRewriteMemo = true ) {
			var keys = new Dictionary<AccountRole, KeyPair>();
			var roles = new [] { AccountRole.Owner, AccountRole.Active, AccountRole.Memo };
			foreach ( var role in roles ) {
				if ( role.Equals( AccountRole.Memo ) && activeRewriteMemo ) {
					keys[ role ] = new KeyPair( GetRole( AccountRole.Active ), userName, password );
				} else {
					keys[ role ] = new KeyPair( GetRole( role ), userName, password );
				}
			}
			return new Keys( keys );
		}

		public PrivateKey this[ PublicKey publicKey ] {
			get {
				foreach ( var keyPair in keys.Values ) {
					if ( keyPair.Equals( publicKey ) ) {
						return keyPair.Private;
					}
				}
				return null;
			}
		}

		public int Count {
			get { return keys.Count; }
		}

		public PublicKey[] PublicKeys {
			get {
				var result = new List<PublicKey>();
				foreach ( var keyPair in keys.Values ) {
					result.Add( keyPair.Public );
				}
				return result.ToArray();
			}
		}

		public Keys CheckAuthorization( AccountObject account ) {
			if ( account == null ) {
				return null;
			}
			var result = new Dictionary<AccountRole, KeyPair>();
			foreach ( var pair in keys ) {
				if ( account.IsEquelKey( pair.Key, pair.Value ) ) {
					result[ pair.Key ] = pair.Value;
				}
			}
			return (result.Count > 0) ? new Keys( result ) : null;
		}

		public void Dispose() {
			keys.Clear();
		}

		static string GetRole( AccountRole role ) {
			if ( role.Equals( AccountRole.Owner ) ) {
				return OWNER_KEY;
			}
			if ( role.Equals( AccountRole.Active ) ) {
				return ACTIVE_KEY;
			}
			if ( role.Equals( AccountRole.Memo ) ) {
				return MEMO_KEY;
			}
			return string.Empty;
		}
	}
}