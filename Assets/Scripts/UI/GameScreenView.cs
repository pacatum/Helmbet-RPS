using System;
using System.Collections;
using System.Collections.Generic;
using Base.Config;
using Base.Data.Json;
using Base.Data.Tournaments;
using Tools;
using UnityEngine;
using UnityEngine.UI;
using Gesture = Base.Config.ChainTypes.RockPaperScissorsGesture;

public class GameChoiseResult
{

    public int roundNumber;
    public Gesture? playerGesture;
    public Gesture? opponentGesture;
    public GameResult result;
}

public enum ScreenState {

    Game,
    RoundOver,
    Wait
}

public class GameScreenView : BaseCanvasView {

    [SerializeField] Text thisUserNickname;
    [SerializeField] Text thisUserNicknameRounds;

    [SerializeField] Text opponentNickname;
    [SerializeField] Text opponentNicknameRounds;
    [SerializeField] Text opponentNicknameWaitingView;

    [SerializeField] HandAnimatorEventHandler playerHand;
    [SerializeField] HandAnimatorEventHandler opponentHand;

    [SerializeField] Button rockButton;
    [SerializeField] Button paperButton;
    [SerializeField] Button scissorsButton;

    [SerializeField] ResultCounterController counterController;
    [SerializeField] ResultLogController logController;

    [SerializeField] GameRoundOverView gameRoundOverView;
    [SerializeField] GameStartPreview gameStartPreview;

    [SerializeField] Text timerText;
    [SerializeField] UnityEngine.GameObject waitObject;
    [SerializeField] LastRoundChoiseView lastRoundChoiseView;
    [SerializeField] Text jackpoText;

    [SerializeField] private Text gamesToWinRoundText;
    [SerializeField] private Text gamesToWinTournamentText;
    [SerializeField] UnityEngine.GameObject buttonsObject;

    List<GameChoiseResult> history = new List<GameChoiseResult>();
    List<Button> buttons = new List<Button>();
    bool isTimer = false;
    int winsToWinGame;
    int winsToWinTournament;
    ScreenState currentScreenState;
    DateTime nextTimeout;


    public TournamentObject CurrentTournament { get; set; }

    public override void Awake() {
        base.Awake();
        rockButton.onClick.AddListener( ChooseRock );
        paperButton.onClick.AddListener( ChoosePaper );
        scissorsButton.onClick.AddListener( ChooseScissors );

        buttons.Add( scissorsButton );
        buttons.Add( rockButton );
        buttons.Add( paperButton );
    }

    public override void Show() {
        base.Show();
        gameRoundOverView.Hide();
        gameStartPreview.gameObject.SetActive( false );
        
        MatcheStart( GameManager.Instance.CurrentMatch );

        GameManager.Instance.OnGameComplete += GameComplete;
        GameManager.Instance.OnMatchComplete += MatcheComplete;
        GameManager.Instance.OnNewMatch += MatcheStart;
    }

    public override void Hide() {
        GameManager.Instance.OnGameComplete -= GameComplete;
        GameManager.Instance.OnMatchComplete -= MatcheComplete;
        GameManager.Instance.OnNewMatch -= MatcheStart;
        gameStartPreview.gameObject.SetActive(true);
        Refresh();
        gameRoundOverView.Hide();
        AudioManager.Instance.StopPlaying();
        playerHand.SetTrigger("idle");
        opponentHand.SetTrigger("idle");
        base.Hide();
    }

    public ScreenState CurrentScreenState {
        get { return currentScreenState; }
        set {
            currentScreenState = value;
        }
    }

    public void ShowWaitingGameView( TournamentObject tournament ) {
        Refresh();

        if ( PlayerPrefs.GetInt( tournament.Id.ToString() ) == 0 ) {
            AudioManager.Instance.PlayNoticeSound();
            PlayerPrefs.SetInt( tournament.Id.ToString(), (int) tournament.Id.Id );
        }
        gameRoundOverView.Hide();
        waitObject.SetActive( false );
        gameStartPreview.SetTournamentInformation( tournament );
        var me = AuthorizationManager.Instance.UserData;
        ApiManager.Instance.Database.GetTournamentDetails( tournament.Id.Id )
            .Then( details => {
                TournamentManager.Instance.GetAssetObject( tournament.Options.BuyIn.Asset.Id )
                    .Then( asset => {
                        jackpoText.text =
                            Utils.GetFormatedDecimaNumber( ( tournament.PrizePool /
                                                             Math.Pow( 10, asset.Precision ) ).ToString() ) +
                            asset.Symbol;
                        if ( tournament.RegisteredPlayers == 2 ) {
                            ApiManager.Instance.Database
                                .GetAccounts( Array.ConvertAll( details.RegisteredPlayers, player => player.Id ) )
                                .Then( accounts
                                          => {
                                          var opponent = Array
                                              .Find( accounts,
                                                    account => !account.Id.Equals( me.FullAccount
                                                                                      .Account.Id ) )
                                              .Name;
                                          UpdateUsernameText( me.UserName, opponent );
                                          opponentNicknameWaitingView.text = opponent;
                                          winsToWinTournament = 5;
                                      } );
                        } else {
                            UpdateUsernameText( me.UserName, "UNDEFINED" );
                            winsToWinTournament = (int) Math.Log( tournament.Options.NumberOfPlayers, 2 ) * 5 -
                                                  (5 - winsToWinGame );
                        }

                    } );


                winsToWinGame = 5;
                gamesToWinRoundText.text = winsToWinGame.ToString();
                gamesToWinTournamentText.text = winsToWinTournament < 0 ? 0.ToString() : winsToWinTournament.ToString();

            } );

    }


    public void Refresh() {
        counterController.ClearCounter();
        logController.ClearLog();
        history.Clear();
        gameRoundOverView.ClearHistory();
        winsToWinGame = 5;
    }

    void DisabledButtons( Button target ) {
        foreach ( var button in buttons ) {
                button.interactable = false;
            if ( !button.Equals( target ) ) {
                button.transform.parent.gameObject.SetActive( false );
            }
        }
    }

    void ActivateButton() {
        buttonsObject.SetActive(true);
        foreach ( var button in buttons ) {
            button.transform.parent.gameObject.SetActive( true );
            button.interactable = true;
        }
    }

    void ChoosePaper() {
        var me = AuthorizationManager.Instance.UserData.FullAccount.Account.Id.ToString();
        if ( PlayerPrefs.GetString( GameManager.Instance.CurrentGame.Id.ToString()+me + " choosen step" ) == "" ) {
            DisabledButtons( paperButton );
            GameManager.Instance.PaperMove();
            AudioManager.Instance.PlayPaperChooseSound();
            PlayerPrefs.SetString( GameManager.Instance.CurrentGame.Id.ToString()+me + " choosen step", "paper" );
        }
    }

    void ChooseRock() {
        var me = AuthorizationManager.Instance.UserData.FullAccount.Account.Id.ToString();
        if ( PlayerPrefs.GetString( GameManager.Instance.CurrentGame.Id.ToString() +me+ " choosen step" ) == "" ) {
            DisabledButtons( rockButton );
            GameManager.Instance.RockMove();
            AudioManager.Instance.PlayRockChooseSound();
            PlayerPrefs.SetString( GameManager.Instance.CurrentGame.Id.ToString()+me + " choosen step", "rock" );
        }
    }

    void ChooseScissors() {
        var me = AuthorizationManager.Instance.UserData.FullAccount.Account.Id.ToString();
        if ( PlayerPrefs.GetString( GameManager.Instance.CurrentGame.Id.ToString()+me + " choosen step" ) == "" ) {
            DisabledButtons( scissorsButton );
            GameManager.Instance.ScissorsMove();
            AudioManager.Instance.PlayScissorsChooseSound();
            PlayerPrefs.SetString( GameManager.Instance.CurrentGame.Id.ToString()+me + " choosen step", "scissors" );
        }
    }

    void MatcheComplete( MatchContainer match ) {
        if ( TournamentManager.Instance.CurrentTournament == null ) {
            return;
        }
        if ( !match.Tournament.Equals( TournamentManager.Instance.CurrentTournament.Id ) ) {
            return;
        }
        GameManager.Instance.OnGameExpectedMove -= ExpectingMove;
        StartCoroutine( MatcheCompleteCoroutine( match ) );
    }

    void UpdateMoveButtons() {
        var me = AuthorizationManager.Instance.UserData.FullAccount.Account.Id.ToString();
        if ( PlayerPrefs.GetString( GameManager.Instance.CurrentGame.Id.ToString()+me + " choosen step" ) == "" ) {
            ActivateButton();
        } else {
            if ( PlayerPrefs.GetString(GameManager.Instance.CurrentGame.Id.ToString() + me+ " choosen step" ) ==
                 "paper" ) {
                DisabledButtons( paperButton );
            } else if ( PlayerPrefs.GetString(GameManager.Instance.CurrentGame.Id.ToString() + me + " choosen step" ) ==
                        "rock" ) {
                DisabledButtons( rockButton );
            } else if ( PlayerPrefs.GetString(GameManager.Instance.CurrentGame.Id.ToString() + me + " choosen step" ) ==
                        "scissors" ) {
                DisabledButtons( scissorsButton );
            }
        }

    }

    IEnumerator MatcheCompleteCoroutine( MatchContainer match ) {
        yield return new WaitForSecondsRealtime( 2f );
        if ( match.Equals( GameManager.Instance.CurrentMatch ) ) {
            UpdateHistory( match );
        }
    }


    void UpdateHistory( MatchContainer match ) {
        history.Clear();
        gameRoundOverView.ClearHistory();
        foreach ( var game in match.CompletedGames ) {
            history.Add( GetNewStep( game.Value ) );
        }
        var lastStep = history[history.Count - 1];
        lastRoundChoiseView.SetLastStep( lastStep.roundNumber, lastStep.playerGesture, lastStep.opponentGesture,
                                        lastStep.result );
        history.Remove( lastStep );
        gameRoundOverView.Show( history );
        history.Clear();

        if ( !match.Winner.Equals( match.Me ) ) {
            gameRoundOverView.CurrentGameRoundOverState = GameRoundOverState.Lose;
            AudioManager.Instance.PlayTotalLooseSound();
            return;
        }

        ApiManager.Instance.Database.GetTournament( match.Tournament.Id )
            .Then( tournament => {

                TournamentManager.Instance.GetMatchesPromise( tournament.Id.Id )
                    .Then( matches => {
                        if ( matches[matches.Length - 1].Id.Equals( match.Id ) && matches[matches.Length - 1]
                                 .MatchWinners.Contains( match.Me.Id ) ) {
                            gameRoundOverView.CurrentGameRoundOverState = GameRoundOverState.Win;
                            AudioManager.Instance.PlayTotalWinSound();
                        }  else {
                            var count = 0;
                            foreach ( var matche in matches ) {
                                if ( matche.State == ChainTypes.MatchState.InProgress ) {
                                    count++;
                                }
                            }
                            gameRoundOverView.SetMatchesInProgressCount( count );
                            gameRoundOverView.CurrentGameRoundOverState = GameRoundOverState.RoundOver;
                            winsToWinGame = 5;
                            gamesToWinRoundText.text = winsToWinGame.ToString();
                            winsToWinTournament =
                            ( ( (int) Math.Log( GameManager.Instance.GetTournamentNumberOfPlayers( match.Tournament ),
                                               2 ) -
                                GameManager.Instance.GetCompletedMatches( match.Tournament ) ) * 5 -
                              ( 5 - winsToWinGame ) );
                            gamesToWinTournamentText.text = winsToWinTournament < 0 ? 0.ToString() : winsToWinTournament.ToString();
                        }
                    } );

            } );
    }

    void UpdateUsernameText( string username, string opponentUsername ) {
        var player = username.Length > 10 ? username.Substring( 0, 10 ) + "..." : username;
        var opponent = opponentUsername.Length > 10 ? opponentUsername.Substring(0, 10) + "..." : opponentUsername;
        thisUserNickname.text = player;
        thisUserNicknameRounds.text = player;

        opponentNickname.text = opponent;
        opponentNicknameRounds.text = opponent;
        opponentNicknameWaitingView.text = opponent;
    }

    void ExpectingMove( MatchContainer match, GameContainer game ) {
        if ( !match.Tournament.Equals( TournamentManager.Instance.CurrentTournament.Id ) ) {
            return;
        }
        
        nextTimeout = game.NextTimeout.Value;
        UpdateGameUi( match );
        if ( gameObject.activeInHierarchy ) {
            StartCoroutine( NewGameStart() );
        }
    }


    IEnumerator NewGameStart( ) {
        yield return new WaitForSecondsRealtime( 2f );
        AudioManager.Instance.PlayWaitingSound();
        UpdateTimer();
        UpdateMoveButtons();
    }

    void MatcheStart( MatchContainer match ) {
        if ( TournamentManager.Instance.CurrentTournament == null ) {
            return;
        }
        if ( match.IsNull() || !match.Tournament.Equals( TournamentManager.Instance.CurrentTournament.Id )) {
            return;
        }
        
        GameManager.Instance.SetCurrentTournament( match.Tournament );

        GameManager.Instance.OnGameExpectedMove -= ExpectingMove;
        GameManager.Instance.OnGameExpectedMove += ExpectingMove;
        StopCoroutine( "MatcheCompleteCoroutine" );
        gameRoundOverView.gameObject.SetActive( false );
        gameStartPreview.gameObject.SetActive( false );

        if ( match.CompletedGames.Count == 0 ) {
            ApiManager.Instance.Database.GetTournament( GameManager.Instance.CurrentTournament.Id.Id )
                .Then( tournament => {

                    GameManager.Instance.UpdateMetches( tournament );

                    if ( !match.CurrentGame.IsNull() ) {
                        nextTimeout = match.CurrentGame.NextTimeout == null
                            ? DateTime.UtcNow.AddSeconds( 15 )
                            : match.CurrentGame.NextTimeout.Value;

                        if ( match.CurrentGame.GameState == ChainTypes.GameState.ExpectingCommitMoves ||
                             match.CurrentGame.GameState == ChainTypes.GameState.ExpectingRevealMoves ) {
                            AudioManager.Instance.PlayWaitingSound();
                        }
                        if ( PlayerPrefs.GetInt( match.Id.ToString() ) == 0 ) {
                            AudioManager.Instance.PlayNoticeSound();
                            PlayerPrefs.SetInt( match.Id.ToString(), (int) match.Id.Id );
                        }

                        UpdateMoveButtons();
                    }
                    UpdateGameUi( match );

                } );
        }

        if ( !match.CurrentGame.IsNull() ) {
            nextTimeout = match.CurrentGame.NextTimeout == null
                ? DateTime.UtcNow.AddSeconds( 15 )
                : match.CurrentGame.NextTimeout.Value;

            if ( match.CurrentGame.GameState == ChainTypes.GameState.ExpectingCommitMoves ||
                 match.CurrentGame.GameState == ChainTypes.GameState.ExpectingRevealMoves ) {
                AudioManager.Instance.PlayWaitingSound();
            }
            if ( PlayerPrefs.GetInt( match.Id.ToString() ) == 0 ) {
                AudioManager.Instance.PlayNoticeSound();
                PlayerPrefs.SetInt( match.Id.ToString(), (int) match.Id.Id );
            }

            UpdateMoveButtons();
        }


        UpdateGameUi(GameManager.Instance.CurrentMatch);
        playerHand.enabled = true;
        opponentHand.enabled = true;
        StopTimer();
        UpdateTimer();

        UpdateJackpot(GameManager.Instance.CurrentMatch);
        UpdateUsernameText(GameManager.Instance.CurrentMatch.Me.Name, GameManager.Instance.CurrentMatch.Opponent.Name );
    }


    void UpdateGameUi( MatchContainer match ) {
        Refresh();
        winsToWinGame = 5;
        gamesToWinRoundText.text = winsToWinGame.ToString();
        foreach ( var game in match.CompletedGames ) {
            logController.LogNewRound( game.Value.Number, game.Value.OpponentGesture, game.Value.MeGesture,
                                      game.Value.Result );
            switch ( game.Value.Result ) {
                case GameResult.Win:
                    counterController.CountWin();
                    winsToWinGame--;
                    gamesToWinRoundText.text = winsToWinGame.ToString();

                    break;
                case GameResult.Lose:
                    counterController.CountLose();
                    break;
                case GameResult.Draw:
                    counterController.CountTie();
                    break;
            }
        }

        winsToWinTournament = ( ( (int) Math.Log( GameManager.Instance.GetTournamentNumberOfPlayers( match.Tournament ),
                                                 2 ) -
                                  GameManager.Instance.GetCompletedMatches( match.Tournament ) ) * 5 -
                                ( 5 - winsToWinGame ) );

        gamesToWinTournamentText.text = winsToWinTournament < 0 ? 0.ToString() : winsToWinTournament.ToString();

    }

    void UpdateJackpot( MatchContainer match ) {

        ApiManager.Instance.Database.GetMatche( match.Id.Id )
            .Then( matchResult => {
                ApiManager.Instance.Database.GetTournament( matchResult.Tournament.Id )
                    .Then( tournament => {
                        TournamentManager.Instance.GetAssetObject( tournament.Options.BuyIn.Asset.Id )
                            .Then( asset => {
                                jackpoText.text =
                                    Utils.GetFormatedDecimaNumber( ( tournament.PrizePool /
                                                                     Math.Pow( 10, asset.Precision ) ).ToString() ) +
                                    asset.Symbol;

                            } );
                    } );
            } );

    }

    void GameComplete( MatchContainer match, GameContainer game ) {
        if ( TournamentManager.Instance.CurrentTournament == null ) {
            return;
        }
        if ( !match.Tournament.Equals( TournamentManager.Instance.CurrentTournament.Id ) ) {
            return;
        }

        AudioManager.Instance.StopPlaying();
        StopTimer();
        buttonsObject.SetActive( false );

        if ( !gameObject.activeSelf ) {
            return;
        }

        if ( game.MeGesture.HasValue ) {
            playerHand.SetTrigger( RockPaperScissorsGestureEnumConverter.ConvertTo( game.MeGesture.Value ) );
        }
        if ( game.OpponentGesture.HasValue ) {
            opponentHand.SetTrigger( RockPaperScissorsGestureEnumConverter.ConvertTo( game.OpponentGesture.Value ) );
        }
        UpdateGameUi( match );
        StartCoroutine( BackToIdleCoroutine( 2f ) );
    }


    GameChoiseResult GetNewStep( GameContainer game ) {
        var step = new GameChoiseResult();
        step.roundNumber = game.Number;
        step.opponentGesture = game.OpponentGesture;
        step.playerGesture = game.MeGesture;
        step.result = game.Result;
        return step;
    }

    IEnumerator BackToIdleCoroutine( float delay ) {
        yield return new WaitForSecondsRealtime( delay );
        playerHand.SetTrigger( "idle" );
        opponentHand.SetTrigger( "idle" );
    }

    void UpdateTimer() {
        waitObject.SetActive( true );
        isTimer = true;
    }


    void StopTimer() {
        isTimer = false;
        waitObject.SetActive( false );

    }

    void Update() {
        if ( isTimer ) {
            var second = ( nextTimeout - DateTime.UtcNow ).Seconds;
            timerText.text = ":" + String.Format( "{0:00}", second );
            if (second <= 0 ) {
                StopTimer();
            }
        }
    }

}