using System;
using System.Collections;
using System.Collections.Generic;
using Base.Config;
using Base.Data.Tournaments;
using UnityEngine;

public class CurrentTournamentsListTabView : DashboardTabView {

    public event Action<TournamentObject> OnPlayClick;
    public event Action<List<TournamentObject>> OnUpdateTournaments;

    [SerializeField] SceduleTournamentItemView sceduleTournamentItemViewPrefab;
    [SerializeField] MessagePopupView messagePopupView;
    [SerializeField] ChooseHandController chooseHandController;

    [SerializeField] LeaveTournamentConfirmation leaveTournamentConfirmation;
    [SerializeField] JoinTournamentConfirmation joinTournamentConfirmation;

    List<SceduleTournamentItemView> sceduleItemsList = new List<SceduleTournamentItemView>();
    List<TournamentObject> tournamentsOnPages = new List<TournamentObject>();
    bool addPage;
    protected int maxItemsOnPage;
    protected int numberOfPages;


    public override void Awake() {
        base.Awake();
        search.OnValueChange += ShowTournaments;
        showMoreButton.onClick.AddListener( AddPage );

        numberOfPages = 1;

        TournamentManager.Instance.OnTournamentChanged += UpdateNewTournament;

    }

    public override void Clear() {
        base.Clear();
        ClearTournamentsItemViewList();
    }

    void OnRectTransformDimensionsChange() {
        if ( !CalculateMaxItemOnPage() ) {
            LoadTournaments();
        }
    }

    protected void Start() {
       CalculateMaxItemOnPage();
    }

    bool CalculateMaxItemOnPage() {
        grid.ResizeGrid();
        var newMaxitems = grid.ColumnCount * grid.RowCount;
        if ( newMaxitems != maxItemsOnPage ) {
            maxItemsOnPage = newMaxitems-1;
            return false;
        }
        return true;
    }

    public override void Hide() {
        loader.IsLoading = false;
        base.Hide();
    }

    public override void ShowTournaments() {
        if ( gameObject.activeInHierarchy && gameObject.activeSelf ) {
            LoadTournaments();
        }
    }

    void Item_OnPlayClick( SceduleTournamentItemView item ) {
        if ( OnPlayClick != null ) {
            OnPlayClick( item.CurrentTournament );
        }
    }

    void Item_OnCancelClick( SceduleTournamentItemView item ) {
        leaveTournamentConfirmation.OnOperationSuccess += Item_OnOperationSuccess;
        leaveTournamentConfirmation.SetUp( item.CurrentTournament );
    }
      
    void Item_OnJoinClick( SceduleTournamentItemView item ) {
        joinTournamentConfirmation.OnOperationSuccess += Item_OnOperationSuccess;
        joinTournamentConfirmation.SetUp( item.CurrentTournament );
    }

    void Item_OnOperationSuccess() {
        leaveTournamentConfirmation.OnOperationSuccess -= Item_OnOperationSuccess;
        joinTournamentConfirmation.OnOperationSuccess -= Item_OnOperationSuccess;
        LoadTournaments();
    }  

    void LoadTournaments() {
        if ( !gameObject.activeInHierarchy || !gameObject.activeSelf ) {
            return;
        }
        foreach ( var item in sceduleItemsList ) {
            item.IsHover = false;
        }

        loader.IsLoading = true;
        itemContainer.anchoredPosition = new Vector2( itemContainer.anchoredPosition.x, 0f );
        TournamentManager.Instance.LoadTournamentsInState( ChainTypes.TournamentState.AcceptingRegistrations, 50 )
            .Then( result => {
                var resultList = new List<TournamentObject>();
                if ( result.Length > 0 ) {
                    resultList.AddRange( result );
                }
                TournamentManager.Instance.LoadTournamentsInState( ChainTypes.TournamentState.AwaitingStart, 50 )
                    .Then( resultAwaiting => {
                        if ( resultAwaiting.Length > 0 ) {
                            resultList.AddRange( resultAwaiting );
                        }
                        TournamentManager.Instance.LoadTournamentsInState( ChainTypes.TournamentState.InProgress, 50 )
                            .Then( resultInProgress => {
                                if ( resultInProgress.Length > 0 ) {
                                    resultList.AddRange( resultInProgress );
                                }

                                if ( resultList.Count == 0 ) {
                                    loader.IsLoading = false;
                                    return;
                                }

                                if ( OnUpdateTournaments != null ) {
                                    OnUpdateTournaments( resultList );
                                }

                                if ( resultList.Count > 0 ) {
                                    resultList.Reverse();
                                    var sortResult = new Dictionary<uint, TournamentObject>();
                                    foreach ( var tournament in resultList ) {
                                        if ( !sortResult.ContainsKey( tournament.Id.Id ) ) {
                                            sortResult.Add( tournament.Id.Id, tournament );
                                        }
                                    }
                                    resultList.Clear();
                                    resultList.AddRange( sortResult.Values );

                                    SortBy( currentSortingType, resultList )
                                        .Then( sortedTournaments => {
                                            filterSettings.FilterTournaments( sortedTournaments, search.searchFilterText )
                                                .Then( resultTpurnamentList => {
                                                    if ( resultTpurnamentList.Count > 0 ) {
                                                        if ( resultTpurnamentList.Count <= maxItemsOnPage * ( numberOfPages - 1 ) && numberOfPages > 1 ) {
                                                            numberOfPages--;
                                                        }

                                                        showMoreButton.gameObject.SetActive( resultTpurnamentList.Count > maxItemsOnPage * numberOfPages );
                                                        tournamentsOnPages.Clear();

                                                        tournamentsOnPages.AddRange( resultTpurnamentList.GetRange( 0, Mathf.Min( resultTpurnamentList.Count, maxItemsOnPage * numberOfPages ) ) );
                                                        StartCoroutine( UpdateTable( tournamentsOnPages ) );
                                                    } else {
                                                        ClearTournamentsItemViewList();
                                                        loader.IsLoading = false;
                                                    }
                                                } )
                                                .Catch( exception => loader.IsLoading = false );
                                        } );
                                }
                            } )
                            .Catch( exception => loader.IsLoading = false );
                    } )
                    .Catch( exception => loader.IsLoading = false );
            } )
            .Catch( exception => loader.IsLoading = false );
    }

    void UpdateNewTournament( TournamentObject tournament ) {
        if ( !gameObject.activeInHierarchy || !gameObject.activeSelf ) {
            return;
        }
        var item = Array.Find( sceduleItemsList.ToArray(), view => view.CurrentTournament.Equals( tournament ) );

        if ( item == null ) {
            if ( tournament.State == ChainTypes.TournamentState.Concluded ||
                 tournament.State == ChainTypes.TournamentState.RegistrationPeriodExpired ) {
                return;
            }
            StartCoroutine( GetItem().UpdateItem( tournament) );
        } else {

            if ( tournament.State == ChainTypes.TournamentState.Concluded ||
                 tournament.State == ChainTypes.TournamentState.RegistrationPeriodExpired ) {
                sceduleItemsList.Remove( item );
                Destroy( item.gameObject );
                return;
            }

            item.UpdateTournament( tournament );
        }
    }


    void ClearTournamentsItemViewList() {
        foreach ( var item in sceduleItemsList ) {
            item.OnJoinClick -= Item_OnJoinClick;
            Destroy( item.gameObject );
        }
        sceduleItemsList.Clear();
    }

    IEnumerator UpdateTable( List<TournamentObject> tournamentsList ) {
        if ( !gameObject.activeInHierarchy || tournamentsList.Count == 0 || !gameObject.activeSelf) {
            loader.IsLoading = false;
            yield break;
        }
        
        for ( int i = 0; i < tournamentsList.Count; i++ ) {
            var item = ( sceduleItemsList.Count <= i ) ? GetItem() : sceduleItemsList[i];
            item.gameObject.SetActive( true );
            yield return item.UpdateItem( tournamentsList[i] );
        }

        if ( numberOfPages > 1 && addPage ) {
            itemContainer.anchoredPosition = new Vector2(itemContainer.anchoredPosition.x,sceduleItemsList.Count*tournamentItem.GetComponent<RectTransform>().sizeDelta.y);
            addPage = false;
        }

        for ( int i = sceduleItemsList.Count - 1; i >= tournamentsList.Count; i-- ) {
            sceduleItemsList[i].OnJoinClick -= Item_OnJoinClick;
            Destroy( sceduleItemsList[i].gameObject );
            sceduleItemsList.RemoveAt( i );
        }
        loader.IsLoading = false;
    }

    SceduleTournamentItemView GetItem() {
        var item = Instantiate( sceduleTournamentItemViewPrefab );
        item.transform.SetParent( itemContainer, false );
        item.transform.localScale = Vector3.one;
        sceduleItemsList.Add( item );
        item.OnCancelClick += Item_OnCancelClick;
        item.OnPlayClick += Item_OnPlayClick;
        item.OnJoinClick += Item_OnJoinClick;
        return item;
    }

    protected override void AddPage() {
        numberOfPages++;
        addPage = true;
        LoadTournaments();

    }

}