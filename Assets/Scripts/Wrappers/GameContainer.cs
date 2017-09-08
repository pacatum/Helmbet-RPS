using System;
using System.Collections.Generic;
using Base;
using Base.Config;
using Base.Data;
using Base.Data.Accounts;
using Base.Data.Tournaments;
using Base.Data.Tournaments.GameDetails;
using Base.Transactions.GameMoves;
using Tools;
using Gesture = Base.Config.ChainTypes.RockPaperScissorsGesture;


public class GameContainer : IDisposable {

	public event Action<GameContainer> OnGameChanged = game => { };

	readonly int number;
	readonly AccountObject me;
	readonly AccountObject opponent;
	readonly Dictionary<SpaceTypeId, Gesture?> gestures;
    readonly DateTime? nextTimeOut;

	Action<GameContainer> GameExpectedMove;
	Action<GameContainer> GameComplete;

	GameObject game;
	GameMoveTransactionService.CommitAndReveal currentCommitAndReveal;
	AccountObject winner;


	public SpaceTypeId Id {
		get { return game.Id; }
	}

	public int Number {
		get { return number; }
	}

	public ChainTypes.GameState GameState {
		get { return game.State; }
	}

    public DateTime? NextTimeout {
        get { return nextTimeOut; }
    }

	public GameResult Result {
		get {
			if ( !winner.IsNull() ) {
				if ( winner.Id.Equals( me.Id ) ) {
					return GameResult.Win;
				}
				if ( winner.Id.Equals( opponent.Id ) ) {
					return GameResult.Lose;
				}
			}
			return GameResult.Draw;
		}
	}

	public Gesture? MeGesture {
		get { return gestures[ me.Id ]; }
	}

	public Gesture? OpponentGesture {
		get { return gestures[ opponent.Id ]; }
	}

	public GameContainer( int number, GameObject game, AccountObject me, AccountObject opponent, Action<GameContainer> gameExpectedMoveCallback = null, Action<GameContainer> gameCompleteCallback = null ) {
		this.number = number;
		this.game = game;
	    this.nextTimeOut = game.NextTimeout;
		this.me = me;
		this.opponent = opponent;
		gestures = new Dictionary<SpaceTypeId, Gesture?> {
			{ me.Id,        null },
			{ opponent.Id,  null }
		};
		GameExpectedMove = gameExpectedMoveCallback;
		GameComplete = gameCompleteCallback;
		Repository.OnObjectUpdate += Repository_OnObjectUpdate;
	}

	void Repository_OnObjectUpdate( IdObject idObject ) {
		if ( idObject.Equals( game ) ) {
			UpdateGameInfo( idObject as GameObject );
		}
	}

	public void UpdateGameInfo( GameObject updatedGame ) {
		if ( updatedGame.ToString().Equals( game.ToString() ) ) {
			return;
		}
		var currentState = GameState;
		game = updatedGame;
		if ( !game.State.Equals( currentState ) ) {
			// change state
			CheckState();
		}
		OnGameChanged.Invoke( this );
	}

	public void CheckState() {
		switch ( game.State ) {
		case ChainTypes.GameState.Complete:
			// game complete
			var revealMoves = (game.GameDetails as RockPaperScissorsGameDetailsData).RevealMoves;
			for ( var i = 0; i < game.Players.Length; i++ ) {
				gestures[ game.Players[ i ] ] = revealMoves.IsNullOrEmpty() ? null : revealMoves[ i ].IsNull() ? null : new Gesture?( revealMoves[ i ].Gesture );
			}
			if ( game.Winners.OrEmpty().Length == 1 ) {
				if ( game.Winners.First().Equals( me.Id ) ) {
					winner = me;
				} else
				if ( game.Winners.First().Equals( opponent.Id ) ) {
					winner = opponent;
				}
			}
			if ( !GameComplete.IsNull() ) {
				GameComplete.Invoke( this );
			}
			Dispose();
			break;
		case ChainTypes.GameState.ExpectingCommitMoves:
			// wait commit
			if ( !currentCommitAndReveal.IsNull() ) {
				currentCommitAndReveal.Commit( game );
			} else if ( !GameExpectedMove.IsNull() ) {
				GameExpectedMove.Invoke( this );
			}
			break;
		case ChainTypes.GameState.ExpectingRevealMoves:
			// wait reveal
			if ( !currentCommitAndReveal.IsNull() ) {
				currentCommitAndReveal.Reveal();
			}
			break;
		}
	}

	public void MakeMove( Gesture gesture ) {
		if ( !currentCommitAndReveal.IsNull() ) {
			return;
		}
		currentCommitAndReveal = new GameMoveTransactionService.CommitAndReveal( gesture, me.Id );
		if ( game.State.Equals( ChainTypes.GameState.ExpectingCommitMoves ) ) {
			currentCommitAndReveal.Commit( game );
		}
	}

	public void Dispose() {
		Repository.OnObjectUpdate -= Repository_OnObjectUpdate;
		GameExpectedMove = null;
		GameComplete = null;
	}
}