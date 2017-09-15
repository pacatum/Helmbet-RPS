using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base.Data.Tournaments;
using UnityEngine;
using Base.Config;

public class HistoryTournamentsListView : DashboardTabView {

    [SerializeField] HistoryTournamentItemView historyTournamentItemViewPrefab;

    List<HistoryTournamentItemView> historyItemsList = new List<HistoryTournamentItemView>();
    List<TournamentObject> tournamentsOnPages = new List<TournamentObject>();
    protected int maxItemsOnPage;
    protected int numberOfPages;

    public override void Awake() {
        base.Awake();
        search.OnValueChange += ShowTournaments;
        showMoreButton.onClick.AddListener( AddPage );

        numberOfPages = 1;

    }

    protected void Start() {
        maxItemsOnPage = Convert.ToInt32( maxSizeContainer.rect.height /
                                          tournamentItem.GetComponent<RectTransform>().rect.height );
    }

    public override void ShowTournaments() {
        if ( gameObject.activeSelf ) {
            LoadTournaments();
        }
    }

    void CheckUserTournaments( List<TournamentObject> list ) {

        var tournamentIds = new Dictionary<uint, TournamentObject>();
        foreach ( var tournament in list ) {
            tournamentIds.Add( tournament.Id.Id, tournament );
        }

        TournamentManager.Instance.GetDetailsTournamentsObject( tournamentIds.Keys.ToArray() )
            .Then( result => {
                var resultList = new List<TournamentObject>();
                foreach ( var tournamentDetail in result ) {
                    foreach ( var player in tournamentDetail.RegisteredPlayers ) {
                        if ( player.Id.Equals( AuthorizationManager.Instance.UserData.FullAccount.Account.Id.Id ) ) {
                            resultList.Add( tournamentIds[tournamentDetail.Tournament.Id] );
                        }
                    }
                }

                if ( resultList.Count == 0 ) {
                    loader.IsLoading = false;
                    return;
                }

                filterSettings.FilterTournaments( resultList, search.searchFilterText )
                    .Then( filterResult => {

                        if ( filterResult.Count > 0 ) {
                            if ( filterResult.Count <= maxItemsOnPage * ( numberOfPages - 1 ) && numberOfPages > 1 ) {
                                numberOfPages--;
                            }
                            tournamentsOnPages.Clear();
                            tournamentsOnPages.AddRange( filterResult.GetRange( 0, Math.Min( filterResult.Count, maxItemsOnPage * numberOfPages ) ) );
                            StartCoroutine( UpdateTable( tournamentsOnPages ) );

                        } else {
                            ClearTournamentsItemViewList();
                            loader.IsLoading = false;
                        }
                    } )
                    .Catch( exception =>  loader.IsLoading = false);


            } );
    }

    void LoadTournaments() {
        loader.IsLoading = true;
        itemContainer.anchoredPosition = new Vector2( itemContainer.anchoredPosition.x, 0f );
        TournamentManager.Instance.LoadTournamentsInState( ChainTypes.TournamentState.Concluded, 100 )
            .Then( tournamentsResult => {
                if ( tournamentsResult.Length > 0 ) {
                    var tournaments = new List<TournamentObject>( tournamentsResult );
                    SortBy( currentSortingType, tournaments ).Then( sortedTournaments => CheckUserTournaments( sortedTournaments ));
                } else {
                    loader.IsLoading = false;
                }

            } );
    }


    void ClearTournamentsItemViewList() {
        foreach ( var item in historyItemsList ) {
            Destroy( item.gameObject );
        }
        historyItemsList.Clear();
    }


    IEnumerator UpdateTable( List<TournamentObject> tournamentsList ) {

        if (numberOfPages > 1 && addPage)
        {
            itemContainer.anchoredPosition = new Vector2(itemContainer.anchoredPosition.x,maxItemsOnPage * (numberOfPages - 1) * tournamentItem.GetComponent<RectTransform>().sizeDelta.y );
            addPage = false;
        }

        for ( int i = 0; i < tournamentsList.Count; i++ ) {
            var item = ( historyItemsList.Count <= i ) ? GetItem() : historyItemsList[i];
            yield return item.UpdateItem( tournamentsList[i]);
        }

        for ( int i = historyItemsList.Count - 1; i >= tournamentsList.Count; i-- ) {
            Destroy( historyItemsList[i].gameObject );
            historyItemsList.RemoveAt( i );
        }

        loader.IsLoading = false;
    }

    HistoryTournamentItemView GetItem() {
        var item = Instantiate( historyTournamentItemViewPrefab );
        item.transform.SetParent( itemContainer, false );
        item.transform.localScale = Vector3.one;
        historyItemsList.Add( item );
        return item;
    }

    private bool addPage;
    protected override void AddPage() {
        numberOfPages++;
        addPage = true;
        LoadTournaments();
    }

}
