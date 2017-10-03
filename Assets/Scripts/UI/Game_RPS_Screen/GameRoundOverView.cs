using Base.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameRoundOverState {

    Win,
    Lose,
    RoundOver

}

public class GameRoundOverView : MonoBehaviour {

    GameRoundOverState currentGameRoundOverState;
    [SerializeField] GameObject looseRoundView;
    [SerializeField] GameObject winRoundView;
    [SerializeField] GameObject roundOverView;
    [SerializeField] GameObject winText;
    [SerializeField] GameObject looseText;
    [SerializeField] Button dashboardButton;

    [SerializeField] Sprite paperGesture;
    [SerializeField] Sprite rockGesture;
    [SerializeField] Sprite scissorsGesture;
    [SerializeField] Sprite undefinedGesture;

    [SerializeField] Sprite paperColorGesture;
    [SerializeField] Sprite rockColorGesture;
    [SerializeField] Sprite scissorsColorGesture;

    [SerializeField] Color loseTextColor;
    [SerializeField] Color undefinedTextColor;
    [SerializeField] Color paperWinTextColor;
    [SerializeField] Color rockWinTextColor;
    [SerializeField] Color scissorsWinTextColor;

    [SerializeField] RoundOver roundOverPrefab;
    [SerializeField] Transform roundLogContentHolder;

    [SerializeField] GameScreenView gameScreenView;
    [SerializeField] Text matchesCountText;

    [SerializeField] Text opponentUsernameText;


    void Awake() {
        dashboardButton.onClick.AddListener( GoToDashboard );
    }

    public GameRoundOverState CurrentGameRoundOverState {
        get { return currentGameRoundOverState; }
        set {
            currentGameRoundOverState = value;
            UpdateState( currentGameRoundOverState );
        }
    }

    void UpdateState( GameRoundOverState state ) {
        looseRoundView.SetActive( false );
        winRoundView.SetActive( false );
        roundOverView.SetActive( false );
        winText.SetActive(false);
        looseText.SetActive(false);
        switch ( state ) {
            case GameRoundOverState.Lose:
                looseRoundView.SetActive( true );
                looseText.SetActive( true );
                break;
            case GameRoundOverState.Win:
                winRoundView.SetActive( true );
                winText.SetActive( true );
                break;
            case GameRoundOverState.RoundOver:
                roundOverView.SetActive( true );
                break;
        }
    }

    void GoToDashboard() {
        UIManager.Instance.CurrentState = UIManager.ScreenState.Dashboard;
        gameScreenView.Refresh();
        Hide();
    }

    public void Show( List<GameChoiseResult> history ) {
        gameObject.SetActive( true );
        UpdateHistory( history );
    }

    public void UpdateOpponentUsername( string opponentUsername ) {
        opponentUsernameText.text = "ROUND AGAINST: " + opponentUsername;
    }


    void UpdateHistory( List<GameChoiseResult> history ) {
        foreach ( var item in history ) {

            var gameOverView = Instantiate( roundOverPrefab );
            gameOverView.transform.SetParent( roundLogContentHolder, false );
            gameOverView.SetUpRoundOver( GetPlayerGesture( item.playerGesture, item.result ), GetOpponentGesture( item.opponentGesture, item.result ),item);
        }
    }

    Sprite GetPlayerGesture( ChainTypes.RockPaperScissorsGesture? state, GameResult result ) {
        if ( state == null ) {
            return undefinedGesture;
        }
        switch ( state ) {
            case ChainTypes.RockPaperScissorsGesture.Rock:
                return result == GameResult.Win ? rockColorGesture : rockGesture;
            case ChainTypes.RockPaperScissorsGesture.Scissors:
                return result == GameResult.Win ? scissorsColorGesture: scissorsGesture;
            case ChainTypes.RockPaperScissorsGesture.Paper:
                return result == GameResult.Win ?paperColorGesture: paperGesture;
            default:
                return undefinedGesture;
        }
    }

    Sprite GetOpponentGesture( ChainTypes.RockPaperScissorsGesture? state, GameResult result ) {
        if ( state == null ) {
            return undefinedGesture;
        }
        switch ( state ) {
            case ChainTypes.RockPaperScissorsGesture.Rock:
                return result == GameResult.Lose ? rockColorGesture : rockGesture;
            case ChainTypes.RockPaperScissorsGesture.Scissors:
                return result == GameResult.Lose ? scissorsColorGesture : scissorsGesture;
            case ChainTypes.RockPaperScissorsGesture.Paper:
                return result == GameResult.Lose ? paperColorGesture : paperGesture;
            default:
                return undefinedGesture;
        }
    }

    public void Hide() {
        gameObject.SetActive( false );
    }

    public void ClearHistory() {

        for ( int i = 0; i < roundLogContentHolder.childCount; i++ ) {
            Destroy( roundLogContentHolder.GetChild( i ).gameObject );
        }

        roundLogContentHolder.GetComponent<RectTransform>().anchoredPosition = new Vector2(roundLogContentHolder.GetComponent<RectTransform>().anchoredPosition.x, 0f);
    }

    public void SetMatchesInProgressCount( int count ) {
        matchesCountText.text = count.ToString();
    }

}
