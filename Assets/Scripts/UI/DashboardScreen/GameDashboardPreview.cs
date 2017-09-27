using System;
using System.Diagnostics;
using Base.Config;
using Base.Data.Tournaments;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class GameDashboardPreview : MonoBehaviour {

    [SerializeField] Text tournamentIdText;
    [SerializeField] Text tournamentNumberOfPlayersText;
    [SerializeField] Text tournamentStartTimeText;
    [SerializeField] Button toGameButton;

    TournamentObject currentTournament;
    private bool tournamentAwaitingStart;


    public TournamentObject CurrentTournament {
        get { return currentTournament; }
    }

    void Awake() {
        toGameButton.onClick.AddListener( BackToGame );
    }

    public void SetUp( TournamentObject tournament, bool awaitingStart ) {
        tournamentAwaitingStart = awaitingStart;
        currentTournament = tournament;

        tournamentIdText.text = "TOURNAMENT №" + tournament.Id;
        tournamentNumberOfPlayersText.text = "NUMBER OF PLAYERS: " + tournament.Options.NumberOfPlayers;
        UpdateTournamentState( tournament.State );
    }

    void Update() {
        if ( tournamentAwaitingStart ) {
            var timeSpan = currentTournament.StartTime.Value - DateTime.UtcNow;
            tournamentStartTimeText.text = "STARTS IN " + String.Format( "{0:00}", timeSpan.Minutes ) + ":" + String.Format( "{0:00}", timeSpan.Seconds );
        }
    }

    void BackToGame() {
        if ( currentTournament.State == ChainTypes.TournamentState.AwaitingStart ) {
            UIController.Instance.UpdateStartGamePreview( currentTournament );
        } else {
            UIController.Instance.UpdateTournamentInProgress( currentTournament );
        }
    }

    void UpdateTournamentState( ChainTypes.TournamentState state ) {
        switch ( state ) {
            case ChainTypes.TournamentState.InProgress:
                TournamentManager.Instance.GetDetailsTournamentObject( currentTournament.Id.Id )
                    .Then( details => {
                        ApiManager.Instance.Database
                            .GetMatches( Array.ConvertAll( details.Matches, match => match.Id ) )
                            .Then( matches
                                      => {
                                      var me = AuthorizationManager.Instance.UserData.FullAccount;
                                      var matcheInProgress =Array.Find( matches,match => match.Players.Contains( me.Account.Id ) && match.State.Equals(ChainTypes.MatchState.InProgress) );
                                      var matcheAwatingPrevious =Array.Find( matches,match => match.Players.Contains( me.Account.Id ) && match.State.Equals(ChainTypes.MatchState.WaitingOnPreviousMatches ));

                                      if ( !matcheInProgress.IsNull() ) {
                                          tournamentStartTimeText.text = "IN PROGRESS";
                                          toGameButton.gameObject.SetActive( true );
                                      } else if ( !matcheAwatingPrevious.IsNull() ) {
                                          tournamentStartTimeText.text = "AWAITING PREVIOUS MATCHES";
                                          toGameButton.gameObject.SetActive( false );
                                      }else if ( matcheInProgress.IsNull() && matcheAwatingPrevious.IsNull() ) {
                                          gameObject.SetActive( false );
                                      }

                                  } );
                    } );
                break;
        }
    }

}
