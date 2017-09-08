using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Gesture = Base.Config.ChainTypes.RockPaperScissorsGesture;


public class GameButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] Sprite rockSprite;
    [SerializeField] Sprite paperSprite;
    [SerializeField] Sprite scissorsSprite;
    [SerializeField] Sprite undefinedSprite;

    [SerializeField] GameObject normal;
    [SerializeField] GameObject hover;

    [SerializeField] Image normalState;
    [SerializeField] Image hoverState;
    [SerializeField] Text roundNumberText;
    [SerializeField] Text stepNameText;

    [SerializeField] private Image hoverBg;
    [SerializeField] private Color paperColor;
    [SerializeField] private Color rockColor;
    [SerializeField] private Color scissorsColor;
    [SerializeField] private Color undefinedColor;

    public bool IsSelect { get; set; }

    void Awake() {
        SetNormalView();
    }

    public void OnPointerEnter( PointerEventData eventData ) {
        SetHoverView();
    }

    public void OnPointerExit( PointerEventData eventData ) {
        SetNormalView();
    }

    public void SetNormalView() {
        normal.SetActive( true );
        hover.SetActive( false );
    }

    public void SetHoverView() {
        normal.SetActive( false );
        hover.SetActive( true );
    }

    public void SetStep( int roundNumber, Gesture? choise ) {
        roundNumberText.text = roundNumber.ToString();
        UpdateImage( choise );
    }

    void UpdateImage( Gesture? choise ) {
        if ( choise.HasValue ) {
            switch ( choise.Value ) {
                case Gesture.Scissors:
                    normalState.sprite = scissorsSprite;
                    hoverState.sprite = scissorsSprite;
                    hoverBg.color = scissorsColor;
                    stepNameText.text = "SCISSORS";
                    break;
                case Gesture.Rock:
                    normalState.sprite = rockSprite;
                    hoverState.sprite = rockSprite;
                    stepNameText.text = "ROCK";
                    hoverBg.color = rockColor;
                    break;
                case Gesture.Paper:
                    normalState.sprite = paperSprite;
                    hoverState.sprite = paperSprite;
                    stepNameText.text = "PAPER";
                    hoverBg.color = paperColor;
                    break;
            }
        } else {
            normalState.sprite = undefinedSprite;
            hoverState.sprite = undefinedSprite;
            stepNameText.text = "UNDEFINED";
            hoverBg.color = undefinedColor;
        }
    }

}