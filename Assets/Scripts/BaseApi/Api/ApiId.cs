using System;
using Base.Requests;
using Base.Responses;
using Tools;


namespace Base.Api {

	public abstract class ApiId {

		readonly ISender sender;


		protected int? Id { get; private set; }

		protected ApiId( int? id, ISender sender ) {
			this.sender = sender;
			Id = id;
		}

		Action<Response> GenerateRequestCallback<T>( Action<T> resolve, Action<Exception> reject, string responseTitle, bool debug, Action<Response> preProcessorCallback = null, Action<Response> postProcessorCallback = null ) {
			return response => {
				if ( preProcessorCallback != null ) {
					preProcessorCallback( response );
				}
				if ( debug ) {
					response.PrintLog( responseTitle );
				}
				response.SendResultData( resolve, reject );
				if ( postProcessorCallback != null ) {
					postProcessorCallback( response );
				}
			};
		}

		protected void DoRequestVoid( int requestId, Parameters parameters, Action resolve, Action<Exception> reject, string title, bool debug ) {
			DoRequest( requestId, parameters, resolve.IsNull() ? ( Action<object> )null : result => resolve(), reject, title, debug );
		}

		protected void DoRequest<T>( int requestId, Parameters parameters, Action<T> resolve, Action<Exception> reject, string title, bool debug ) {
			sender.Send( (resolve != null || reject != null) ? new RequestCallback( requestId, parameters, GenerateRequestCallback( resolve, reject, title, debug ), title, debug ) : new Request( requestId, parameters, title, debug ) );
		}

		protected int GenerateNewId() {
			return sender.Identificators.GenerateNewId();
		}

		public bool IsInitialized {
			get { return Id.HasValue; }
		}

		protected ApiId Init( int id ) {
			Id = id;
			return this;
		}
	}
}