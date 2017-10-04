using System.Collections.Generic;
using Base.Data.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gesture = Base.Config.ChainTypes.RockPaperScissorsGesture;


public class RoundChoiseItemView : MonoBehaviour {

    public Sprite ScissorsSprite;
    public Sprite PaperSprite;
    public Sprite RockSprite;
    public Sprite UndefinedSprite;

    [SerializeField] Image anotherPlayerChoiseIcon;
    [SerializeField] Text anotherPlayerChoiseText;
    [SerializeField] Image thisPlayerChoiseIcon;
    [SerializeField] Text thisPlayerChoiseText;
    [SerializeField] TextMeshProUGUI equalsSymbolText;

    [SerializeField] GameObject roundTieResultIcon;
    [SerializeField] GameObject roundWinResultIcon;
    [SerializeField] GameObject roundLooseResultIcon;

    List<GameObject> roundResultImages = new List<GameObject>();
    GameResult currentResultState;


    public string EqualsSymbol {
        get { return equalsSymbolText == null ? string.Empty : equalsSymbolText.text; }
        protected set {
            if ( equalsSymbolText != null ) {
                equalsSymbolText.text = value;
            }
        }
    }

    void Awake() {
        roundResultImages.Add( roundWinResultIcon );
        roundResultImages.Add( roundLooseResultIcon );
        roundResultImages.Add( roundTieResultIcon );
    }

    void OnEnable() {
        UpdateResultImage( currentResultState );
    }

    public void UpdateItem( Gesture? opponentChoise, Gesture? playerChoise, GameResult state ) {
        UpdateResultImage( state );

        if ( opponentChoise.HasValue ) {
            switch ( opponentChoise.Value ) {
                case Gesture.Scissors:
                    anotherPlayerChoiseIcon.sprite = ScissorsSprite;
                    break;
                case Gesture.Rock:
                    anotherPlayerChoiseIcon.sprite = RockSprite;
                    break;
                case Gesture.Paper:
                    anotherPlayerChoiseIcon.sprite = PaperSprite;
                    break;
            }
        } else {
            anotherPlayerChoiseIcon.sprite = UndefinedSprite;
        }

        if ( playerChoise.HasValue ) {
            switch ( playerChoise.Value ) {
                case Gesture.Scissors:
                    thisPlayerChoiseIcon.sprite = ScissorsSprite;
                    break;
                case Gesture.Rock:
                    thisPlayerChoiseIcon.sprite = RockSprite;
                    break;
                case Gesture.Paper:
                    thisPlayerChoiseIcon.sprite = PaperSprite;
                    break;
            }
        } else {
            thisPlayerChoiseIcon.sprite = UndefinedSprite;
        }
        currentResultState = state;
        anotherPlayerChoiseText.text = opponentChoise.HasValue ? RockPaperScissorsGestureEnumConverter.ConvertTo( opponentChoise.Value ).ToUpper() : "OVERDUED";
        thisPlayerChoiseText.text = playerChoise.HasValue ? RockPaperScissorsGestureEnumConverter.ConvertTo( playerChoise.Value ).ToUpper() : "OVERDUED";
    }

    void UpdateResultImage( GameResult state ) {
        switch ( state ) {
            case GameResult.Win:
                SwitchResultImage( roundLooseResultIcon );
                EqualsSymbol = ">";
                break;
            case GameResult.Lose:
                SwitchResultImage( roundWinResultIcon );
                EqualsSymbol = "<";
                break;
            case GameResult.Draw:
                SwitchResultImage( roundTieResultIcon );
                EqualsSymbol = "=";
                break;
        }
    }

    void SwitchResultImage( GameObject target ) {
        foreach ( var item in roundResultImages ) {
            item.SetActive( false );
        }
        target.SetActive( true );
    }

}