using System;
using System.Collections;
using System.Collections.Generic;
using Base.Config;
using Base.Data.Tournaments;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class TournamentItemFooterView : MonoBehaviour {

    
    [SerializeField] Color playerInTournamentColor;
    [SerializeField] Color otherTournamentsColor;
    [SerializeField] Color hoverColor;
    [SerializeField] Image footerTargetImage;

    [SerializeField] Color myTournamentsItemBgColor;
    [SerializeField] Color otherTournamentsItemBgColor;
    [SerializeField] Image itemBgColor;

    [SerializeField] Button activePlayButton;
    [SerializeField] Button inActivePlayButton;
    [SerializeField] Button joinButton;
    [SerializeField] Button cancelButton;

    [SerializeField] TextMeshProUGUI startTimeText;
    [SerializeField] TextMeshProUGUI registerTimeText;
    [SerializeField] TextMeshProUGUI opponentText;
    [SerializeField] UnityEngine.GameObject liveMessage;

    [SerializeField] Image headerItemView;
    [SerializeField] Color playColor;
    [SerializeField] Color awaitingColor;

    Color currentColor;
    Color currentItemBgColor;
    Color currentTextColor;

    TournamentObject currenTournamentObject;
    TournamentDetailsObject currenTournamentDetailsObject;

    public Button PlayButton {
        get { return activePlayButton; }
    }

    public Button JoinButton {
        get { return joinButton; }
    }

    public Button CancelButton {
        get { return cancelButton; }
    }
    
    void Awake() {
        if ( activePlayButton != null ) {
            activePlayButton.onClick.AddListener( OpenGameScreen );
        }
    }

    public void UpdateFooter( TournamentObject tournament, TournamentDetailsObject details, bool isHover ) {

        if ( isHover ) {
            footerTargetImage.color = hoverColor;
            itemBgColor.color = myTournamentsItemBgColor;
            startTimeText.color = myTournamentsItemBgColor;
            if ( joinButton != null) {
                joinButton.GetComponent<UIButtonView>().SetAlternativeState();
            }
            if ( activePlayButton != null ) {
                activePlayButton.GetComponent<UIButtonView>().SetAlternativeState();
            }
        } else {
            footerTargetImage.color = currentColor;
            itemBgColor.color = currentItemBgColor;
            startTimeText.color = currentTextColor;
            if ( joinButton != null ) {
                joinButton.GetComponent<UIButtonView>().SetNormalState();
            }
            if ( activePlayButton != null ) {
                activePlayButton.GetComponent<UIButtonView>().SetNormalState();
            }
        }

        switch ( tournament.State ) {
            case ChainTypes.TournamentState.InProgress:
                break;
            case ChainTypes.TournamentState.AcceptingRegistrations:
                if ( !IsPlayerJoined( details ) ) {
                    joinButton.gameObject.SetActive( isHover );
                    registerTimeText.gameObject.SetActive( !isHover );
                }
                break;
            case ChainTypes.TournamentState.AwaitingStart:
                break;

        }
    }

    bool IsPlayerJoined( TournamentDetailsObject details ) {
        return  details.RegisteredPlayers.Contains( AuthorizationManager.Instance.Authorization.UserNameData.FullAccount.Account.Id );
    }

    void SaveColors() {
        currentColor = footerTargetImage.color;
        currentItemBgColor = itemBgColor.color;
        currentTextColor = startTimeText.color;
    }

    public IEnumerator SetUp( TournamentObject tournament, TournamentDetailsObject details ) {

        currenTournamentObject = tournament;
        currenTournamentDetailsObject = details;

        if ( tournament.State == ChainTypes.TournamentState.Concluded ) {
            SaveColors();
            yield break;
        }
        var me = AuthorizationManager.Instance.UserData;

        activePlayButton.gameObject.SetActive( false );
        inActivePlayButton.gameObject.SetActive( false );
        joinButton.gameObject.SetActive( false );
        cancelButton.gameObject.SetActive( false );
        startTimeText.gameObject.SetActive( false );
        registerTimeText.gameObject.SetActive( false );
        opponentText.gameObject.SetActive( false );
        liveMessage.gameObject.SetActive( false );
        headerItemView.gameObject.SetActive( false );

        if ( IsPlayerJoined( details ) ) {
            footerTargetImage.color = playerInTournamentColor;
            itemBgColor.color = myTournamentsItemBgColor;
        } else {
            footerTargetImage.color = otherTournamentsColor;
            itemBgColor.color = otherTournamentsItemBgColor;
        }


        switch ( tournament.State ) {
            case ChainTypes.TournamentState.InProgress:
                if ( !IsPlayerJoined( details ) ) {
                    liveMessage.SetActive( true );
                } else {
                    var matchesList = new List<MatchObject>();
                    yield return TournamentManager.Instance.GetMatcheObjects( Array.ConvertAll( details.Matches, matche => matche.Id ), matchesList );

                    var matchesInProgress = Array.FindAll( matchesList.ToArray(), match => match.State.Equals( ChainTypes.MatchState.InProgress ) );
                    var playerInMatches = Array.FindAll( matchesInProgress, match => match.Players.Contains( me.FullAccount.Account.Id ) );

                    if ( playerInMatches.Length == 0 ) {
                        inActivePlayButton.gameObject.SetActive( true );
                        headerItemView.gameObject.SetActive( true );
                        headerItemView.color = awaitingColor;
                    } else {
                        opponentText.gameObject.SetActive( true );
                        activePlayButton.gameObject.SetActive( true );
                        headerItemView.gameObject.SetActive( true );
                        headerItemView.color = playColor;
                        opponentText.text = Utils.GetFormatedString( GameManager.Instance.CurrentMatch.Opponent.Name );
                    }

                    SaveColors();


                }
                break;
            case ChainTypes.TournamentState.AcceptingRegistrations:
                startTimeText.gameObject.SetActive( true );
                UpdateStartTime( tournament.StartTime);

                if ( IsPlayerJoined( details ) ) {
                    cancelButton.gameObject.SetActive( true );
                } else {
                    registerTimeText.gameObject.SetActive( true );
                    UpdateRegisterTime( tournament.Options.RegistrationDeadline );
                }

                SaveColors();
                break;

            case ChainTypes.TournamentState.AwaitingStart:
                if ( IsPlayerJoined( details ) ) {
                    headerItemView.gameObject.SetActive( true );
                    headerItemView.color = awaitingColor;
                    if ( ( tournament.StartTime.Value - DateTime.UtcNow ).TotalMinutes <= 2 ) {
                        activePlayButton.gameObject.SetActive( true );
                    } else {
                        inActivePlayButton.gameObject.SetActive( true );
                        TournamentManager.Instance.AddAwaitingStartTournaments( tournament );
                    }
                }

                SaveColors();
                break;
        }
    }

    void UpdateRegisterTime( DateTime registerTime ) {
        var time = registerTime - DateTime.UtcNow;
        if ( time.TotalMinutes < 60 ) {
            registerTimeText.text = "Register: <" + (int) time.TotalMinutes + "m";
        } else if ( time.TotalHours > 0 ) {
            registerTimeText.text = "Register: ~" + (int) time.TotalHours + "h";
        }
    }

    void UpdateStartTime( DateTime? startTime ) {
        if ( startTime.HasValue ) {
            var time = startTime - DateTime.UtcNow;
            startTimeText.text = "Start: " + Convert.ToDateTime( time.ToString() ).ToString( "hh:mm:ss" );
        } else {
            startTimeText.text = "2 minutes after full";
        }
    }

    public void OpenGameScreen() {
        var me = AuthorizationManager.Instance.UserData.FullAccount.Account.Id;
        if ( currenTournamentObject.State.Equals( ChainTypes.TournamentState.AwaitingStart ) && IsPlayerJoined( currenTournamentDetailsObject ) ) {
            UIController.Instance.UpdateStartGamePreview( currenTournamentObject );
            return;
        }
        if (currenTournamentDetailsObject.RegisteredPlayers.Contains( me ) ) {
            ApiManager.Instance.Database.GetMatches( Array.ConvertAll( currenTournamentDetailsObject.Matches, match => match.Id ) )
                .Then( matches
                          => {
                          var matchesInProgress =
                              Array.FindAll( matches,
                                            match => match.State == ChainTypes.MatchState.InProgress );
                          var playerInMatches =
                              Array.FindAll( matchesInProgress, player => player.Players.Contains( me ) );

                          if ( playerInMatches.Length == 0 ) {
                              UIController.Instance.UpdateTournamentDetails( currenTournamentObject );
                              UIManager.Instance.CurrentState = UIManager.ScreenState.TournamentDetails;
                          } else {
                              UIController.Instance.UpdateTournamentInProgress( currenTournamentObject );
                          }

                      } );

        } else {
            UIController.Instance.UpdateTournamentDetails( currenTournamentObject );
            UIManager.Instance.CurrentState = UIManager.ScreenState.TournamentDetails;
        }
    }


    public void OpenDetailsView() {
        UIController.Instance.UpdateTournamentDetails( currenTournamentObject );
        UIManager.Instance.CurrentState = UIManager.ScreenState.TournamentDetails;
    }

    public void OpenWaitingGameView() {
        var me = AuthorizationManager.Instance.UserData.FullAccount.Account.Id;

        if ( currenTournamentDetailsObject.RegisteredPlayers.Contains( me ) && ( currenTournamentObject.StartTime.Value - DateTime.UtcNow ).TotalMinutes <= 2 ) {
            UIController.Instance.UpdateStartGamePreview( currenTournamentObject );
        }
    }

}
