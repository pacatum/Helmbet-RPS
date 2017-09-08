using System;
using Base.Config;
using Base.Data.Operations;
using Newtonsoft.Json.Linq;
using Tools;


namespace Base.Data.Json {

	public sealed class OperationResultDataPairConverter : JsonCustomConverter<OperationResultData, JArray> {

		protected override OperationResultData Deserialize( JArray value, Type objectType ) {
			if ( value.IsNullOrEmpty() || value.Count != 2 ) {
				return null;
			}
			var type = ( ChainTypes.OperationResult )Convert.ToInt32( value.First );
			switch ( type ) {
			case ChainTypes.OperationResult.Void:
				return VoidOperationResultData.Create( JToken.FromObject( value.Last ) );
			case ChainTypes.OperationResult.SpaceTypeId:
				return SpaceTypeIdOperationResultData.Create( JToken.FromObject( value.Last ) );
			case ChainTypes.OperationResult.Asset:
				return AssetOperationResultData.Create( JToken.FromObject( value.Last ) );
			default:
				Unity.Console.Error( "Unexpected operation result type:", type );
				return null;
			}
		}

		protected override JArray Serialize( OperationResultData value ) {
			if ( value == null ) {
				return new JArray();
			}
			return new JArray( ( int )value.Type, JToken.FromObject( value.Value ) );
		}
	}
}