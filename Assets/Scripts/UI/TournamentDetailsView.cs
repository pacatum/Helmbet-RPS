using System.Collections.Generic;
using Base;
using Base.Config;
using Base.Data;
using Base.Data.Tournaments;
using UnityEngine;
using UnityEngine.UI;
using System;
using Tools;

public class TournamentDetailsView : BaseCanvasView {

    [SerializeField] SceduleTournamentItemView tournamentItemView;
    [SerializeField] HistoryTournamentItemView historyTournamentItem;
    [SerializeField]  UnityEngine.GameObject infoPanel;
    [SerializeField]  Button closeButton;
    [SerializeField]  UnityEngine.GameObject detailsView;

    [SerializeField] List<TournamentRoundView> roundItemViews = new List<TournamentRoundView>();
    List<TournamentMatcheView> tournamentMatcheViews = new List<TournamentMatcheView>();
    TournamentObject currenTournament;


    void ResetTournemanets() {
        foreach ( var item in tournamentMatcheViews ) {
            Destroy( item.gameObject );
        }
        tournamentMatcheViews.Clear();
    }

   public override void Awake() {
        base.Awake();
        closeButton.onClick.AddListener( CloseButton );
    }

    void UpdateTournamentsHandler( IdObject idObject ) {
        if ( idObject.SpaceType.Equals(SpaceType.Tournament) ) {
            var tournament = idObject as TournamentObject;
            if ( currenTournament.Id.Equals( tournament.Id ) && tournament.State.Equals(ChainTypes.TournamentState.Concluded)) {
                tournamentItemView.gameObject.SetActive(false);
                historyTournamentItem.gameObject.SetActive(true);
                historyTournamentItem.UpdateDetails(tournament);
                
            }
        }

        if ( !idObject.SpaceType.Equals(SpaceType.Match) ) {
            return;
        }

        var match = new MatchObject();
        if ( idObject.SpaceType.Equals(SpaceType.Match) ) {
            match = idObject as MatchObject;
        }

        if ( match.IsNull() || !match.Tournament.Equals( currenTournament.Id ) ) {
            return;
        }

        if ( match.State.Equals( ChainTypes.MatchState.Complete ) && infoPanel.activeSelf &&
             currenTournament.State.Equals(ChainTypes.TournamentState.InProgress) ) {
            ResetTournemanets();
            ShowTournamentInfo( currenTournament);
        }
    }

    public override void Show() {
        base.Show();
        Repository.OnObjectUpdate += UpdateTournamentsHandler;
    }

    void UpdateTournament( TournamentObject info ) {
        
        if ( info.State.Equals(ChainTypes.TournamentState.InProgress) ) {
            tournamentItemView.gameObject.SetActive( true );
            historyTournamentItem.gameObject.SetActive( false );
            tournamentItemView.UpdateDetails( info );
        } else if ( info.State.Equals(ChainTypes.TournamentState.Concluded) ) {
            tournamentItemView.gameObject.SetActive( false );
            historyTournamentItem.gameObject.SetActive( true );
            historyTournamentItem.UpdateDetails( info );
        }
    }

    public void ShowTournamentInfo( TournamentObject info ) {
        if ( info.IsNull() ) {
            return;
        }
        detailsView.SetActive( false );
        currenTournament = info;
        infoPanel.SetActive( true );
        UpdateTournament( currenTournament );
        TournamentManager.Instance.GetDetailsTournamentObject( info.Id.Id )
            .Then( detailResult => {
                ApiManager.Instance.Database.GetMatches( Array.ConvertAll( detailResult.Matches, matche => matche.Id ) )
                    .Then( matchesResult => {
                        var playersCount = detailResult.RegisteredPlayers.Length;
                        var matchesCount = playersCount - 1;

                        var roundsCount = 0;
                        var tempMatchesCount = playersCount;
                        if ( matchesCount > 1 ) {
                            while ( tempMatchesCount / 2 > 0 ) {
                                roundsCount++;
                                tempMatchesCount /= 2;
                            }
                        } else {
                            roundsCount = 1;
                        }
                        UpdateRoundsView( roundsCount, playersCount );
                        detailsView.SetActive( true );
                        for ( int i = 0; i < matchesResult.Length; i++ ) {
                            StartCoroutine( tournamentMatcheViews[i].UpdatePlayers( matchesResult[i], matchesResult ) );
                        }
                    } );
            } );


    }


    void UpdateRoundsView( int rounds, int players ) {
        var playersCount = players;
        for ( int i = 0; i < roundItemViews.Count; i++ ) {
            roundItemViews[i].ClearRoundTitle();
            if ( i  <= rounds-1 ) {
                roundItemViews[i].SetRoundTitle( rounds-i );
                var matcheItem =  roundItemViews[i].UpdateMatches( playersCount, i + 1 );
                tournamentMatcheViews.AddRange( matcheItem );
                playersCount /= 2;
            }
        }
        tournamentMatcheViews[tournamentMatcheViews.Count-1].SetLast();
    }

    void CloseButton() {
        UIManager.Instance.CurrentState = UIManager.Instance.PreviousState;
    }

    public override void Hide() {
        Repository.OnObjectUpdate -= UpdateTournamentsHandler;
        ResetTournemanets();
        infoPanel.SetActive( false );
        base.Hide();
    }

}
