using UnityEngine;
using UnityEngine.UI;

public class CreateNewErrorView : MonoBehaviour {

    [SerializeField] int correctlyFilledInMaxCount;
    [SerializeField] GameObject errorPanel;
    [SerializeField] Text settingFieldErrorTitleText;
    [SerializeField] Text settingErrorText;
    [SerializeField] Text correctlyFieldsFilledInText;
    [SerializeField] Text correctlyFilledInMaxCountText;
    [SerializeField] Image bgTargetGraphics;
    [SerializeField] Color allCorrectColor;
    [SerializeField] Color errorColor; 
    [SerializeField] Color allCorrectBgColor;
    [SerializeField] Color errorBgColor;

    int correctlyFilledInCount;


    public void SetErrorMessage( bool active ) {
        errorPanel.SetActive( active );
    }

    public void SetError(string settingViewTitle, string settingViewErrorMessage) {
        settingFieldErrorTitleText.text = settingViewTitle;
        settingErrorText.text = settingViewErrorMessage;
    }

    public int CorrectlyFilledInCount {
        get { return correctlyFilledInCount; }
        set {
            correctlyFilledInCount = value;
            correctlyFieldsFilledInText.text = correctlyFilledInCount.ToString();
            UpdateColor();
        }
    }

    public int CorrectlyFilledInMaxCount {
        get { return correctlyFilledInMaxCount; }
        set {
            correctlyFilledInMaxCount = value;
            correctlyFilledInMaxCountText.text = correctlyFilledInMaxCount.ToString();
        }
    }

    void UpdateColor() {
        if ( correctlyFilledInCount == correctlyFilledInMaxCount ) {
            correctlyFieldsFilledInText.color = allCorrectColor;
            bgTargetGraphics.color = allCorrectBgColor;
        } else {
            correctlyFieldsFilledInText.color = errorColor;
            bgTargetGraphics.color = errorBgColor;
        }
    }

}
