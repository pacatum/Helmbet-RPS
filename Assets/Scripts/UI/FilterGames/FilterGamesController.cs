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
            //item.OnFilterItemChanged += Filter_OnChange;
        }
        FilterRestoreValuesToDefault();
    }

    void Start() {
        SwitchItemView( null );
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

    public IPromise<List<TournamentObject>> FilterTournaments(
        List<TournamentObject> tournaments, string searchFilter ) {
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

                    TournamentManager.Instance.GetMatchWinners( details )
                        .Then( accounts => {
                            resolve(FilterTournaments(tournaments, assets, searchFilter, details, accounts));
                        } )
                        .Catch( exception => {
                            
                            resolve(FilterTournaments(tournaments, assets,searchFilter, details, new AccountObject[details.Length]));
                        } );

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
                           game.SelectChoise.Equals( "ROCK, PAPER, SCISSORS " );

            var search = IsTournamentContains( tournaments[i], assets[i], searchFilter, accounts[i].IsNull()?null:accounts[i] );

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

        var winner = account.IsNull() ? string.Empty.Contains( search ) : account.Name.ToLower().Contains( search );
        var tournamentId = tournament.Id.ToString().ToLower().Contains(search);
        var buyIn = ((tournament.Options.BuyIn.Amount / Math.Pow(10, asset.Precision)) + asset.Symbol).ToLower().Contains(search);
        var jackpot = ((tournament.Options.BuyIn.Amount / Math.Pow(10, asset.Precision) * tournament.Options.NumberOfPlayers) + asset.Symbol).ToLower().Contains(search);
        var registerDeadline = tournament.Options.RegistrationDeadline.ToLocalTime().ToString("ddMMM, yyyy. HH:mm tt").ToLower().Contains(search);
        var startTime = tournament.Options.StartTime.HasValue ? tournament.Options.StartTime.Value.ToLocalTime().ToString("ddMMM, yyyy. HH:mm tt").ToLower().Contains(search) : string.Empty.Contains(search);
        var startDelay = "2 minutes after full".Contains(search);
        var gameName = "RPS".Contains(search);
        var result = false;
        if ( tournament.State == ChainTypes.TournamentState.Concluded ) {
            result = account.Id.Equals( AuthorizationManager.Instance.UserData.FullAccount.Account.Id )
                ? ( ( tournament.Options.BuyIn.Amount / Math.Pow( 10, asset.Precision ) *
                      ( tournament.Options.NumberOfPlayers - 1 ) ) + asset.Symbol ).ToLower()
                .Contains( search )
                : ( "-" + ( tournament.Options.BuyIn.Amount / Math.Pow( 10, asset.Precision ) ) + asset.Symbol )
                .ToLower()
                .Contains( search );
            startTime = tournament.StartTime.HasValue ? tournament.StartTime.Value.ToLocalTime().ToString("ddMMM, yyyy. HH:mm tt").ToLower().Contains(search) : string.Empty.Contains(search);
            registerDeadline = false;
            jackpot = false;
        }
        

        return buyIn || jackpot || registerDeadline || startTime || startDelay || gameName || tournamentId || winner || result;
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
        applyButton.enabled = active;
    }

}
