using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Base.Config;
using Base.Data;
using Base.Data.Accounts;
using Base.Data.Assets;
using Base.Data.Tournaments;
using Promises;
using Tools;
using UnityEngine;


public class TournamentManager : SingletonMonoBehaviour<TournamentManager> {

    public event Action<TournamentObject> OnTournamentChanged;

    Dictionary<uint, AssetObject> cacheAsset = new Dictionary<uint, AssetObject>();
    List<TournamentObject> awaitingStartTournaments = new List<TournamentObject>();
    public TournamentObject CurrentTournament { get; set; }


    protected override void Awake() {
        base.Awake();
        Repository.OnObjectUpdate -= UpdateTournamentsHandler;
        Repository.OnObjectUpdate += UpdateTournamentsHandler;
        StartCoroutine( UpdateAwaitingStartTournaments() );
    }

    public void AddAwaitingStartTournaments( TournamentObject tournament ) {
        if ( awaitingStartTournaments.Contains( tournament ) || ( ( tournament.StartTime.Value - DateTime.UtcNow ).TotalMinutes <= 2.0 ) ) {
            return;
        }
        awaitingStartTournaments.Add( tournament );
    }

    IEnumerator UpdateAwaitingStartTournaments() {
        while ( true ) {
            foreach ( var tournament in awaitingStartTournaments ) {
                if ( ( tournament.StartTime.Value - DateTime.UtcNow ).TotalMinutes <= 2.0 ) {
                    UpdateTournaments( tournament );
                }
            }
            yield return new WaitForSecondsRealtime( 1f );
        }
    }

    void UpdateTournamentsHandler( IdObject idObject ) {
        if ( idObject.SpaceType.Equals( SpaceType.Tournament ) ) {
            UpdateTournaments( idObject as TournamentObject );
        }
    }

    void UpdateTournaments( TournamentObject tournament ) {
        if ( OnTournamentChanged != null ) {
            OnTournamentChanged( tournament );
        }
        ApiManager.Instance.Database.GetTournamentDetails( tournament.Id.Id )
            .Then( tournamentDetails => {
                var me = AuthorizationManager.Instance.UserData.FullAccount.Account;
                if ( !AuthorizationManager.Instance.IsAuthorized ||
                     !tournamentDetails.RegisteredPlayers.Contains( me.Id ) ) {
                    return;
                }

                if ( tournament.State.Equals( ChainTypes.TournamentState.AwaitingStart ) ) {
                    if ( tournament.StartTime.HasValue &&
                         ( ( tournament.StartTime.Value - DateTime.UtcNow ).TotalMinutes <=
                           2.0 ) || tournament.Options.StartDelay.HasValue ) {
                        awaitingStartTournaments.Remove( tournament );
                        UIController.Instance.HidePopups();
                        UIController.Instance.UpdateStartGamePreview( tournament );
                    }
                }

                if ( tournament.State.Equals( ChainTypes.TournamentState.InProgress ) ) {
                    ApiManager.Instance.Database.GetMatches( Array.ConvertAll( tournamentDetails.Matches, matche => matche.Id ) )
                        .Then( matches => {
                            var myMatches = Array.Find( matches, match => match.State.Equals( ChainTypes.MatchState.InProgress ) && match.Players.Contains( me.Id ) );
                            if ( myMatches != null && PlayerPrefs.GetInt( tournament.Id.Id.ToString() ) == 0 ) {
                                PlayerPrefs.SetInt( tournament.Id.ToString(), 1 );
                                if ( UIManager.Instance.CurrentState != UIManager.ScreenState.Game ) {
                                    UIController.Instance.UpdateTournamentInProgress( tournament );
                                }
                            }
                        } );
                }
            } );
    }


    public IPromise<TournamentObject> LoadTournament( uint id ) {
        return ApiManager.Instance.Database.GetTournament( id );
    }

    public IPromise<TournamentObject[]> LoadTournamentsInState( ChainTypes.TournamentState state, uint maxCount ) {
        return ApiManager.Instance.Database.GetTournamentsInState( state, maxCount );
    }

    public IPromise<TournamentDetailsObject> GetDetailsTournamentObject( uint tournamentId ) {
        return ApiManager.Instance.Database.GetTournamentDetails( tournamentId );
    }

    public IPromise<AssetObject> GetAssetObject( uint assetId = 0 ) {
        return new Promise<AssetObject>( ( resolve, reject ) => {
            if ( !cacheAsset.ContainsKey( assetId ) ) {
                ApiManager.Instance.Database.GetAsset( assetId )
                    .Then( result => {
                        cacheAsset.Add( assetId, result );
                        resolve( result );
                    } );
            } else {
                resolve( cacheAsset[assetId] );
            }
        } );
    }

    public IPromise<AssetObject[]> GetAssetsObject( uint[] assetIds ) {
        return ApiManager.Instance.Database.GetAssets( assetIds );
    }

    public IPromise<MatchObject[]> GetMatchesPromise( uint tournamentId ) {
        return new Promise<MatchObject[]>( ( resolve, exeption ) => ApiManager.Instance.Database.GetTournamentDetails( tournamentId )
                                              .Then( details => {
                                                  ApiManager.Instance.Database.GetMatches( Array.ConvertAll( details.Matches, matche => matche.Id ) ).Then( resolve );
                                              } ) );
    }

    public IPromise<TournamentDetailsObject[]> GetDetailsTournamentsObject( uint[] tournamentIds ) {
        return ApiManager.Instance.Database.GetTournamentsDetails( tournamentIds );
    }

    public IEnumerator GetTournamentDetailsObject( uint tournamentId, List<TournamentDetailsObject> result ) {
        List<TournamentDetailsObject> tournamentDetailList = null;
        ApiManager.Instance.Database.GetTournamentDetails( tournamentId )
            .Then( details => ( tournamentDetailList = new List<TournamentDetailsObject>() ).Add( details ) )
            .Catch( exeption => tournamentDetailList = new List<TournamentDetailsObject>() );
        while ( tournamentDetailList == null ) {
            yield return null;
        }
        result.AddRange( tournamentDetailList );
    }

    public IEnumerator GetMatcheObjects( uint[] matchesId, List<MatchObject> result ) {
        List<MatchObject> matchList = null;
        ApiManager.Instance.Database.GetMatches( matchesId )
            .Then( matches => ( matchList = new List<MatchObject>() ).AddRange( matches ) )
            .Catch( exeption => matchList = new List<MatchObject>() );
        while ( matchList == null ) {
            yield return null;
        }
        result.AddRange( matchList );
    }

    public IEnumerator GetAccount( SpaceTypeId accountId, List<AccountObject> accountResult ) {
        List<AccountObject> account = null;
        ApiManager.Instance.Database.GetAccount( accountId.Id )
            .Then( result => {
                account = new List<AccountObject>();
                account.Add( result );
            } )
            .Catch( exeption => account = new List<AccountObject>() );

        while ( account == null ) {
            yield return null;
        }

        if ( account.Count > 0 ) {
            accountResult.AddRange( account );
        }

    }

    public IEnumerator GetMatcheWinnerAccountsObjects( TournamentDetailsObject details, List<AccountObject> accountWinners ) {
        List<AccountObject> loadedAccounts = null;
        ApiManager.Instance.Database.GetMatche( details.Matches[details.Matches.Length - 1].Id )
            .Then( matchResult => {
                ApiManager.Instance.Database.GetAccounts( Array.ConvertAll( matchResult.MatchWinners, winner => winner.Id ) )
                    .Then( accountResult => loadedAccounts = new List<AccountObject>( accountResult ) )
                    .Catch( exception => loadedAccounts = new List<AccountObject>() );
            } )
            .Catch( exception => loadedAccounts = new List<AccountObject>() );

        while ( loadedAccounts == null ) {
            yield return null;
        }

        if ( loadedAccounts.Count > 0 ) {
            accountWinners.AddRange( loadedAccounts );
        }
    }

}