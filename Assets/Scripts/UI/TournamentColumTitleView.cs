using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentColumTitleView : MonoBehaviour {

    public event Action<SortType, ButtonView> OnSortChange;

    [SerializeField] protected ButtonView gameIdButton;
    [SerializeField] protected ButtonView gameNameButton;
    [SerializeField] protected ButtonView playersButton;
    [SerializeField] protected ButtonView startTimeButton;
    [SerializeField] protected ButtonView buyInButton;
    [SerializeField] protected ButtonView jackpotButton;
    [SerializeField] protected ButtonView registerDeadlineButton;
    [SerializeField] protected ButtonView winnerButton;

    List<ButtonView> columnButtons = new List<ButtonView>();

    protected virtual void Awake() {

        AddButtonToList( gameIdButton );
        AddButtonToList( gameNameButton );
        AddButtonToList( playersButton );
        AddButtonToList( startTimeButton );
        AddButtonToList( buyInButton );
        AddButtonToList( jackpotButton );
        AddButtonToList( registerDeadlineButton );
        AddButtonToList( winnerButton );

        foreach ( var button in columnButtons ) {
            button.OnButtonClick += SwitchButtonState;
        }

        
    }

    void Start() {
        SwitchButtonState( registerDeadlineButton );
    }

    protected void SwitchButtonState( ButtonView target ) {
        foreach ( var button in columnButtons ) {
            if ( button.Equals( target ) ) {
                button.Currentstate = ButtonState.Pressed;
                CheckSortingButton( button );
            } else {
                button.Currentstate = ButtonState.Active;
            }
        }
    }

    void CheckSortingButton( ButtonView target ) {

        if ( target.Equals( startTimeButton ) ) {
            Sort_OnChange( SortType.StartTime, target);
        }

        if ( target.Equals( registerDeadlineButton ) ) {
            Sort_OnChange( SortType.RegisterDeadline, target);
        }

        if ( target.Equals( buyInButton ) ) {
            Sort_OnChange( SortType.BuyIn, target);
        }

        if ( target.Equals( jackpotButton ) ) {
            Sort_OnChange( SortType.Jackpot, target);
        }

        if ( target.Equals( gameNameButton ) ) {
            Sort_OnChange( SortType.Game, target);
        }

        if ( target.Equals( playersButton ) ) {
            Sort_OnChange( SortType.Players, target );
        }

        if ( target.Equals( gameIdButton ) ) {
            Sort_OnChange( SortType.GameId, target );
        }
    }

    void Sort_OnChange( SortType type, ButtonView button ) {
        if ( OnSortChange != null ) {
            OnSortChange( type, button );
        }
    }

    void AddButtonToList( ButtonView button ) {
        if ( button != null ) {
            columnButtons.Add( button );
        }
    }

}
