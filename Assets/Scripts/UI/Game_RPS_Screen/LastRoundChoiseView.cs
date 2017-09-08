using Base.Config;
using UnityEngine;
using UnityEngine.UI;
using Gesture = Base.Config.ChainTypes.RockPaperScissorsGesture;

public class LastRoundChoiseView : MonoBehaviour {

    [SerializeField] Text roundNumberText;

    [SerializeField] Image opponentChoiseItemBgImage;
    [SerializeField] Image opponentChoiseItemGesture;
    [SerializeField] Text opponentChoiseText;

    [SerializeField] Image playerChoiseItemBgImage;
    [SerializeField] Image playerChoiseItemGesture;
    [SerializeField] Text playerChoiseText;

    [SerializeField] Color rockColor;
    [SerializeField] Color paperColor;
    [SerializeField] Color scissorsColor;
    [SerializeField] Color looseColor;

    [SerializeField] Sprite rockSprite;
    [SerializeField] Sprite paperSprite;
    [SerializeField] Sprite scissorsSprite;
    [SerializeField] Sprite undefinedSprite;

    [SerializeField] Text resultText;
    [SerializeField] Color winResultTextColor;
    [SerializeField] Color looseResultTextColor;



    public void SetLastStep( int roundNumber, ChainTypes.RockPaperScissorsGesture? playerGesture,
                             ChainTypes.RockPaperScissorsGesture? opponentGesture, GameResult result ) {
        roundNumberText.text = "GAME " + roundNumber;
        if ( result == GameResult.Win ) {
            resultText.text = "YOU WON";
            resultText.color = winResultTextColor;
        } else {
            resultText.text = "YOU LOOSE";
            resultText.color = looseColor;
        }
        UpdateOpponentImage( opponentGesture, result );
        UpdatePlayerImage( playerGesture, result );
    }

    void UpdateOpponentImage( Gesture? choise, GameResult result ) {
        if ( choise.HasValue ) {
            switch ( choise.Value ) {
                case Gesture.Scissors:
                    opponentChoiseText.text = "SCISSORS";
                    opponentChoiseItemBgImage.color = opponentChoiseItemGesture.color =
                        opponentChoiseText.color = result == GameResult.Lose ? scissorsColor : looseColor;
                    opponentChoiseItemGesture.sprite = scissorsSprite;
                    break;
                case Gesture.Rock:
                    opponentChoiseText.text = "ROCK";
                    opponentChoiseItemBgImage.color = opponentChoiseItemGesture.color =
                        opponentChoiseText.color = result == GameResult.Lose ? rockColor : looseColor;
                    opponentChoiseItemGesture.sprite = rockSprite;
                    break;
                case Gesture.Paper:
                    opponentChoiseText.text = "PAPER";
                    opponentChoiseItemBgImage.color = opponentChoiseItemGesture.color =
                        opponentChoiseText.color = result == GameResult.Lose ? paperColor : looseColor;
                    opponentChoiseItemGesture.sprite = paperSprite;
                    break;
            }
        } else {
            opponentChoiseItemBgImage.color = looseColor;
            opponentChoiseText.text = "UNDEFINED";
            opponentChoiseItemGesture.sprite = null;
            opponentChoiseText.color = looseColor;
            opponentChoiseItemGesture.color = looseColor;
        }
    }

    void UpdatePlayerImage( Gesture? choise, GameResult result ) {
        if ( choise.HasValue ) {
            switch ( choise.Value ) {
                case Gesture.Scissors:
                    playerChoiseText.text = "SCISSORS";
                    playerChoiseItemBgImage.color = playerChoiseItemGesture.color =
                        playerChoiseText.color = result == GameResult.Win ? scissorsColor : looseColor;
                    playerChoiseItemGesture.sprite = scissorsSprite;
                    break;
                case Gesture.Rock:
                    playerChoiseText.text = "ROCK";
                    playerChoiseItemBgImage.color = playerChoiseItemGesture.color =
                        playerChoiseText.color = result == GameResult.Win ? rockColor : looseColor;
                    playerChoiseItemGesture.sprite = rockSprite;
                    break;
                case Gesture.Paper:
                    playerChoiseText.text = "PAPER";
                    playerChoiseItemBgImage.color = playerChoiseItemGesture.color =
                        playerChoiseText.color = result == GameResult.Win ? paperColor : looseColor;
                    playerChoiseItemGesture.sprite = paperSprite;
                    break;
            }
        } else {
            playerChoiseItemBgImage.color = looseColor;
            playerChoiseText.text = "UNDEFINED";
            playerChoiseItemGesture.sprite = null;
            playerChoiseText.color = looseColor;
            playerChoiseItemGesture.color = looseColor;
        }
    }

}
