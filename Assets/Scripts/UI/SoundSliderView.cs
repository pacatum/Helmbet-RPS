using UnityEngine;
using UnityEngine.UI;

public class SoundSliderView : MonoBehaviour {

    private Slider slider;

    void Awake() {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener( ChangeVolume );
    }
    
    void ChangeVolume( float volume ) {
        AudioManager.Instance.SetVolume( 1f-volume );
    }
}
