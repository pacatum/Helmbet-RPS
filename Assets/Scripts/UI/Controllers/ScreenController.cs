using UnityEngine;


public class ScreenController : MonoBehaviour {

	[SerializeField] int minScreenWidth;
	[SerializeField] int minScreenHeight;


	void Update() {
		if ( Screen.width < minScreenWidth ) {
			Screen.SetResolution( minScreenWidth, Screen.height, false );
		} else if ( Screen.height < minScreenHeight ) {
			Screen.SetResolution( Screen.width, minScreenHeight, false );
		}
	}

	void OnRectTransformDimensionsChange() {
		Debug.LogError( "!" );
	}
}