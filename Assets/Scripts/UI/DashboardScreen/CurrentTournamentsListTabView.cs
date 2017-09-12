using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Base.Config;
using Base.Data;
using Base.Data.Operations.Fee;
using Base.Data.Properties;
using Base.Data.Tournaments;
using Base.Transactions.Tournaments;
using Tools;
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


    protected int maxItemsOnPage;
    protected int numberOfPages;


    public override void Awake() {
        base.Awake();
        search.OnValueChange += ShowTournaments;
        showMoreButton.onClick.AddListener( AddPage );

        numberOfPages = 1;

        TournamentManager.Instance.OnTournamentChanged += UpdateNewTournament;

    }

    protected void Start() {
        maxItemsOnPage = Convert.ToInt32( maxSizeContainer.rect.height /
                                          tournamentItem.GetComponent<RectTransform>().rect.height );
    }


    public override void Hide() {
        loader.IsLoading = false;
        ClearTournamentsItemViewList();
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
        currentJoinItem = item;
        leaveTournamentConfirmation.OnOperationSuccess += Item_OnCancelUpdate;
        leaveTournamentConfirmation.SetUp( item.CurrentTournament );
    }

    void Item_OnCancelUpdate() {
        StartCoroutine( currentJoinItem.UpdateItem( currentJoinItem.CurrentTournament, null ) );
        leaveTournamentConfirmation.OnOperationSuccess -= Item_OnCancelUpdate;
		joinTournamentConfirmation.OnOperationSuccess -= Item_OnCancelUpdate;
    }


    void Item_OnJoinClick( SceduleTournamentItemView item ) {
		currentJoinItem = item;
		joinTournamentConfirmation.OnOperationSuccess += Item_OnCancelUpdate;
		joinTournamentConfirmation.SetUp( item.CurrentTournament );
    }

    private SceduleTournamentItemView currentJoinItem;

   
    

    IEnumerator UpdateItem( SceduleTournamentItemView item ) {
        if ( item.IsNull() ) {
            yield break;
        }
        TournamentObject tournament = null;
        TournamentDetailsObject tournamentDetails = null;
        TournamentManager.Instance.LoadTournament( item.CurrentTournament.Id.Id )
            .Then( result => {
                TournamentManager.Instance.GetDetailsTournamentObject( result.Id.Id )
                    .Then( details => {
                        tournamentDetails = details;
                        tournament = result;
                        loader.IsLoading = false;
                    } );
            } );
        while ( tournament.IsNull() || tournamentDetails.IsNull() ) {
            yield return null;
        }
        yield return item.UpdateItem( tournament, tournamentDetails );
    }
		
    void LoadTournaments() {
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

                                    SortBy( currentSortingType, resultList );
                                    filterSettings.FilterTournaments( resultList, search.searchFilterText )
                                        .Then( resultTpurnamentList => {
                                            if ( resultTpurnamentList.Count > 0 ) {
                                                if ( resultTpurnamentList.Count <=
                                                     maxItemsOnPage * ( numberOfPages - 1 ) &&
                                                     numberOfPages > 1 ) {
                                                    numberOfPages--;
                                                }
                                                tournamentsOnPages.Clear();
                                                tournamentsOnPages.AddRange( resultTpurnamentList
                                                                                .GetRange( 0,
                                                                                          Math
                                                                                              .Min( maxItemsOnPage * numberOfPages,
                                                                                                   resultTpurnamentList
                                                                                                       .Count ) ) );
                                                StartCoroutine( UpdateTable( tournamentsOnPages ) );
                                            } else {
                                                ClearTournamentsItemViewList();
                                                loader.IsLoading = false;
                                            }
                                        } )
                                        .Catch( exception => loader.IsLoading = false );
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
            StartCoroutine( GetItem().UpdateItem( tournament, null ) );
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
        if ( !gameObject.activeInHierarchy || tournamentsList.Count == 0 ) {
            loader.IsLoading = false;
            yield break;
        }

        if ( numberOfPages > 1 && addPage ) {
            itemContainer.anchoredPosition = new Vector2(
                                                         itemContainer.anchoredPosition.x,
                                                         maxItemsOnPage * ( numberOfPages - 1 ) *
                                                         tournamentItem.GetComponent<RectTransform>().sizeDelta.y
                                                        );
            addPage = false;
        }
        for ( int i = 0; i < tournamentsList.Count; i++ ) {
            var item = ( sceduleItemsList.Count <= i ) ? GetItem() : sceduleItemsList[i];
            item.gameObject.SetActive( true );
            var details = new List<TournamentDetailsObject>();
            yield return TournamentManager.Instance.GetTournamentDetailsObject( tournamentsList[i].Id.Id, details );
            yield return item.UpdateItem( tournamentsList[i], details[0] );
        }
        for ( int i = sceduleItemsList.Count - 1; i > tournamentsList.Count; i-- ) {
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

    private bool addPage;

    protected override void AddPage() {
        numberOfPages++;
        addPage = true;
        LoadTournaments();

    }

}