using Base.Requests;


namespace Base.Api {

	public interface ISender {

		void Send( Request request );

		RequestIdentificator Identificators { get; }
	}
}