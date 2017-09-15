using Base.Config;
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
    [SerializeField] Button dashboardButton;

    [SerializeField] Sprite paperGesture;
    [SerializeField] Sprite rockGesture;
    [SerializeField] Sprite scissorsGesture;
    [SerializeField] Sprite undefinedGesture;

    [SerializeField] RoundOver roundOverPrefab;
    [SerializeField] Transform roundLogContentHolder;

    [SerializeField] GameScreenView gameScreenView;
    [SerializeField] Text matchesCountText;


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
        switch ( state ) {
            case GameRoundOverState.Lose:
                looseRoundView.SetActive( true );
                break;
            case GameRoundOverState.Win:
                winRoundView.SetActive( true );
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


    void UpdateHistory( List<GameChoiseResult> history ) {


        foreach ( var item in history ) {
            var gameOverView = Instantiate( roundOverPrefab );
            gameOverView.transform.SetParent( roundLogContentHolder, false );
            gameOverView.SetUpRoundOver( GetGesture( item.playerGesture ), GetGesture( item.opponentGesture ),
                                        item.roundNumber );
        }
    }

    Sprite GetGesture( ChainTypes.RockPaperScissorsGesture? state ) {
        if ( state == null ) {
            return undefinedGesture;
        }
        switch ( state ) {
            case ChainTypes.RockPaperScissorsGesture.Rock:
                return rockGesture;
            case ChainTypes.RockPaperScissorsGesture.Scissors:
                return scissorsGesture;
            case ChainTypes.RockPaperScissorsGesture.Paper:
                return paperGesture;
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
