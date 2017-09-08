using System;
using Tools;


namespace Base.ECC {

	public class KeyPair : IEquatable<KeyPair>, IEquatable<PublicKey> {

		readonly PrivateKey privateKey = null;


		KeyPair() { }

		public KeyPair( string role, string userName, string password ) {
			privateKey = PrivateKey.FromSeed( password.Trim() + userName.Trim() + role.Trim() );
		}

		public KeyPair( PrivateKey privateKey, string associatePublicKey = null ) {
			this.privateKey = privateKey;
			if ( !associatePublicKey.IsNull() ) {
				Assert.Equal( associatePublicKey, Public.ToString(), "Associate public key doesn't equal with generated public key" );
			}
		}

		public bool Equals( KeyPair otherKeyPair ) {
			return Equals( otherKeyPair.Public );
		}

		public bool Equals( PublicKey publicKey ) {
			return Public.Equals( publicKey );
		}

		public PrivateKey Private {
			get { return privateKey; }
		}

		public PublicKey Public {
			get { return privateKey.ToPublicKey(); }
		}
	}
}