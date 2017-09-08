
using System;
using System.Collections.Generic;
using System.Linq;
using Base.Config;
using Base.Data.Tournaments;
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

   protected SortOrder currentSortOrder = SortOrder.Ascending;
   protected SortType currentSortingType = SortType.RegisterDeadline;
    
    protected ButtonView currentSortButtonView;

    public override void Awake() {
        base.Awake();
        tournamentColumn.OnSortChange += SortingChange;
        currentSortOrder = SortOrder.Descending;
    }
    public void Clear() {
        filterSettings.Restore();
        search.ClearInput();
    }

    public override void Show() {
        base.Show();
        if ( gameObject.activeSelf && gameObject.activeInHierarchy) {
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
                currentSortOrder = currentSortOrder.Equals(SortOrder.Ascending) ? SortOrder.Descending : currentSortOrder = SortOrder.Ascending;
            }
            currentSortingType = value;
            currentSortButtonView.GetComponent<SortingArrow>().SetArrowSprite(currentSortOrder);
            ShowTournaments();
        }
    }

    protected void SortingChange( SortType type, ButtonView button ) {
        currentSortButtonView = button;
        CurrentSortingType = type;
    }

    public void SortBy( SortType type, List<TournamentObject> tournamentList ) {
        List<TournamentObject> result = new List<TournamentObject>();
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
    }

    #endregion

}


