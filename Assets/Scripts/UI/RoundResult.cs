using Base.Config;
using UnityEngine;
using UnityEngine.UI;

public class RoundResult : MonoBehaviour {

    public Sprite PaperBigGesture;
    public Sprite RockBigGesture;
    public Sprite ScissorsBigGesture;

    public Color PaperColour;
    public Color RockColour;
    public Color ScissorsColour;
    public Color InactiveColour;
    public Color WinColour;
    public Color LooseColour;

    public Image PlayerGesture;
    public Image PlayerEclipse;
    public Text PlayerText;
    public Image OpponentGesture;
    public Image OpponentEclipse;
    public Text OpponentText;
    public Text RoundNumberText;
    public Text WhoWonText;


    public void SetUpRoundResult( ChainTypes.RockPaperScissorsGesture playerGesture,
                                  ChainTypes.RockPaperScissorsGesture opponentGesture, int roundNumber,
                                  bool isPlayerWon ) {
        PlayerGesture.sprite = GetSpriteByGesture( playerGesture );
        PlayerText.text = playerGesture.ToString().ToUpper();
        var playerColour = isPlayerWon
            ? GetColorByGesture( playerGesture )
            : GetColorByGesture( ChainTypes.RockPaperScissorsGesture.Lizard );
        PlayerGesture.color = PlayerEclipse.color = PlayerText.color = playerColour;

        OpponentGesture.sprite = GetSpriteByGesture( opponentGesture );
        OpponentText.text = opponentGesture.ToString().ToUpper();
        var opponentColour = !isPlayerWon
            ? GetColorByGesture( opponentGesture )
            : GetColorByGesture( ChainTypes.RockPaperScissorsGesture.Lizard );
        OpponentGesture.color = OpponentEclipse.color = OpponentText.color = opponentColour;

        RoundNumberText.text = "ROUND " + roundNumber;
        WhoWonText.text = isPlayerWon ? "YOU WON" : "YOU LOOSE";
        WhoWonText.color = isPlayerWon ? WinColour : LooseColour;
    }

    private Sprite GetSpriteByGesture( ChainTypes.RockPaperScissorsGesture gesture ) {
        switch ( gesture ) {
            case ChainTypes.RockPaperScissorsGesture.Scissors:
                return ScissorsBigGesture;
            case ChainTypes.RockPaperScissorsGesture.Rock:
                return RockBigGesture;
            case ChainTypes.RockPaperScissorsGesture.Paper:
                return PaperBigGesture;
            default:
                return new Sprite();
        }
    }

    private Color GetColorByGesture( ChainTypes.RockPaperScissorsGesture gesture ) {
        switch ( gesture ) {
            case ChainTypes.RockPaperScissorsGesture.Scissors:
                return ScissorsColour;
            case ChainTypes.RockPaperScissorsGesture.Rock:
                return RockColour;
            case ChainTypes.RockPaperScissorsGesture.Paper:
                return PaperColour;
            default:
                return InactiveColour;
        }
    }



}
