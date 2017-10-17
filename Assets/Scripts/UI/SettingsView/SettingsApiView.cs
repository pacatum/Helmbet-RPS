using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsApiView : BaseCanvasView {

    public event Action OnCalcelClick;

    [Header( "Remove Websocket Api" )]
    [SerializeField] SettingApiRemoveController settingApiRemoveController;
    [SerializeField] SettingApiAddController settingApiAddController;
    [SerializeField] Dropdown apiConnectionDropDown;
    [SerializeField] ScreenLoader screenLoader;

    [SerializeField] Sprite currentItemColor;
    [SerializeField] Sprite normalItemColor;

    MessagePopupView messagePopupView;
    string prevoiusSelectApi = "";


    public override void Awake() {
        base.Awake();
        messagePopupView = FindObjectOfType<MessagePopupView>();
        settingApiAddController.OnExpandPanelOpen += ChangeState;
        settingApiRemoveController.OnExpandPanelOpen += ChangeState;
        apiConnectionDropDown.onValueChanged.AddListener( delegate( int value ) { ChangeApiConnection( value ); } );
        settingApiAddController.OnApiAdded += InitDropdown;
        settingApiRemoveController.OnApiRemoved += InitDropdown;
    }

    public override void Show() {
        base.Show();
        settingApiAddController.HideExpandPanel();
        settingApiRemoveController.HideExpandPanel();
        InitDropdown();
        NodeManager.OnSelecteHostChanged += OnDropdown_Init;
    }

    public override void Hide() {
        NodeManager.OnSelecteHostChanged -= OnDropdown_Init;
        base.Hide();
    }

    void OnDropdown_Init( string host ) {
       // Debug.LogError( "On DROPDOWN INIT " +  host);
        InitDropdown();
    }

    List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();

    void InitDropdown() {
        apiConnectionDropDown.ClearOptions();
        optionDatas.Clear();
        //Debug.LogError( "CURRENT HOST: " + NodeManager.Instance.SelecteHost );
        foreach ( var host in NodeManager.Instance.Hosts ) {
            Dropdown.OptionData option = new Dropdown.OptionData( host, host.Equals( NodeManager.Instance.SelecteHost ) ? currentItemColor : normalItemColor );

            if ( !host.Equals( NodeManager.Instance.SelecteHost ) ) {
                optionDatas.Add( option );
            } else {
                optionDatas.Insert( 0, option );
            }
        }

        apiConnectionDropDown.AddOptions( optionDatas );
        if ( prevoiusSelectApi == "" ) {
            prevoiusSelectApi = apiConnectionDropDown.captionText.text;
        }
    }

    void ChangeApiConnection( int value ) {
        var selectedApi = apiConnectionDropDown.options[value].text;
        //Debug.LogError( "selected api: " + selectedApi );
        screenLoader.IsLoading = true;
        NodeManager.Instance.ConnectTo( selectedApi, ConnectResultCallback );
    }

    void ConnectResultCallback( NodeManager.ConnectResult result ) {
        //Debug.LogError("result callback");
        screenLoader.IsLoading = false;
        if ( result == NodeManager.ConnectResult.Ok ) {
            prevoiusSelectApi = apiConnectionDropDown.captionText.text;
        } else if ( result == NodeManager.ConnectResult.NoInternet ) {
            messagePopupView.SerErrorPopup( "YOU HAVE NO INTERNET!" );
        } else if ( result == NodeManager.ConnectResult.BadRequest ) {
            messagePopupView.SerErrorPopup( "BAD REQUEST!" );

            //apiConnectionDropDown.value = 0;
            //ChangeApiConnection( 0 );
        }
    }

    void Cancel_OnClick() {
        if ( OnCalcelClick != null ) {
            OnCalcelClick();
        }
    }

    void ChangeState( GameObject setting ) {
        if ( !settingApiAddController.gameObject.Equals( setting ) ) {
            settingApiAddController.HideExpandPanel();
        } else {
            settingApiRemoveController.HideExpandPanel();
        }
    }

}
