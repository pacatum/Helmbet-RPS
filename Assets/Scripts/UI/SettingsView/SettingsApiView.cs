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
    List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();


    public override void Awake() {
        base.Awake();
        messagePopupView = FindObjectOfType<MessagePopupView>();
        settingApiAddController.OnExpandPanelOpen += ChangeState;
        settingApiRemoveController.OnExpandPanelOpen += ChangeState;
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
        InitDropdown();
    }

    void InitDropdown() {
        apiConnectionDropDown.onValueChanged.RemoveAllListeners();
        apiConnectionDropDown.ClearOptions();
        optionDatas.Clear();
        foreach ( var host in NodeManager.Instance.Urls ) {
            var option = new Dropdown.OptionData( host, host.Equals( NodeManager.Instance.SelecteUrl ) ? currentItemColor : normalItemColor );
            optionDatas.Add( option );

        }
        apiConnectionDropDown.AddOptions( optionDatas );
        apiConnectionDropDown.value = apiConnectionDropDown.options.IndexOf(apiConnectionDropDown.options.Find( option => option.image.Equals( currentItemColor ) ));
        apiConnectionDropDown.onValueChanged.AddListener(delegate (int value) { ChangeApiConnection(value); });
    }

    void ChangeApiConnection( int value ) {
        var selectedApi = apiConnectionDropDown.options[value].text;
        screenLoader.IsLoading = true;
        NodeManager.Instance.ConnectTo( selectedApi, ConnectResultCallback );
    }

    void ConnectResultCallback( NodeManager.ConnectResult result ) {
        screenLoader.IsLoading = false;
        if ( result == NodeManager.ConnectResult.Ok ) {
            prevoiusSelectApi = apiConnectionDropDown.captionText.text;
        } else if ( result == NodeManager.ConnectResult.NoInternet ) {
            messagePopupView.SerErrorPopup( "YOU HAVE NO INTERNET!" );
        } else if ( result == NodeManager.ConnectResult.BadRequest ) {
            messagePopupView.SerErrorPopup( "BAD REQUEST!" );
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
