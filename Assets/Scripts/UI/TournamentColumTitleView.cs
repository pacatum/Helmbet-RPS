using System;
using System.Collections.Generic;
using UnityEngine;

public class TournamentColumTitleView : MonoBehaviour {

    public event Action<SortType, ButtonView> OnSortChange;

    [SerializeField] protected ButtonView playersButton;
    [SerializeField] protected ButtonView startTimeButton;
    [SerializeField] protected ButtonView buyInButton;
    [SerializeField] protected ButtonView registerDeadlineButton;

    List<ButtonView> columnButtons = new List<ButtonView>();


    protected virtual void Awake() {
        AddButtonToList( playersButton );
        AddButtonToList( startTimeButton );
        AddButtonToList( buyInButton );
        AddButtonToList( registerDeadlineButton );

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
            Sort_OnChange( SortType.StartTime, target );
        }
        if ( target.Equals( registerDeadlineButton ) ) {
            Sort_OnChange( SortType.RegisterDeadline, target );
        }
        if ( target.Equals( buyInButton ) ) {
            Sort_OnChange( SortType.BuyIn, target );
        }
        if ( target.Equals( playersButton ) ) {
            Sort_OnChange( SortType.Players, target );
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
