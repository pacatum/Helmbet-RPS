using System;
using System.Collections.Generic;


public class UIManager : SingletonMonoBehaviour<UIManager> {

    public enum ScreenState {

        Login /*               */ = 0,
        Dashboard /*           */ = 1,
        GameFind /*            */ = 2,
        DepositWithdraw /*     */ = 3,
        Settings /*            */ = 4,
        CreateNew /*           */ = 5,
        Game /*                */ = 6,
        Account /*             */ = 7,
        TournamentDetails /*   */ = 8,
        GameStartPreview /*    */ = 9,
        GameInfo = 10

    }


    public event Action<ScreenState> OnStateChanged;
	
	ScreenState currentState;
	List<int> history = new List<int>();


	protected override void Awake() {
		base.Awake();
		CurrentState = ScreenState.Login;
	}

	public ScreenState CurrentState {
		get { return currentState; }
		set {
			if ( currentState != value ) {
				PreviousState = currentState;
				history.Add( ( int )currentState );
				currentState = value;
				if ( OnStateChanged != null ) {
					OnStateChanged( currentState );
				}
			}
		}
	}

	public ScreenState PreviousState { get; protected set; }

	public void ClearHistory() {
		history.Clear();
		CurrentState = ScreenState.Login;
	}
}