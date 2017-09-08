using System;
using System.Collections.Generic;
using Base.Config;
using Base.Data;
using Base.Data.Tournaments;
using UnityEngine;

public enum CursorState {

	Normal, 
	Hover

}

public class UIController : SingletonMonoBehaviour<UIController> {

    public event Action OnDashboardButton;
    public event Action OnGamesButton;

    List<BaseCanvasView> allCanvases = new List<BaseCanvasView>();

    [Header( "Cursor" )] [SerializeField] Texture2D hoverCursorTexture;


    [SerializeField] HeaderView headerView;
    [SerializeField] DashdoardView dashboardView;
    [SerializeField] SettingsView settingsView;
    [SerializeField] LoginView loginView;
    [SerializeField] CreateNewView createNewView;
    [SerializeField] GameScreenView gameScreenView;
    [SerializeField] ScreenLoader screenLoader;
    [SerializeField] GameHeaderView gameHeaderView;
    [SerializeField] AccountView accountView;
    [SerializeField] TournamentDetailsView tournamentDetailsView;
    [SerializeField] GameInfoView gameInfoView;


    [Header( "Popups" )] [SerializeField] private MessagePopupView messagePopupView;

    public string bufferString;

    protected override void Awake() {
        base.Awake();

        allCanvases.Add( dashboardView );
        allCanvases.Add( settingsView );
        allCanvases.Add( loginView );
        allCanvases.Add( createNewView );
        allCanvases.Add( gameScreenView );
        allCanvases.Add( tournamentDetailsView );
        allCanvases.Add( gameInfoView );

        createNewView.OnGameInfoClick += GameInfo_OnClick;

        headerView.OnDashboardClick += Header_OnDashboardClick;
        headerView.OnGamesClick += GameInfo_OnClick;
        headerView.OnSettingsClick += Header_OnSettingsClick;
        headerView.OnAccountClick += Header_OnAccountClick;

        loginView.OnLoginClick += Header_OnDashboardClick;
        loginView.OnLoginDone += headerView.OnSetLoginUsername;

        dashboardView.OnCreateNewClick += Dashboard_OnCreateNewClick;
        createNewView.OnCancelClick += Header_OnDashboardClick;
        gameHeaderView.OnMinimazeButton += Header_OnDashboardClick;
        accountView.OnLogoutButton += Account_OnLogoutClick;


        UIManager.Instance.OnStateChanged += GlobalManager_Instance_OnStateChanged;
    }

    void Start() {
        GlobalManager_Instance_OnStateChanged( UIManager.ScreenState.Login );
    }

    public void ScreenLoader_OnLoad( bool isLoad ) {
        screenLoader.LoadScreen( isLoad );
    }

    void SwitchCanvas( BaseCanvasView target ) {
        foreach ( var canvas in allCanvases ) {
            if ( !canvas.Equals( target ) ) {
                canvas.Hide();
            } else {
                canvas.Show();
            }
        }
    }

    void GlobalManager_Instance_OnStateChanged( UIManager.ScreenState state ) {
        switch ( state ) {
            case UIManager.ScreenState.Dashboard:
                ShowDashboardCanvas();
                break;
            case UIManager.ScreenState.Settings:
                ShowSettingsView();
                break;
            case UIManager.ScreenState.Login:
                ShowLoginView();
                break;
            case UIManager.ScreenState.CreateNew:
                ShowCreateNewView();
                break;
            case UIManager.ScreenState.Game:
                ShowGameScreenView();
                break;
            case UIManager.ScreenState.Account:
                ShowAccountView();
                break;
            case UIManager.ScreenState.TournamentDetails:
                ShowTournamentDetailsView();
                break;
            case UIManager.ScreenState.GameStartPreview:
                ShowGameScreenView();
                break;
            case UIManager.ScreenState.GameInfo:
                ShowGameInfoView();
                break;
        }
    }


    void Account_OnLogoutClick() {
        AuthorizationManager.Instance.ResetAuthorization();
        dashboardView.Clear();
        UIManager.Instance.ClearHistory();
        accountView.Hide();
    }

    void Header_OnDashboardClick() {
        if ( UIManager.Instance.CurrentState != UIManager.ScreenState.Settings ) {
            UIManager.Instance.CurrentState = UIManager.ScreenState.Dashboard;
        }
    }


    void Header_OnAccountClick() {
        if ( accountView.IsActive ) {
            accountView.Hide();
        } else {
            accountView.Show();
        }
    }

    void Header_OnSettingsClick() {
        UIManager.Instance.CurrentState = UIManager.ScreenState.Settings;
    }

    void Dashboard_OnDepositClick() {
        UIManager.Instance.CurrentState = UIManager.ScreenState.DepositWithdraw;
    }

    void Dashboard_OnCreateNewClick() {
        UIManager.Instance.CurrentState = UIManager.ScreenState.CreateNew;
    }

    void GameInfo_OnClick() {
        UIManager.Instance.CurrentState = UIManager.ScreenState.GameInfo;
        if ( OnGamesButton != null ) {
            OnGamesButton();
        }
    }

    void ShowDashboardCanvas() {
        SwitchCanvas( dashboardView );
        if ( OnDashboardButton != null ) {
            OnDashboardButton();
        }
    }

    void ShowLoginView() {
        SwitchCanvas( loginView );
    }

    void ShowAccountView() {
        SwitchCanvas( accountView );
    }

    void ShowGameInfoView() {
        SwitchCanvas( gameInfoView );
    }

    void ShowSettingsView() {
        settingsView.Show();
        settingsView.CurrentState = SettingsView.SettingsState.General;
    }

    void ShowCreateNewView() {
        SwitchCanvas( createNewView );
        if ( OnDashboardButton != null ) {
            OnDashboardButton();
        }
    }


    void ShowTournamentDetailsView() {
        SwitchCanvas( tournamentDetailsView );
        tournamentDetailsView.ShowTournamentInfo( currenTournamentObject );
    }

    void ShowGameScreenView() {
        SwitchCanvas( gameScreenView );
    }

    public void SwitchCursorState( CursorState state ) {
        switch ( state ) {
            case CursorState.Normal:
                Cursor.SetCursor( null, Vector2.zero, CursorMode.Auto );
                break;
            case CursorState.Hover:
                Cursor.SetCursor( hoverCursorTexture, Vector2.zero, CursorMode.ForceSoftware );
                break;
        }
    }

    private TournamentObject currenTournamentObject;

    public void UpdateTournamentDetails( TournamentObject info ) {
        currenTournamentObject = info;
    }

    public void UpdateTournamentInProgress( TournamentObject info ) {
        TournamentManager.Instance.CurrentTournament = info;
        GameManager.Instance.UpdateMetches( info );
        UIManager.Instance.CurrentState = UIManager.ScreenState.Game;
    }

    public void UpdateStartGamePreview( TournamentObject tournament ) {
        UIManager.Instance.CurrentState = UIManager.ScreenState.GameStartPreview;
        gameScreenView.ShowWaitingGameView( tournament );
    }

    public void HidePopups() {
        messagePopupView.HideAll();
    }

}
