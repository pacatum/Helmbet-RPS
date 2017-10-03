using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScreenResolution : MonoBehaviour {

    [SerializeField] GameObject minScreenGameInfoView;
    [SerializeField] GameObject maxScreenGameInfoView;

    [SerializeField] RectTransform settingsRectTransform;

    [SerializeField] float minScreenSettingsTop;
    [SerializeField] float maxScreenSettingsTop;

    [SerializeField] Transform scalingTransform;
    [SerializeField] float minScreenScale;
    [SerializeField] float maxScreenScale;
    [SerializeField] float minScreenSettingsLeft;
    [SerializeField] float maxScreenSettingsLeft;

    void OnRectTransformDimensionsChange() {
        if ( Screen.height <= 800 || Screen.width <= 1000 ) {
            SetMinScreenView();
        } else {
            SetMaxScreenView();
        }
    }

    void SetMinScreenView() {
        minScreenGameInfoView.SetActive(true);
        maxScreenGameInfoView.SetActive(false);
        settingsRectTransform.offsetMax = new Vector2(0f, minScreenSettingsTop);
        scalingTransform.localScale = new Vector3(minScreenScale, minScreenScale);
        scalingTransform.GetComponent<RectTransform>().offsetMax = new Vector2(minScreenSettingsLeft, 0f);
    }

    void SetMaxScreenView() {
        minScreenGameInfoView.SetActive(false);
        maxScreenGameInfoView.SetActive(true);
        settingsRectTransform.offsetMax = new Vector2(0f, maxScreenSettingsTop);
        scalingTransform.localScale = new Vector3(maxScreenScale, maxScreenScale);
        scalingTransform.GetComponent<RectTransform>().offsetMax = new Vector2(maxScreenSettingsLeft, 0f);
    }

}
