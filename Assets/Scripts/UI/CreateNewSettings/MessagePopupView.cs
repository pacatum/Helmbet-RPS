using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessagePopupView : MonoBehaviour {

    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] TextMeshProUGUI errorMessageText;
    [SerializeField] GameObject successPopup;
    [SerializeField] GameObject errorPopup;
    [SerializeField] Button succesButton;


    void Awake() {
        succesButton.onClick.AddListener( GoToDashboardScreen );
        HideAll();
    }

    public void SetSuccessPopup( string message ) {
        messageText.text = message;
        successPopup.SetActive( true );
    }

    public void SerErrorPopup( string message ) {
        errorMessageText.text = message;
        errorPopup.SetActive( true );
    }

    public void HideAll() {
        successPopup.SetActive(false);
        errorPopup.SetActive(false);
    }

    void GoToDashboardScreen() {
        if ( UIManager.Instance.CurrentState.Equals(UIManager.ScreenState.CreateNew) ) {
            UIManager.Instance.CurrentState = UIManager.ScreenState.Dashboard;
        }
    }
}
