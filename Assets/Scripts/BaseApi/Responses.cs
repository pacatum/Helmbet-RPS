using System.Collections.Generic;
using Base.Data.Json;
using Base.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Responses {

	public sealed class Response {

		readonly string rawData;
		readonly JObject jsonObject;


		Response( string rawData ) {
			jsonObject = JObject.Parse( rawData ?? string.Empty );
			this.rawData = rawData;
		}

		public static Response Parse( string data ) {
			return data.IsNullOrEmpty() ? null : new Response( data );
		}

		public void SendResultData<T>( System.Action<T> resolve, System.Action<System.Exception> reject = null, bool isProcessed = true ) {
			if ( resolve != null && IsResult ) {
				resolve.Invoke( jsonObject.ToObject<Result>().GetData<T>() );
			} else
			if ( reject != null && IsError ) {
				reject.Invoke( jsonObject.ToObject<Error>().ToException() );
			}
			IsProcessed = isProcessed;
		}

		public void SendNoticeData( System.Action<JToken[]> callback, bool isProcessed = true ) {
			if ( callback != null && IsNotice ) {
				callback.Invoke( jsonObject.ToObject<Notice>().Results );
			}
			IsProcessed = isProcessed;
		}

		bool IsError {
			get { return Error.IsInstance( jsonObject ); }
		}

		bool IsResult {
			get { return Result.IsInstance( jsonObject ); }
		}

		bool IsNotice {
			get { return Notice.IsInstance( jsonObject ); }
		}

		public int RequestId {
			get {
				if ( IsError ) {
					return jsonObject.ToObject<Error>().ForRequestId;
				}
				if ( IsResult ) {
					return jsonObject.ToObject<Result>().ForRequestId;
				}
				if ( IsNotice ) {
					return jsonObject.ToObject<Notice>().SubscribeId;
				}
				return RequestIdentificator.INVALID_ID;
			}
		}

		public bool IsProcessed { get; private set; }

		public override string ToString() {
			return rawData;
		}

		public void PrintLog( string title ) {
			if ( IsError ) {
				Unity.Console.DebugError( Unity.Console.SetYellowColor( title ), Unity.Console.SetRedColor( "<<<---" ), Unity.Console.SetWhiteColor( ToString() ) );
			} else
			if ( IsResult ) {
				Unity.Console.DebugLog( Unity.Console.SetYellowColor( title ), Unity.Console.SetRedColor( "<<<---" ), Unity.Console.SetWhiteColor( ToString() ) );
			} else
			if ( IsNotice ) {
				Unity.Console.DebugLog( Unity.Console.SetCyanColor( title ), Unity.Console.SetRedColor( "<<<---" ), Unity.Console.SetWhiteColor( ToString() ) );
			}
		}

		public static Response Open( string url ) {
			return new Response( JsonConvert.SerializeObject( Result.Open( url ) ) );
		}

		public static Response Close( string reason ) {
			return new Response( JsonConvert.SerializeObject( Result.Close( reason ) ) );
		}
	}


	public sealed class Result {

		const string ID_FIELD_KEY = "id";
		const string RESULT_FIELD_KEY = "result";

		[JsonProperty( ID_FIELD_KEY )]
		int id;
		[JsonProperty( RESULT_FIELD_KEY )]
		JToken result;


		public int ForRequestId {
			get { return id; }
		}

		public T GetData<T>() {
			return result.ToObject<T>();
		}

		public static Result Open( string url ) {
			return new Result { id = RequestIdentificator.OPEN_ID, result = JToken.FromObject( url ) };
		}

		public static Result Close( string reason ) {
			return new Result { id = RequestIdentificator.CLOSE_ID, result = JToken.FromObject( reason ) };
		}

		public static bool IsInstance( JObject jsonObject ) {
			return new List<JProperty>( jsonObject.Properties() ).Exists( p => RESULT_FIELD_KEY.Equals( p.Name ) );
		}
	}


	public sealed class Notice {

		sealed class ParametersConverter : JsonCustomConverter<Parameters, JArray> {

			protected override Parameters Deserialize( JArray value, System.Type objectType ) {
				if ( value.IsNullOrEmpty() || value.Count != 2 ) {
					return null;
				}
				var id = System.Convert.ToInt32( value.First );
				value = value.Last.ToObject<JArray>();
				if ( !value.IsNullOrEmpty() && value.First.Type.Equals( JTokenType.Array ) ) {
					value = value.First.ToObject<JArray>();
				}
				return new Parameters( id, value.ToObject<JToken[]>() );
			}

			protected override JArray Serialize( Parameters value ) {
				return new JArray( value.Id, value.Results );
			}
		}


		[JsonConverter( typeof( ParametersConverter ) )]
		sealed class Parameters {

			public int Id { get; private set; }
			public JToken[] Results { get; private set; }

			public Parameters( int id, JToken[] results ) {
				Id = id;
				Results = results;
			}
		}


		const string METHOD_FIELD_KEY = "method";
		const string NOTICE_FIELD_KEY = "notice";
		const string PARAMS_FIELD_KEY = "params";

		[JsonProperty( METHOD_FIELD_KEY )]
		string method;
		[JsonProperty( PARAMS_FIELD_KEY )]
		Parameters parameters;


		public int SubscribeId {
			get { return parameters.Id; }
		}

		public JToken[] Results {
			get { return parameters.Results; }
		}

		public static bool IsInstance( JObject jsonObject ) {
			return new List<JProperty>( jsonObject.Properties() ).Exists( p => METHOD_FIELD_KEY.Equals( p.Name ) && NOTICE_FIELD_KEY.Equals( p.Value.ToString() ) );
		}
	}


	public sealed class Error {

		public class WrappedErrorException : System.Exception {

			public Error Error { get; private set; }

			public WrappedErrorException( Error error ) : base( error.ToString() ) {
				Error = error;
			}
		}


		const string ID_FIELD_KEY = "id";
		const string ERROR_FIELD_KEY = "error";
		const string DATA_FIELD_KEY = "data";

		[JsonProperty( ID_FIELD_KEY )]
		int id;
		[JsonProperty( ERROR_FIELD_KEY )]
		JObject data;


		public int ForRequestId {
			get { return id; }
		}

		public WrappedErrorException ToException() {
			return new WrappedErrorException( this );
		}

		public string Data {
			get {
				var token = JToken.FromObject( string.Empty );
				return data.TryGetValue( DATA_FIELD_KEY, out token ) ? token.ToString() : string.Empty;
			}
		}

		public override string ToString() {
			return Data;
		}

		public static bool IsInstance( JObject jsonObject ) {
			return new List<JProperty>( jsonObject.Properties() ).Exists( p => ERROR_FIELD_KEY.Equals( p.Name ) );
		}
	}
}