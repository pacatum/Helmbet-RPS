using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingApiAddController : MonoBehaviour {

    public event Action<GameObject> OnExpandPanelOpen;
    public event Action OnApiAdded;

    [SerializeField] GameObject expandPanel;
    [SerializeField] Button addButton;
    [SerializeField] Button cancelButton;
    [SerializeField] InputField input;
    [SerializeField] Button hideButton;
    [SerializeField] Button openExpandPanel;

    private MessagePopupView messagePopupView;


    void Awake() {
        messagePopupView = FindObjectOfType<MessagePopupView>();
        cancelButton.onClick.AddListener( HideExpandPanel );
        hideButton.onClick.AddListener( HideExpandPanel );
        addButton.onClick.AddListener( AddApi );
        openExpandPanel.onClick.AddListener( OpenExpandpanel );
        input.text = string.Empty;
    }

    public void HideExpandPanel() {
        gameObject.SetActive( true );
        input.text = string.Empty;
        expandPanel.SetActive( false );
    }

    void AddApi() {
        if ( input.text == "" ) {
            messagePopupView.SerErrorPopup( "Enter the API!" );
            return;
        }

        if ( !NodeManager.Instance.AddHost( input.text ) ) {
            messagePopupView.SerErrorPopup( "The API is entered incorrectly" );
        } else {
            input.text = string.Empty;
            if ( OnApiAdded != null ) {
                OnApiAdded();
            }
        }
    }

    void OpenExpandpanel() {
        gameObject.SetActive( false );
        expandPanel.SetActive( true );
        if ( OnExpandPanelOpen != null ) {
            OnExpandPanelOpen( gameObject );
        }
    }

}
