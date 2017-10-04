using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SortType {

    GameId,
    StartTime,
    BuyIn,
    Jackpot, 
    RegisterDeadline,
    Players, 
    Game,
    Winner,
    Result

}

public enum SortOrder {

    Ascending,
    Descending

}

public class DashdoardView : BaseCanvasView {

    public event Action OnCreateNewClick;

    [SerializeField] Button currentTournamentsButton;
    [SerializeField] Button historyTournamentsButton;
    [SerializeField] Button createNewButton;
    [SerializeField] DashboardTabView currentTournamentView;
    [SerializeField] DashboardTabView historyTournamentView;
    
    List<DashboardTabView> views = new List<DashboardTabView>();
    DashboardTabView currentActiveView;


    public override void Awake() {
        base.Awake();
        views.Add( currentTournamentView );
        views.Add( historyTournamentView );

        historyTournamentsButton.onClick.AddListener( OpenHistoryTournaments );
        currentTournamentsButton.onClick.AddListener( OpenCurrentTournaments );

        createNewButton.onClick.AddListener( ShowCreateNewNiew );
        Clear();
    }

    public void Tournamnets_OnChange() {
        if ( CurrentActiveView.gameObject.activeSelf ) {
            CurrentActiveView.ShowTournaments();
        }
    }

    void OpenCurrentTournaments() {
        SwitchTabs( currentTournamentView );
    }

    void OpenHistoryTournaments() {
        SwitchTabs( historyTournamentView );
    }

    void SwitchTabs( DashboardTabView target ) {
        foreach ( var view in views ) {
            if ( view.Equals( target ) ) {
                view.Show();
                currentActiveView = view;
            } else {
                view.Hide();
            }
        }
    }

    private DashboardTabView CurrentActiveView {
        get { return currentActiveView ? currentActiveView : currentTournamentView; }
    }
    
    public override void Show() {
        base.Show();
        SwitchTabs( CurrentActiveView );
    }

    public override void Hide() {
        currentTournamentView.Hide();
        base.Hide();
    }

    void ShowCreateNewNiew() {
        if ( OnCreateNewClick != null ) {
            OnCreateNewClick();
        }
    }

    public void Clear() {
        CurrentActiveView.Clear();
    }


}
