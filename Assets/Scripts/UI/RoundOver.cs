using Base.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundOver : MonoBehaviour {

    [SerializeField] Image playerGestureImage;
    [SerializeField] Image opponentGestureImage;
    [SerializeField] Text roundNumberText;
    [SerializeField] TextMeshProUGUI playerChoiseText;
    [SerializeField] TextMeshProUGUI opponentChoiseText;

    [SerializeField] Color loseTextColor;
    [SerializeField] Color undefinedTextColor;
    [SerializeField] Color paperWinTextColor;
    [SerializeField] Color rockWinTextColor;
    [SerializeField] Color scissorsWinTextColor;

    public void SetUpRoundOver( Sprite playerGestureSprite, Sprite opponentGestureSprite, GameChoiseResult choiseResult ) {
        playerGestureImage.sprite = playerGestureSprite;
        opponentGestureImage.sprite = opponentGestureSprite;
        roundNumberText.text = choiseResult.roundNumber.ToString();

        UpdateStepText( choiseResult );

        switch ( choiseResult.result ) {
            case GameResult.Win:
                opponentChoiseText.color = choiseResult.opponentGesture == null ? undefinedTextColor : loseTextColor;
                UpdateTextColor( choiseResult.playerGesture, playerChoiseText );
                break;
            case GameResult.Lose:
                playerChoiseText.color = choiseResult.playerGesture == null ? undefinedTextColor : loseTextColor;
                UpdateTextColor( choiseResult.opponentGesture, opponentChoiseText );
                break;
            case GameResult.Draw:
                playerChoiseText.color = choiseResult.playerGesture == null ? undefinedTextColor : loseTextColor;
                opponentChoiseText.color = choiseResult.opponentGesture == null ? undefinedTextColor : loseTextColor;
                break;
        }
    }

    void UpdateTextColor( ChainTypes.RockPaperScissorsGesture? gesture, TextMeshProUGUI text ) {
        if ( gesture == null ) {
            text.color = undefinedTextColor;
        }

        switch ( gesture ) {
            case ChainTypes.RockPaperScissorsGesture.Paper:
                text.color = paperWinTextColor;
                break;
            case ChainTypes.RockPaperScissorsGesture.Scissors:
                text.color = scissorsWinTextColor;
                break;
            case ChainTypes.RockPaperScissorsGesture.Rock:
                text.color = rockWinTextColor;
                break;

        }
    }

    void UpdateStepText( GameChoiseResult choiseResult ) {
        if ( choiseResult.opponentGesture == null ) {
            opponentChoiseText.text = "OVERDUED";
        }

        if ( choiseResult.playerGesture == null ) {
            playerChoiseText.text = "OVERDUED";
        }

        switch ( choiseResult.opponentGesture ) {
            case ChainTypes.RockPaperScissorsGesture.Paper:
                opponentChoiseText.text = "PAPER";
                break;
            case ChainTypes.RockPaperScissorsGesture.Rock:
                opponentChoiseText.text = "ROCK";
                break;
            case ChainTypes.RockPaperScissorsGesture.Scissors:
                opponentChoiseText.text = "SCISSORS";
                break;
            default:
                opponentChoiseText.text = "OVERDUED";
                break;
        }
        switch ( choiseResult.playerGesture ) {
            case ChainTypes.RockPaperScissorsGesture.Paper:
                playerChoiseText.text = "PAPER";
                break;
            case ChainTypes.RockPaperScissorsGesture.Rock:
                playerChoiseText.text = "ROCK";
                break;
            case ChainTypes.RockPaperScissorsGesture.Scissors:
                playerChoiseText.text = "SCISSORS";
                break;
            default:
                playerChoiseText.text = "OVERDUED";
                break;
        }
    }

}
