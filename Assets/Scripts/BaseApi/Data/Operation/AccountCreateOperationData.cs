using System;
using Buffers;
using Base.Config;
using Base.Data.Accounts;
using Base.ECC;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Operations {

	public sealed class AccountCreateOperationData : OperationData {

		const string FEE_FIELD_KEY = "fee";
		const string REGISTRAR_FIELD_KEY = "registrar";
		const string REFERRER_FIELD_KEY = "referrer";
		const string REFERRER_PERCENT_FIELD_KEY = "referrer_percent";
		const string NAME_FIELD_KEY = "name";
		const string OWNER_FIELD_KEY = "owner";
		const string ACTIVE_FIELD_KEY = "active";
		const string OPTIONS_FIELD_KEY = "options";
		const string EXTENSIONS_FIELD_KEY = "extensions";

		public override AssetData Fee { get; set; }
		public SpaceTypeId Registrar { get; private set; }
		public SpaceTypeId Referrer { get; private set; } 
		public ushort ReferrerPercent { get; private set; }
		public string Name { get; private set; }
		public AuthorityData Owner { get; private set; }
		public AuthorityData Active { get; private set; }
		public AccountOptionsData Options { get; private set; }
		public object Extensions { get; private set; }

		public override ChainTypes.Operation Type {
			get { return ChainTypes.Operation.AccountCreate; }
		}

		public bool IsEquelKey( AccountRole role, KeyPair key ) {
			switch ( role ) {
			case AccountRole.Owner:
				if ( Owner != null && Owner.KeyAuths != null ) {
					foreach ( var keyAuth in Owner.KeyAuths ) {
						if ( keyAuth.IsEquelKey( key ) ) {
							return true;
						}
					}
				}
				return false;
			case AccountRole.Active:
				if ( Active != null && Active.KeyAuths != null ) {
					foreach ( var keyAuth in Active.KeyAuths ) {
						if ( keyAuth.IsEquelKey( key ) ) {
							return true;
						}
					}
				}
				return false;
			case AccountRole.Memo:
				return Options != null && Options.IsEquelKey( key );
			default:
				return false;
			}
		}

		public override ByteBuffer ToBufferRaw( ByteBuffer buffer = null ) {
			//buffer = buffer ?? new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			//return buffer;
			throw new NotImplementedException();
		}

		public override string Serialize() {
			return new JSONBuilder( new JSONDictionary {
				{ FEE_FIELD_KEY,                Fee },
				{ REGISTRAR_FIELD_KEY,          Registrar },
				{ REFERRER_FIELD_KEY,           Referrer },
				{ REFERRER_PERCENT_FIELD_KEY,   ReferrerPercent },
				{ NAME_FIELD_KEY,               Name },
				{ OWNER_FIELD_KEY,              Owner },
				{ ACTIVE_FIELD_KEY,             Active },
				{ OPTIONS_FIELD_KEY,            Options },
				{ EXTENSIONS_FIELD_KEY,         Extensions }
			} ).Build();
		}

		public static AccountCreateOperationData Create( JObject value ) {
			var token = value.Root;
			var instance = new AccountCreateOperationData();
			instance.Fee = value.TryGetValue( FEE_FIELD_KEY, out token ) ? token.ToObject<AssetData>() : AssetData.EMPTY;
			instance.Registrar = value.TryGetValue( REGISTRAR_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.Referrer = value.TryGetValue( REFERRER_FIELD_KEY, out token ) ? token.ToObject<SpaceTypeId>() : SpaceTypeId.EMPTY;
			instance.ReferrerPercent = value.TryGetValue( REFERRER_PERCENT_FIELD_KEY, out token ) ? token.ToObject<ushort>() : ushort.MinValue;
			instance.Name = value.TryGetValue( NAME_FIELD_KEY, out token ) ? token.ToObject<string>() : string.Empty;
			instance.Owner = value.TryGetValue( OWNER_FIELD_KEY, out token ) ? token.ToObject<AuthorityData>() : null;
			instance.Active = value.TryGetValue( ACTIVE_FIELD_KEY, out token ) ? token.ToObject<AuthorityData>() : null;
			instance.Options = value.TryGetValue( OPTIONS_FIELD_KEY, out token ) ? token.ToObject<AccountOptionsData>() : null;
			instance.Extensions = value.TryGetValue( EXTENSIONS_FIELD_KEY, out token ) ? token.ToObject<object>() : new object();
			return instance;
		}
	}
}