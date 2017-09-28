
using System;
using System.Collections.Generic;
using System.Linq;
using Base.Config;
using Base.Data.Accounts;
using Base.Data.Tournaments;
using Promises;
using UnityEngine;
using UnityEngine.UI;


public class DashboardTabView : BaseCanvasView {

    [SerializeField] protected ScreenLoader loader;

    [SerializeField] protected RectTransform itemContainer;
    [SerializeField] protected RectTransform maxSizeContainer;
    [SerializeField] protected TournamentColumTitleView tournamentColumn;
    [SerializeField] protected BaseTournamentItemView tournamentItem;

    [SerializeField] protected FilterGamesController filterSettings;
    [SerializeField] protected SearchInput search;
    [SerializeField] protected Button showMoreButton;

    [SerializeField] protected DynamicGrid grid;

    protected SortOrder currentSortOrder = SortOrder.Ascending;
    protected SortType currentSortingType = SortType.RegisterDeadline;
    protected ButtonView currentSortButtonView;

    public override void Awake() {
        base.Awake();
        tournamentColumn.OnSortChange += SortingChange;
        currentSortOrder = SortOrder.Descending;
        currentSortingType = SortType.RegisterDeadline;
    }

    public virtual void Clear() {
        filterSettings.Restore();
        search.ClearInput();
    }

    public override void Show() {
        base.Show();
        if ( gameObject.activeSelf && gameObject.activeInHierarchy ) {
            ShowTournaments();
        }
    }

    public virtual void ShowTournaments() {
    }

    protected virtual void AddPage() {

    }

    #region Sorting

    public SortType CurrentSortingType {
        get { return currentSortingType; }
        set {
            if ( currentSortingType == value ) {
                currentSortOrder = currentSortOrder.Equals( SortOrder.Ascending ) ? SortOrder.Descending : currentSortOrder = SortOrder.Ascending;
            }
            currentSortingType = value;
            currentSortButtonView.GetComponent<SortingArrow>().SetArrowSprite( currentSortOrder );
            ShowTournaments();
        }
    }

    protected void SortingChange( SortType type, ButtonView button ) {
        currentSortButtonView = button;
        CurrentSortingType = type;
    }

    public IPromise<List<TournamentObject>> SortBy( SortType type, List<TournamentObject> tournamentList ) {

        List<TournamentObject> result = new List<TournamentObject>();
        if ( type.Equals( SortType.Result ) || type.Equals( SortType.Winner ) ) {
            return new Promise<List<TournamentObject>>( ( action, exeption ) => {
                TournamentManager.Instance.GetDetailsTournamentsObject( Array.ConvertAll( tournamentList.ToArray(), detail => detail.Id.Id ) )
                    .Then( tournamentDetails => {
                        ApiManager.Instance.Database.GetMatches( Array.ConvertAll( tournamentDetails, matche => matche.Matches[matche.Matches.Length - 1].Id ) )
                            .Then( lastMatches => {
                                ApiManager.Instance.Database.GetAccounts( Array.ConvertAll( lastMatches, winner => winner.MatchWinners[0].Id ) )
                                    .Then( winners => {

                                        if ( type.Equals( SortType.Winner ) ) {
                                            var winnersDistionary = new Dictionary<AccountObject, TournamentObject>();
                                            for ( int i = 0; i < winners.Length; i++ ) {
                                                winnersDistionary.Add( winners[i], tournamentList[i] );
                                            }
                                            var sortedByWinners = currentSortOrder.Equals( SortOrder.Ascending ) ? winnersDistionary.OrderBy( pair => pair.Key.Name ).ToDictionary( key => key.Key, value => value.Value ) : winnersDistionary.OrderByDescending( pair => pair.Key.Name ).ToDictionary( key => key.Key, value => value.Value );
                                            result.AddRange( sortedByWinners.Values );
                                            action( result );
                                        } else if ( type.Equals( SortType.Result ) ) {
                                            var resultDisctionary = new Dictionary<TournamentObject, long>();
                                            var me = AuthorizationManager.Instance.UserData.FullAccount.Account.Id;
                                            for ( int i = 0; i < winners.Length; i++ ) {
                                                resultDisctionary.Add( tournamentList[i], winners[i].Id.Equals( me ) ? ( tournamentList[i].PrizePool - tournamentList[i].Options.BuyIn.Amount ) : -tournamentList[i].Options.BuyIn.Amount );
                                            }
                                            var sortedByResult = currentSortOrder.Equals( SortOrder.Ascending ) ? resultDisctionary.OrderBy( pair => pair.Value ).ToDictionary( key => key.Key, value => value.Value ) : resultDisctionary.OrderByDescending( pair => pair.Value ).ToDictionary( key => key.Key, value => value.Value );
                                            result.AddRange( sortedByResult.Keys );
                                            action( result );
                                        }

                                    } );
                            } );
                    } );
            } );
        } else {
            switch ( type ) {
                case SortType.RegisterDeadline:
                    result.AddRange( currentSortOrder.Equals( SortOrder.Ascending )
                                        ? tournamentList.OrderBy( x => x.Options.RegistrationDeadline ).ToList()
                                        : tournamentList.OrderByDescending( x => x.Options.RegistrationDeadline )
                                            .ToList() );
                    break;
                case SortType.BuyIn:
                    result.AddRange( currentSortOrder.Equals( SortOrder.Ascending )
                                        ? tournamentList.OrderBy( x => x.Options.BuyIn.Amount ).ToList()
                                        : tournamentList.OrderByDescending( x => x.Options.BuyIn.Amount ).ToList() );
                    break;

                case SortType.Players:
                    result.AddRange( currentSortOrder.Equals( SortOrder.Ascending )
                                        ? tournamentList.OrderBy( x => x.Options.NumberOfPlayers ).ToList()
                                        : tournamentList.OrderByDescending( x => x.Options.NumberOfPlayers ).ToList() );
                    break;
                case SortType.GameId:
                    result.AddRange( currentSortOrder.Equals( SortOrder.Ascending )
                                        ? tournamentList.OrderBy( x => x.Id.Id ).ToList()
                                        : tournamentList.OrderByDescending( x => x.Id.Id ).ToList() );
                    break;
                case SortType.Jackpot:
                    result.AddRange( currentSortOrder.Equals( SortOrder.Ascending )
                                        ? tournamentList.OrderBy( x => x.Options.BuyIn.Amount * x.Options.NumberOfPlayers )
                                            .ToList()
                                        : tournamentList
                                            .OrderByDescending( x => x.Options.BuyIn.Amount * x.Options.NumberOfPlayers )
                                            .ToList() );
                    break;
                case SortType.StartTime:
                    if ( tournamentList[0].State == ChainTypes.TournamentState.Concluded ) {
                        result.AddRange( currentSortOrder.Equals( SortOrder.Ascending )
                                            ? tournamentList.OrderBy( x => x.StartTime ).ToList()
                                            : tournamentList.OrderByDescending( x => x.StartTime )
                                                .ToList() );
                    } else {
                        result.AddRange( currentSortOrder.Equals( SortOrder.Ascending )
                                            ? tournamentList.OrderBy( x => x.Options.StartTime ).ToList()
                                            : tournamentList.OrderByDescending( x => x.Options.StartTime )
                                                .ToList() );
                    }
                    break;
                case SortType.Winner:

                    break;
                default:
                    result.AddRange( tournamentList );
                    break;

            }
            tournamentList.Clear();
            tournamentList.AddRange( result );
            return new Promise<List<TournamentObject>>( ( action, exeption ) => action( tournamentList ) );


        }
    }

    #endregion

}


