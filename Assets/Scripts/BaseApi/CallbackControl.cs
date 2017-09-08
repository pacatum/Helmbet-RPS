using System;
using System.Collections.Generic;
using Base.Requests;
using Base.Responses;


namespace Base.Eventing {

	public sealed class CallbackControl : IDisposable {

		readonly Dictionary<int, Action<Response>> requestCallbacks;
		readonly Dictionary<int, Action<Response>> regularCallbacks;


		public CallbackControl() {
			requestCallbacks = new Dictionary<int, Action<Response>>();
			regularCallbacks = new Dictionary<int, Action<Response>>();
		}

		public void AddRegularCallback( int eventId, Action<Response> action ) {
			regularCallbacks[ eventId ] = action;
		}

		public void RemoveRegularCallback( int eventId ) {
			regularCallbacks.Remove( eventId );
		}

		public void SetRequestCallback( RequestCallback request ) {
			requestCallbacks[ request.RequestId ] = request.Callback;
		}

		public void ResetRequestCallback( int eventId ) {
			requestCallbacks.Remove( eventId );
		}

		public void Clear() {
			regularCallbacks.Clear();
			requestCallbacks.Clear();
		}

		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		void Dispose( bool disposing ) {
			Clear();
		}

		public bool InvokeCallback( Response response ) {
			if ( regularCallbacks.ContainsKey( response.RequestId ) ) {
				regularCallbacks[ response.RequestId ].Invoke( response );
				return true;
			}
			if ( requestCallbacks.ContainsKey( response.RequestId ) ) {
				requestCallbacks[ response.RequestId ].Invoke( response );
				if ( response.IsProcessed ) {
					ResetRequestCallback( response.RequestId );
				}
				return true;
			}
			return false;
		}
	}
}