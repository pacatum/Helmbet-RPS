using System.Collections.Generic;
using Tools;
using Unity;


namespace Base.Config {

	public static class ChainConfig {

		class NetworkParameters {

			public string CoreAsset { get; private set; }
			public string AddressPrefix { get; private set; }
			public string ChainId { get; private set; }

			public NetworkParameters( string coreAsset, string addressPrefix, string chainId ) {
				CoreAsset = coreAsset;
				AddressPrefix = addressPrefix;
				ChainId = chainId;
			}
		}


		static Dictionary<string, NetworkParameters> networks = new Dictionary<string, NetworkParameters> {
			{ "BitShares",		new NetworkParameters( "BTS",   "BTS",  "4018d7844c78f6a6c41c6a552b898022310fc5dec06da467ee7905a8dad512c8" ) },
			{ "Muse",			new NetworkParameters( "MUSE",  "MUSE", "45ad2d3f9ef92a49b55c2227eb06123f613bb35dd08bd876f2aea21925a67a67" ) },
			{ "Test",			new NetworkParameters( "TEST",  "TEST", "39f5e2ede1f8bc1a3a54a7914414e3779e33193f1f5693510e73cb7a87617447" ) },
			{ "Obelisk",		new NetworkParameters( "GOV",   "FEW",  "1cfde7c388b9e8ac06462d68aadbd966b58f88797637d9af805b4560b0e9661e" ) },
			{ "Base",			new NetworkParameters( "TEST",  "TEST", "f34fcd74a151676cfce5121f37c4c3257821cd6754239a3f9c63023018cdd710" ) },
			{ "BaseDev",		new NetworkParameters( "TEST",  "TEST", "f34fcd74a151676cfce5121f37c4c3257821cd6754239a3f9c63023018cdd710" ) },
			{ "PPY2T",			new NetworkParameters( "PPY2T", "PPY",  "ecbdc74d20aa1d4bab31b7c2152f505cf0c732e69c55643319cd8e41960f43df" ) },
			{ "PPYTEST",		new NetworkParameters( "PPYTEST", "PPYTEST",  "be6b79295e728406cbb7494bcb626e62ad278fa4018699cf8f75739f4c1a81fd" ) }
		};

		static string coreAsset = "CORE";
		static string addressPrefix = "GPH";
		static double expireInSeconds = 15.0;
		static double expireInSecondsProposal = 24.0 * 60.0 * 60.0;


		public static string CoreAsset {
			get { return coreAsset; }
		}

		public static string AddressPrefix {
			get { return addressPrefix; }
		}

		public static double ExpireInSeconds {
			get { return expireInSeconds; }
		}

		public static double ExpireInSecondsProposal {
			get { return expireInSecondsProposal; }
		}

		public static void SetChainId( string chainId ) {
			chainId = chainId.OrEmpty();
			var keys = new List<string>( networks.Keys );
			for ( var i = 0; i < keys.Count; i++ ) {
				var name = keys[ i ];
				var network = networks[ name ];
				if ( network.ChainId.Equals( chainId ) ) {
					coreAsset = network.CoreAsset;
					addressPrefix = network.AddressPrefix;
					Console.DebugLog( Console.SetWhiteColor( "Address prefix:", addressPrefix ) );
					return;
				}
			}
			Console.DebugError( Console.SetRedColor( "Unknown chain id:", chainId ) );
		}

		public static void Reset() {
			coreAsset = "CORE";
			addressPrefix = "GPH";
			expireInSeconds = 15.0;
			expireInSecondsProposal = 24.0 * 60.0 * 60.0;
		}

		public static void SetPrefix( string prefix = "GPH" ) {
			addressPrefix = prefix;
		}
	}
}