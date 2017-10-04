using UnityEngine;

public class ResolutionScalingSettings : MonoBehaviour {

    [SerializeField] Canvas canvas;

    [SerializeField] float minimumScreenResolution = 1000f;
    [SerializeField] float middleScreenResolution = 1280f;
    [SerializeField] float maximumScreenResolution = 1600f;
    [SerializeField] float overMaximumScreenResolution = 1800f;

    [SerializeField] float scaleFactorForMinimum = 0.55f;
    [SerializeField] float scaleFactorForMiddle = 0.6f;
    [SerializeField] float scaleFactorForMax = 0.65f;
    [SerializeField] float scaleFactorOverMax = 0.7f;


    private Canvas GetCanvas {
        get { return canvas == null ? canvas = GetComponent<Canvas>() : canvas; }
    }

    void OnRectTransformDimensionsChange() {
        var screenWidth = Screen.width;

        if ( screenWidth >= minimumScreenResolution && screenWidth <= middleScreenResolution ) {
            GetCanvas.scaleFactor = scaleFactorForMinimum;
        } else if ( screenWidth > middleScreenResolution && screenWidth <= maximumScreenResolution ) {
            GetCanvas.scaleFactor = scaleFactorForMiddle;
        } else if ( screenWidth > maximumScreenResolution && screenWidth <= overMaximumScreenResolution ) {
            GetCanvas.scaleFactor = scaleFactorForMax;
        } else if ( screenWidth > overMaximumScreenResolution ) {
            GetCanvas.scaleFactor = scaleFactorOverMax;
        }

    }

}
