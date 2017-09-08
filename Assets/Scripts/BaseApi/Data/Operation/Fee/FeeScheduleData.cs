using Newtonsoft.Json;
using Tools;


namespace Base.Data.Operations.Fee {

	public class FeeScheduleData : NullableObject {
		[JsonProperty( "parameters" )]
		public FeeParametersData[] Parameters { get; private set; }
		[JsonProperty( "scale" )]
		public uint Scale { get; private set; }
	}
}