using System;
using System.Collections;
using Base.Data.Tournaments;
using UnityEngine;
using UnityEngine.UI;
using Base.Config;
using Tools;

public class SceduleTournamentItemView : BaseTournamentItemView {

    public event Action<SceduleTournamentItemView> OnPlayClick;
    public event Action<SceduleTournamentItemView> OnCancelClick;
    public event Action<SceduleTournamentItemView> OnJoinClick;

    [SerializeField] protected Button joinButton;
    [SerializeField] protected Button cancelButton;
    [SerializeField] protected Button toGameButtom;
    [SerializeField] protected UnityEngine.GameObject liveMessageView;

    [SerializeField] Color fullPlayersColor;
    [SerializeField] Color notFullPlayersColor;

    protected void Awake() {
        if ( joinButton != null ) {
            joinButton.onClick.AddListener( JoinBtn_Click );
        }
        if ( cancelButton != null ) {
            cancelButton.onClick.AddListener( CancelBtn_Click );
        }
        if ( toGameButtom != null ) {
            toGameButtom.onClick.AddListener( PlayBtn_Click );
        }
    }

    protected virtual void PlayBtn_Click() {
        if ( currentTournament.State == ChainTypes.TournamentState.InProgress ) {
            ToGame();
        }
    }

    protected virtual void CancelBtn_Click() {
        if ( OnCancelClick != null ) {
            OnCancelClick( this );
        }
    }

    protected virtual void JoinBtn_Click() {
        if ( OnJoinClick != null ) {
            OnJoinClick( this );
        }
    }


    void UpdateActions() {
        var me = AuthorizationManager.Instance.UserData;
        joinButton.gameObject.SetActive( false );
        cancelButton.gameObject.SetActive( false );
        toGameButtom.gameObject.SetActive( false );
        liveMessageView.SetActive( false );

        switch ( currentTournament.State ) {
            case ChainTypes.TournamentState.InProgress:
                if ( !IsPlayerJoined( tournamentDetailsObject ) ) {
                    liveMessageView.SetActive( true );
                } else {
                    ApiManager.Instance.Database.GetMatches( Array.ConvertAll( tournamentDetailsObject.Matches, matche => matche.Id ) )
                        .Then( matches=> {
                                  var matchesInProgress = Array.FindAll( matches, match => match.State == ChainTypes.MatchState.InProgress );
                                  var playerInMatches =Array.FindAll( matchesInProgress, match => match.Players.Contains( me.FullAccount.Account.Id ) );

                                  if ( playerInMatches.Length == 0 ) {
                                      liveMessageView.SetActive( true );
                                  } else {
                                      toGameButtom.gameObject.SetActive( true );
                                  }

                              } );
                }
                break;
            case ChainTypes.TournamentState.AcceptingRegistrations:
                if ( IsPlayerJoined( tournamentDetailsObject ) ) {
                    cancelButton.gameObject.SetActive( true );
                } else if ( currentTournament.RegisteredPlayers < currentTournament.Options.NumberOfPlayers ) {
                    joinButton.gameObject.SetActive( true );
                }
                break;

            case ChainTypes.TournamentState.AwaitingStart:
                if ( tournamentDetailsObject.RegisteredPlayers.Contains( me.FullAccount.Account.Id ) ) {

                    if ( ( currentTournament.StartTime.Value - DateTime.UtcNow ).TotalMinutes <= 2 ) {
                        toGameButtom.gameObject.SetActive( true );
                    } else {
                        TournamentManager.Instance.AddAwaitingStartTournaments( currentTournament );
                    }
                }
                break;

        }
    }


    bool IsPlayerJoined( TournamentDetailsObject details ) {
        return details.RegisteredPlayers.Contains( AuthorizationManager.Instance.Authorization.UserNameData.FullAccount
                                                      .Account
                                                      .Id );
    }

    public override IEnumerator UpdateItem( TournamentObject info) {
        yield return StartCoroutine( base.UpdateItem( info) );
        playerRegisteredText.color = ( info.RegisteredPlayers == info.Options.NumberOfPlayers )
            ? fullPlayersColor
            : notFullPlayersColor;
        UpdateActions();
    }

    public override void UpdateTournament( TournamentObject tournament ) {
        if ( !gameObject.activeInHierarchy || !gameObject.activeSelf ) {
            return;
        }
        StartCoroutine( UpdateItem( tournament) );
    }


}
