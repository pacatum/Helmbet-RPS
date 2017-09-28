using System;
using System.Collections;
using System.Collections.Generic;
using Base.Config;
using Base.Data;
using Base.Data.Accounts;
using Base.Data.Assets;
using Base.Data.Tournaments;
using Promises;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class FilterGamesController : SingletonMonoBehaviour<FilterGamesController> {

    public event Action OnFilterChange;

    [SerializeField] FilterItemView participate;
    [SerializeField] FilterItemView game;
    [SerializeField] FilterItemView players;
    [SerializeField] FilterItemView currency;
    [SerializeField] FilterItemView maxBuyIn;

    [SerializeField] Button applyButton;
    [SerializeField] Button restoreButton;
    

    private List<FilterItemView> items = new List<FilterItemView>();
    DashdoardView dashboard;

    protected override void Awake() {
        base.Awake();
        dashboard = FindObjectOfType<DashdoardView>();
        OnFilterChange += dashboard.Tournamnets_OnChange;
        applyButton.onClick.AddListener( Filter_OnChange );
        restoreButton.onClick.AddListener( FilterRestoreValuesToDefault );

        items.Add( participate );
        items.Add( game );
        items.Add( players );
        items.Add( currency );
        items.Add( maxBuyIn );

        foreach ( var item in items ) {
            item.OnItemClick += SwitchItemView;
        }
        FilterRestoreValuesToDefault();
    }

    void Start() {
        SwitchItemView( null );
        SetApplyButton( false );
    }

    public void SwitchItemView( FilterItemView target ) {
        foreach ( var item in items ) {
            if ( item.Equals( target ) ) {
                item.ShowExpandItem();
            } else {
                item.ShowCloseItem();
            }
        }
    }

    public IPromise<List<TournamentObject>> FilterTournaments(List<TournamentObject> tournaments, string searchFilter ) {
        var tournamentIds = new List<uint>();
        var tournamentAssetIds = new List<uint>();

        foreach ( var tournament in tournaments ) {
            tournamentIds.Add( tournament.Id.Id );
            tournamentAssetIds.Add( tournament.Options.BuyIn.Asset.Id );
        }

        return new Promise<List<TournamentObject>>( ( resolve, reject ) => {
            Promise<IdObject[]>.All(
                                    TournamentManager.Instance.GetAssetsObject( tournamentAssetIds.ToArray() )
                                        .Then<IdObject[]>( assets => assets ),
                                    TournamentManager.Instance.GetDetailsTournamentsObject( tournamentIds.ToArray() )
                                        .Then<IdObject[]>( details => details )
                                   )
                .Then( results => {
                    var list = new List<IdObject[]>( results ).ToArray();

                    var assets = list[0] as AssetObject[];
                    var details = list[1] as TournamentDetailsObject[];
                    
                    if ( tournaments[0].State == ChainTypes.TournamentState.Concluded ) {
                        ApiManager.Instance.Database
                            .GetMatches( Array.ConvertAll( details,
                                                          matchId => matchId.Matches[matchId.Matches.Length - 1].Id ) )
                            .Then( lastMatches
                                      => {
                                      ApiManager.Instance.Database
                                          .GetAccounts( Array.ConvertAll( lastMatches,
                                                                         winner => winner.MatchWinners[0].Id ) )
                                          .Then( winnerAccounts
                                                    => {
                                                    var result = FilterTournaments( tournaments, assets, searchFilter,
                                                                                   details,
                                                                                   winnerAccounts );
                                                    resolve(result );
                                                } )
                                          .Catch( exception => resolve( FilterTournaments( tournaments, assets,
                                                                                          searchFilter,
                                                                                          details,
                                                                                          new AccountObject[details
                                                                                              .Length] ) ) );
                                  } );
                    } else {

                        var myTournaments = new List<TournamentObject>();
                        var otherTournaments = new List<TournamentObject>();
                        var me = AuthorizationManager.Instance.UserData;

                        for ( int i = 0; i < tournaments.Count; i++ ) {
                            if ( details[i].RegisteredPlayers.Contains( me.FullAccount.Account.Id ) ) {
                                myTournaments.Add( tournaments[i] );
                            } else {
                                otherTournaments.Add( tournaments[i] );
                            }
                        }

                        var resultList = new List<TournamentObject>();
                        resultList.AddRange( myTournaments );
                        resultList.AddRange(otherTournaments);

                        var result = FilterTournaments( resultList, assets, searchFilter,
                                                       details,
                                                       new AccountObject[details.Length] );
                        resolve( result );
                    }

                } );
        } );
    }


    List<TournamentObject> FilterTournaments( List<TournamentObject> tournaments, AssetObject[] assets,
                                              string searchFilter, TournamentDetailsObject[] details,
                                              AccountObject[] accounts ) {
        var resultList = new List<TournamentObject>();

        for ( int i = 0; i < tournaments.Count; i++ ) {

            var buyIn =
                tournaments[i].Options.BuyIn.Amount / Mathf.Pow( 10, assets[i].Precision ) <=
                Convert.ToDouble( maxBuyIn.SelectChoise );

            var numberOfPlayers = tournaments[i].Options.NumberOfPlayers >=
                                  Convert.ToUInt32( players.SelectChoise ) &&
                                  tournaments[i].Options.NumberOfPlayers <=
                                  Convert.ToUInt32( players.EndRangeSelectChoise );

            var symbol = assets[i].Symbol.Equals( currency.SelectChoise ) ||
                         currency.SelectChoise.Equals( "ANY" );

            var gameName = game.SelectChoise.Equals( "ANY" ) ||
                           game.SelectChoise.Equals("ROCK, PAPER, SCISSORS");
            

            var search = IsTournamentContains( tournaments[i], assets[i], searchFilter, accounts[i]==null? null : accounts[i] );
            

            if ( buyIn && numberOfPlayers && symbol && gameName && search ) {

                if ( participate.SelectChoise.Equals( "ONLY ME" ) ) {
                    if ( IsPlayerJoined( details[i] ) ) {
                        resultList.Add( tournaments[i] );
                    }
                } else {
                    resultList.Add( tournaments[i] );
                }
            }
        }
        return resultList;
    }

    bool IsTournamentContains( TournamentObject tournament, AssetObject asset,
                               string searchText, AccountObject account ) {

        var search = searchText.ToLower();
        var tournamentId = tournament.Id.ToString().ToLower().Contains( search );
        var buyIn = ( ( tournament.Options.BuyIn.Amount / Math.Pow( 10, asset.Precision ) ) + asset.Symbol ).ToLower().Contains( search );
        var jackpot = ( ( tournament.Options.BuyIn.Amount / Math.Pow( 10, asset.Precision ) * tournament.Options.NumberOfPlayers ) + asset.Symbol ).ToLower().Contains( search );
        var time = tournament.Options.RegistrationDeadline - DateTime.UtcNow;
        var registerDeadline = false;
        if (time.TotalMinutes < 60)
        {
            registerDeadline = ("Register: <" + (int)time.TotalMinutes + "m").Contains(searchText);
        }
        else if ( time.TotalHours > 0 ) {
            registerDeadline = ( "Register: ~" + (int) time.TotalHours + "h" ).Contains( searchText );
        }
        //var startTime = ( "Start: " + Convert.ToDateTime( time.ToString() ).ToString( "hh:mm:ss" ) ).Contains( searchText );
        var startDelay = "2 minutes after full".Contains(search);
        var gameName = "rps".Contains( search );
        if ( tournament.State.Equals( ChainTypes.TournamentState.Concluded ) ) {
            var result = account.Id.Equals( AuthorizationManager.Instance.UserData.FullAccount.Account.Id )
                ? ( ( tournament.Options.BuyIn.Amount / Math.Pow( 10, asset.Precision ) *
                      ( tournament.Options.NumberOfPlayers - 1 ) ) + asset.Symbol ).ToLower()
                .Contains( search )
                : ( "-" + ( tournament.Options.BuyIn.Amount / Math.Pow( 10, asset.Precision ) ) + asset.Symbol ).ToLower().Contains( search );

            var winner = account.Name.ToLower().Contains( search );
            return buyIn || gameName || tournamentId || winner || result;
        }
        return buyIn || jackpot || registerDeadline  || gameName || tournamentId || startDelay;
    }


    bool IsPlayerJoined( TournamentDetailsObject tournamentDetails ) {
        foreach ( var player in tournamentDetails.RegisteredPlayers ) {
            if ( player.Id == AuthorizationManager.Instance.UserData.FullAccount.Account.Id.Id ) {
                return true;
            }
        }
        return false;
    }
    

    void FilterRestoreValuesToDefault() {
        Restore();
        Filter_OnChange();
    }

    void Filter_OnChange() {
        if ( OnFilterChange != null ) {
            OnFilterChange();
        }
        SwitchItemView( null );
    }

    public void Restore() {
        foreach ( var item in items ) {
            item.RestoreFilterItem();
        }
    }

    public void SetApplyButton( bool active ) {
        applyButton.interactable = active;
    }

}
