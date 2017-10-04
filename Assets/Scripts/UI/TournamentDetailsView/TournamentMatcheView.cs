using System.Collections;
using System.Collections.Generic;
using Base.Config;
using Base.Data.Tournaments;
using Tools;
using UnityEngine;
using UnityEngine.UI;
using GameObject = UnityEngine.GameObject;

public class TournamentMatcheView : MonoBehaviour {

    [SerializeField] TournamentMatchePlayerView firstPlayer;
    [SerializeField] TournamentMatchePlayerView secondPlayer;
    [SerializeField] Sprite lastPassSprite;
    [SerializeField] GameObject passToNextMatche;


    public void SetLast() {
        passToNextMatche.GetComponent<Image>().sprite = lastPassSprite;
    }

    public IEnumerator UpdatePlayers( MatchObject info, MatchObject[] matches ) {
        secondPlayer.CurrentPlayerState = PlayerState.Looser;
        firstPlayer.CurrentPlayerState = PlayerState.Looser;
        if ( info == null ) {
            yield break;
        }

        var accountIds = new List<uint>();
        foreach ( var account in info.Players ) {
            accountIds.Add( account.Id );
        }

        if ( info.State.Equals( ChainTypes.MatchState.WaitingOnPreviousMatches ) && info.Players.Length > 0 ) {
            var previousMatch = new MatchObject();
            var indexOfPreviousMatch = 1;
            for ( int i = 0; i < matches.Length; i++ ) {
                if ( matches[i].State == ChainTypes.MatchState.Complete &&
                     info.Players.Contains( matches[i].MatchWinners[0] ) ) {
                    previousMatch = matches[i];
                    indexOfPreviousMatch = i + 1;
                }
            }

            ApiManager.Instance.Database.GetAccount( previousMatch.MatchWinners[0].Id )
                .Then( winner => {
                    SetPlayersUndefined();
                    if ( ( indexOfPreviousMatch ) % 2 == 0 ) {
                        secondPlayer.SetUsername( winner.Name );
                        secondPlayer.CurrentPlayerState = PlayerState.Winner;
                    } else {
                        firstPlayer.SetUsername( winner.Name );
                        firstPlayer.CurrentPlayerState = PlayerState.Winner;
                    }
                } );
        } else {
            ApiManager.Instance.Database.GetAccounts( accountIds.ToArray() )
                .Then( accountResult => {
                    if ( accountResult.Length > 0 ) {
                        firstPlayer.SetUsername( accountResult[0].Name );
                        secondPlayer.SetUsername( accountResult[1].Name );
                        if ( info.MatchWinners.Length == 0 ) {
                            firstPlayer.CurrentPlayerState = PlayerState.Winner;
                            secondPlayer.CurrentPlayerState = PlayerState.Winner;
                        } else {
                            if ( info.MatchWinners[0].Id.Equals( accountResult[0].Id.Id ) ) {
                                firstPlayer.CurrentPlayerState = PlayerState.Winner;
                                secondPlayer.CurrentPlayerState = PlayerState.Looser;
                            } else {
                                firstPlayer.CurrentPlayerState = PlayerState.Looser;
                                secondPlayer.CurrentPlayerState = PlayerState.Winner;
                            }
                        }
                    } else {
                        SetPlayersUndefined();
                    }
                } );
        }
    }

    public void SetPlayersUndefined() {
        firstPlayer.CurrentPlayerState = PlayerState.Undefined;
        secondPlayer.CurrentPlayerState = PlayerState.Undefined;
    }

}
