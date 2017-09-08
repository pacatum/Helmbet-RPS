using System;
using Newtonsoft.Json;
using Tools;


namespace Base.Data {

	public class IdObject : NullableObject, IDisposable, IEquatable<IdObject> {

		[JsonProperty( "id", Order = -2 )]
		public SpaceTypeId Id { get; private set; }                  // "x.x.x"

		[JsonIgnore]
		public SpaceType SpaceType {
			get { return Id.SpaceType; }
		}

		public void Dispose() {
			GC.SuppressFinalize( this );
		}

		public bool Equals( IdObject other ) {
			return !other.IsNull() && Id.Equals( other.Id );
		}
	}
}