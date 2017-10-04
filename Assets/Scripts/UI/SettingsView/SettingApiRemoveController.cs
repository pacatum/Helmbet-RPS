using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingApiRemoveController : MonoBehaviour {

    public event Action<GameObject> OnExpandPanelOpen;
    public event Action OnApiRemoved;

    [SerializeField] GameObject expandPanel;
    [SerializeField] Button removeButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Dropdown dropdown;
    [SerializeField] Button hideButton;
    [SerializeField] Button openExpandPanel;

    private MessagePopupView messagePopupView;


    void Awake() {
        messagePopupView = FindObjectOfType<MessagePopupView>();
        cancelButton.onClick.AddListener( HideExpandPanel );
        hideButton.onClick.AddListener( HideExpandPanel );
        removeButton.onClick.AddListener( RemoveApi );
        openExpandPanel.onClick.AddListener( OpenExpandpanel );
    }

    public void HideExpandPanel() {
        gameObject.SetActive(true);
        expandPanel.SetActive( false );
    }
    
    void RemoveApi() {
        string selectedApi = dropdown.captionText.text;
        if ( NodeManager.Instance.IsDefault( selectedApi ) ) {
            messagePopupView.SerErrorPopup( "CANT REMOVE DEFAULT API!" );
            return;
        }

        if ( NodeManager.Instance.RemoveHost( selectedApi ) ) {
            InitDropdown();
            if ( OnApiRemoved != null ) {
                OnApiRemoved();
            }
        }
    }

    void InitDropdown() {
        dropdown.ClearOptions();
        foreach ( var host in NodeManager.Instance.Hosts ) {
            Dropdown.OptionData option = new Dropdown.OptionData( ( host ) );
            dropdown.AddOptions( new List<Dropdown.OptionData>() { option } );
        }
    }

    void OpenExpandpanel() {
        gameObject.SetActive( false );
        expandPanel.SetActive( true );
        InitDropdown();
        if ( OnExpandPanelOpen != null ) {
            OnExpandPanelOpen(gameObject);
        }
    }

}
