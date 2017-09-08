using System;
using System.Collections.Generic;
using Base.Config;
using Base.Data.Tournaments;
using Tools;
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
    Winner

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

    [SerializeField] GameDashboardPreview gameDashboardPreviewPrefab;
    [SerializeField] Transform dashboardPreviewContainer;


    List<DashboardTabView> views = new List<DashboardTabView>();
    List<GameDashboardPreview> dashboardPreviews = new List<GameDashboardPreview>();
    DashboardTabView currentActiveView;


    public override void Awake() {
        base.Awake();
        views.Add( currentTournamentView );
        views.Add( historyTournamentView );

        historyTournamentsButton.onClick.AddListener( OpenHistoryTournaments );
        currentTournamentsButton.onClick.AddListener( OpenCurrentTournaments );

        currentTournamentView.GetComponent<CurrentTournamentsListTabView>().OnUpdateTournaments += UpdateGamePreviews;
        TournamentManager.Instance.OnTournamentChanged += UpdateTournament;

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

    void UpdateTournament( TournamentObject tournament ) {
        var gamePreview = Array.Find( dashboardPreviews.ToArray(),
                                     preview => preview.CurrentTournament.Equals( tournament ) );
        if ( gamePreview == null ) {
            return;
        }

        if ( tournament.State.Equals(ChainTypes.TournamentState.Concluded) ) {
            dashboardPreviews.RemoveAt( dashboardPreviews.IndexOf( gamePreview ) );
            Destroy( gamePreview.gameObject );
        }
    }

    void UpdateGamePreviews( List<TournamentObject> tournaments ) {
        var result = Array.FindAll( tournaments.ToArray(),
                                   tournament => tournament.State.Equals( ChainTypes.TournamentState.InProgress) ||
                                                 tournament.State.Equals(ChainTypes.TournamentState.AwaitingStart) && (tournament.StartTime.Value-DateTime.UtcNow).TotalMinutes <= 2);


        ApiManager.Instance.Database.GetTournamentsDetails( Array.ConvertAll( result, detail => detail.Id.Id ) )
            .Then( details
                      => {
                      var myDetails =
                          Array.FindAll( details,
                                        detail => detail.RegisteredPlayers.Contains( AuthorizationManager.Instance
                                                                                      .UserData.FullAccount.Account
                                                                                      .Id ) );

                      
                      var myTournaments = new List<TournamentObject>();
                      foreach ( var detail in myDetails ) {
                          foreach ( var tournament in result ) {
                              if ( detail.Tournament.Equals( tournament.Id ) ) {
                                  myTournaments.Add( tournament );
                              }
                          }
                      }

                      for ( int i = 0; i < myTournaments.Count; i++ ) {
                          var item = ( dashboardPreviews.Count <= i ) ? GetItem() : dashboardPreviews[i];
                          item.gameObject.SetActive( true );
                          item.SetUp( myTournaments[i],
                                     myTournaments[i].State.Equals( ChainTypes.TournamentState.AwaitingStart ) );
                      }
                      for ( int i = dashboardPreviews.Count - 1; i >= myTournaments.Count; i-- ) {
                          Destroy( dashboardPreviews[i].gameObject );
                          dashboardPreviews.RemoveAt( i );
                      }

                  } );
    }

    GameDashboardPreview GetItem() {
        var item = Instantiate( gameDashboardPreviewPrefab );
        item.transform.SetParent( dashboardPreviewContainer, false );
        item.transform.SetAsFirstSibling();
        dashboardPreviews.Add( item );
        return item;
    }

}
