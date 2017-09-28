using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeaderView : BaseCanvasView {

    public event Action OnDashboardClick;
    public event Action OnGamesClick;
    public event Action OnSettingsClick;

    public event Action OnAccountClick;
    public event Action OnNoticeClick;

    [SerializeField] ButtonView settingsBtn;
    [SerializeField] ButtonView dashboardBtn;
    [SerializeField] ButtonView gamesBtn;
    [SerializeField] ButtonView helpButton;
    [SerializeField] ButtonView noticeButton;
    [SerializeField] Button accountButton;


    List<ButtonView> allHeaderButtons = new List<ButtonView>();


    public override void Awake() {
        dashboardBtn.GetComponent<Button>().onClick.AddListener( DashboardBtn_Click );
        gamesBtn.GetComponent<Button>().onClick.AddListener( GamesBtn_Click );
        settingsBtn.GetComponent<Button>().onClick.AddListener( SettingsBtn_Click );
        accountButton.onClick.AddListener( AccountBtn_Click );
        noticeButton.GetComponent<Button>().onClick.AddListener( NoticesBtn_Click );

        allHeaderButtons.Add( gamesBtn );
        allHeaderButtons.Add( dashboardBtn );
        allHeaderButtons.Add( settingsBtn );
       // allHeaderButtons.Add( helpButton );
        allHeaderButtons.Add( noticeButton );

        UIController.Instance.OnDashboardButton += ShowDashboardBtn;
        UIController.Instance.OnGamesButton += ShowGamesBtn;

        OnDashboardClick += ShowDashboardBtn;
        OnSettingsClick += ShowSettingsButton;
        OnGamesClick += ShowGamesBtn;
        
    }

    void Start() {
        SwitchButton( dashboardBtn );
    }

    void SwitchButton( ButtonView target ) {
        foreach ( var button in allHeaderButtons ) {
            if ( button.Equals( target ) ) {
                button.Currentstate = ButtonState.Pressed;
            } else {
                button.Currentstate = ButtonState.Active;
            }
        }
    }

    public void OnSetLoginUsername( string username ) {
        accountButton.GetComponentInChildren<TextMeshProUGUI>().text = Utils.GetFormatedString( username );
    }

    void SettingsBtn_Click() {
        if ( OnSettingsClick != null ) {
            OnSettingsClick();
        }
    }

    void DashboardBtn_Click() {
        if ( OnDashboardClick != null ) {
            OnDashboardClick();
        }
    }

    void GamesBtn_Click() {
        if ( OnGamesClick != null ) {
            OnGamesClick();
        }
    }

    void NoticesBtn_Click() {
        if ( OnNoticeClick != null ) {
            OnNoticeClick();
        }
    }

    void ShowDashboardBtn() {
        SwitchButton( dashboardBtn );
    }

    void ShowGamesBtn() {
        SwitchButton( gamesBtn );
    }

    void ShowSettingsButton() {
        SwitchButton( settingsBtn );
    }

    void AccountBtn_Click() {
        if ( OnAccountClick != null ) {
            OnAccountClick();
        }
    }

}
