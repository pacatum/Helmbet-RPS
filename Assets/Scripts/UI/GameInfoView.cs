using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoView : BaseCanvasView {

    [SerializeField] ButtonView aboutGameButton;
    [SerializeField] ButtonView creatingTournamentButton;
    [SerializeField] ButtonView howToPlayButton;
    [SerializeField] ButtonView payoutsButton;
    [SerializeField] ButtonView tournamentHistoryButton;
    [SerializeField] ButtonView settingButton;

    [SerializeField] ScrollRect scrollView;
    [SerializeField] float fisrtScrollYPosition;
    [SerializeField] float secondScrollYPosition;
    [SerializeField] float thirdScrollYPosition;
    [SerializeField] float fourthScrollYPosition;
    [SerializeField] Button backButton;

    List<ButtonView> buttons = new List<ButtonView>();


    public override void Awake() {
        backButton.onClick.AddListener( OpenPreviousView );

        buttons.Add( aboutGameButton );
        buttons.Add( creatingTournamentButton );
        buttons.Add( howToPlayButton );
        buttons.Add( payoutsButton );
        buttons.Add( tournamentHistoryButton );
        buttons.Add( settingButton );

        foreach ( var button in buttons ) {
            button.OnButtonClick += SwitchButton;
        }

        aboutGameButton.GetComponent<Button>().onClick.AddListener( ScrollToAboutGameText );
        creatingTournamentButton.GetComponent<Button>().onClick.AddListener( ScrollToCreatingTournamentText );
        howToPlayButton.GetComponent<Button>().onClick.AddListener( ScrollToHowToPlayText );
        payoutsButton.GetComponent<Button>().onClick.AddListener( delegate { ScrollToTheEnd( payoutsButton ); } );
        tournamentHistoryButton.GetComponent<Button>().onClick.AddListener( delegate { ScrollToTheEnd( tournamentHistoryButton ); } );
        settingButton.GetComponent<Button>().onClick.AddListener( delegate { ScrollToTheEnd( settingButton ); } );
        scrollView.onValueChanged.AddListener( UpdateScrollPosition );
        Clear();
    }

    void OpenPreviousView() {
        UIManager.Instance.CurrentState = UIManager.Instance.PreviousState;
    }

    public override void Hide() {
        Clear();
        base.Hide();
    }

    void SwitchButton( ButtonView target ) {
        foreach ( var button in buttons ) {
            if ( button.Equals( target ) ) {
                button.Currentstate = ButtonState.Pressed;
            } else {
                button.Currentstate = ButtonState.Active;
            }
        }
    }

    void ScrollToAboutGameText() {
        scrollView.content.anchoredPosition = new Vector2( scrollView.content.anchoredPosition.x, fisrtScrollYPosition );
    }

    void ScrollToCreatingTournamentText() {
        scrollView.content.anchoredPosition = new Vector2( scrollView.content.anchoredPosition.x, secondScrollYPosition );
    }

    void ScrollToHowToPlayText() {
        scrollView.content.anchoredPosition = new Vector2( scrollView.content.anchoredPosition.x, thirdScrollYPosition );
    }

    void ScrollToTheEnd( ButtonView button ) {
        scrollView.content.anchoredPosition = new Vector2( scrollView.content.anchoredPosition.x, fourthScrollYPosition );
        SwitchButton( button );
    }

    void UpdateScrollPosition( Vector2 position ) {
        var contentPosition = scrollView.content.anchoredPosition.y;
        if ( contentPosition < secondScrollYPosition ) {
            SwitchButton( aboutGameButton );
        } else if ( contentPosition >= secondScrollYPosition && contentPosition < thirdScrollYPosition ) {
            SwitchButton( creatingTournamentButton );
        } else if ( contentPosition >= thirdScrollYPosition && contentPosition < fourthScrollYPosition ) {
            SwitchButton( howToPlayButton );
        } else if(contentPosition>= fourthScrollYPosition) {
            if ( settingButton.Currentstate != ButtonState.Pressed && tournamentHistoryButton.Currentstate != ButtonState.Pressed )
                SwitchButton( payoutsButton );
        }
    }

    void Clear() {
        SwitchButton( aboutGameButton );
        ScrollToAboutGameText();
    }

}
