using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SettingsState {

    General,
    Password,
    ApiAccess

}

public class SettingsView : BaseCanvasView {

    List<BaseCanvasView> settingsCanvases = new List<BaseCanvasView>();
    List<SettingsButtonView> tabButtons = new List<SettingsButtonView>();
    SettingsState currentState;

    [SerializeField] SettingsLanguageView settingLanguageView;
    [SerializeField] SettingsPasswordView settingPasswordView;
    [SerializeField] SettingsApiView settingApiView;
    [SerializeField] SettingsButtonView settingLanguageBtn;
    [SerializeField] SettingsButtonView settingPasswordBtn;
    [SerializeField] SettingsButtonView settingApiBtn;
    [SerializeField] Button closeButton;


    public override void Awake() {
        base.Awake();
        settingsCanvases.Add( settingApiView );
        tabButtons.Add( settingApiBtn );
        settingApiBtn.GetComponent<Button>().onClick.AddListener( Settings_OnApiAccessClick );
        settingPasswordView.OnApplyClick += SettingsCanvas_Hide;
        settingPasswordView.OnCancelClick += SettingsCanvas_Hide;
        closeButton.onClick.AddListener( SettingsCanvas_Hide );
        settingApiView.OnCalcelClick += SettingsCanvas_Hide;
    }

    public override void Show() {
        base.Show();
        SwitchSettingCanvas( settingApiView );
    }

    void SettingsCanvas_Hide() {
        UIManager.Instance.CurrentState = UIManager.Instance.PreviousState;
    }

    public SettingsState CurrentState {
        get { return currentState; }
        set {
            if ( currentState != value ) {
                currentState = value;
                Settings_OnStateChange( currentState );
            }
        }
    }

    void Settings_OnStateChange( SettingsState state ) {
        switch ( state ) {
            case SettingsState.General:
                ShowGeneralSettingCanvas();
                return;
            case SettingsState.Password:
                ShowPasswordSettingCanvas();
                return;
            case SettingsState.ApiAccess:
                ShowApiSettingCanvas();
                return;
        }
    }

    private void SwitchSettingCanvas( BaseCanvasView target ) {
        foreach ( var canvas in settingsCanvases ) {
            if ( !canvas.Equals( target ) ) {
                canvas.Hide();
            } else {
                canvas.Show();
            }
        }
    }

    void SwitchButtonState( SettingsButtonView target ) {
        foreach ( var button in tabButtons ) {
            if ( button.Equals( target ) ) {
                button.Currentstate = ButtonState.Pressed;
            } else {
                button.Currentstate = ButtonState.Active;
            }
        }
    }

    void Settings_OnGeneralClick() {
        CurrentState = SettingsState.General;
        SwitchButtonState( settingLanguageBtn );
    }

    void Settings_OnPasswordClick() {
        CurrentState = SettingsState.Password;
        SwitchButtonState( settingPasswordBtn );

    }

    void Settings_OnApiAccessClick() {
        CurrentState = SettingsState.ApiAccess;
        SwitchButtonState( settingApiBtn );
    }

    void ShowGeneralSettingCanvas() {
        SwitchSettingCanvas( settingLanguageView );
    }

    void ShowPasswordSettingCanvas() {
        SwitchSettingCanvas( settingPasswordView );
    }

    void ShowApiSettingCanvas() {
        SwitchSettingCanvas( settingApiView );
    }

}
