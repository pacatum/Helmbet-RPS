using System;
using System.Collections.Generic;
using Base.Responses;
using Newtonsoft.Json;


namespace Base.Requests {

	public sealed class RequestIdentificator {

		public const int OPEN_ID = -1;
		public const int CLOSE_ID = -2;
		public const int INVALID_ID = 0;

		int currentId;


		public RequestIdentificator() {
			currentId = INVALID_ID;
		}

		public RequestIdentificator( int startId ) {
			currentId = startId;
		}

		public int GenerateNewId() {
			return ++currentId;
		}

		public int OpenId {
			get { return OPEN_ID; }
		}

		public int CloseId {
			get { return CLOSE_ID; }
		}
	}


	public class Request {

		const string METHOD_NAME = "call";

		[JsonProperty( "id" )]
		public int RequestId { get; private set; }
		[JsonProperty( "method" )]
		public string MethodName { get; private set; }
		[JsonProperty( "params" )]
		public Parameters Parameters { get; private set; }
		[JsonIgnore]
		public string Title { get; private set; }
		[JsonIgnore]
		public bool Debug { get; private set; }


		public Request( int requestId, Parameters parameters, string title, bool debug ) {
			MethodName = METHOD_NAME;
			Parameters = parameters ?? new Parameters();
			RequestId = requestId;
			Title = title;
			Debug = debug;
		}

		public override string ToString() {
			return JsonConvert.SerializeObject( this );
		}

		public void PrintLog() {
			Unity.Console.DebugLog( Unity.Console.SetYellowColor( Title ), Unity.Console.SetGreenColor( "--->>>" ), Unity.Console.SetWhiteColor( ToString() ) );
		}
	}


	public sealed class RequestCallback : Request {

		[JsonIgnore]
		public Action<Response> Callback { get; private set; }

		public RequestCallback( int requestId, Parameters parameters, Action<Response> callback, string title, bool debug ) : base( requestId, parameters, title, debug ) {
			Callback = callback;
		}
	}


	public class Parameters : List<object> { }
}