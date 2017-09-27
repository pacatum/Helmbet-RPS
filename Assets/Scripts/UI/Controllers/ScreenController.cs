using UnityEngine;

public class ScreenController : SingletonMonoBehaviour<ScreenController> {



    [SerializeField] int minScreenWidth;
    [SerializeField] int minScreenHeight;

    public int MinScreenWidth {
        get { return minScreenWidth; }
    }

    public int MinScreenHeight {
        get { return minScreenHeight; }
    }

    void Update() {
        if ( Screen.width < minScreenWidth ) {
            Screen.SetResolution( minScreenWidth, Screen.height, false );
        }else if ( Screen.height < minScreenHeight ) {
            Screen.SetResolution(Screen.width, minScreenHeight, false);
        }
    }


}
