using System;
using System.Collections.Generic;
using Base;
using Base.Config;
using Base.Data;
using Base.Data.Accounts;
using Base.Data.Tournaments;
using Tools;


public class MatchContainer : IDisposable {

	public event Action<MatchContainer> OnMatchChanged = match => { };

	public event Action<MatchContainer, GameContainer> OnNewGame = ( match, game ) => { };
	public event Action<MatchContainer, GameContainer> OnGameComplete = ( match, game ) => { };
	public event Action<MatchContainer, GameContainer> OnGameExpectedMove = ( match, game ) => { };

	readonly int number;
	readonly AccountObject me;
	readonly AccountObject opponent;
	readonly Dictionary<SpaceTypeId, GameContainer> completedGames = new Dictionary<SpaceTypeId, GameContainer>();

	Action<MatchContainer> MatchComplete;

	MatchObject match;
	AccountObject winner;
	GameContainer currentGame;


	public SpaceTypeId Id {
		get { return match.Id; }
	}

	public SpaceTypeId Tournament {
		get { return match.Tournament; }
	}

	public GameContainer CurrentGame {
		get { return currentGame; }
	}

	public Dictionary<SpaceTypeId, GameContainer> CompletedGames {
		get { return completedGames; }
	}

	public int Number {
		get { return number; }
	}

	public AccountObject Me {
		get { return me; }
	}

	public AccountObject Opponent {
		get { return opponent; }
	}

	public AccountObject Winner {
		get { return winner; }
	}

	public ChainTypes.MatchState MatchState {
		get { return match.State; }
	}

	public MatchContainer( int number, MatchObject match, AccountObject me, AccountObject opponent, Action<MatchContainer> matchCompleteCallback = null ) {
		this.number = number;
		this.match = match;
		this.me = me;
		this.opponent = opponent;
		MatchComplete = matchCompleteCallback;
		Repository.OnObjectUpdate += Repository_OnObjectUpdate;
	}

	void Repository_OnObjectUpdate( IdObject idObject ) {
		if ( idObject.Equals( match ) ) {
			UpdateMatchInfo( idObject as MatchObject );
			return;
		}
		if ( idObject.SpaceType.Equals( SpaceType.Game ) ) {
			var game = idObject as GameObject;
			if ( !game.Match.Equals( match.Id ) ) {
				// not my game
				return;
			}
			if ( completedGames.ContainsKey( game.Id ) ) {
				// game is completed before
				completedGames[ game.Id ].UpdateGameInfo( game );
				return;
			}
			if ( !currentGame.IsNull() && currentGame.Id.Equals( game.Id ) ) {
				// game is current
				return;
			}
			// new game in this match
			NewGame( game ).CheckState();
		}
	}

	public GameContainer NewGame( GameObject game ) {
		if ( !currentGame.IsNull() && !currentGame.Id.Equals( game.Id ) ) {
			completedGames[ currentGame.Id ] = currentGame;
		}
		currentGame = new GameContainer( completedGames.Count + 1, game, me, opponent, gameExpectedMove => {
			OnGameExpectedMove.Invoke( this, gameExpectedMove );
		}, completedGame => {
			OnGameComplete.Invoke( this, completedGames[ completedGame.Id ] = completedGame );
		} );
		OnNewGame( this, currentGame );
		return currentGame;
	}

	public void UpdateMatchInfo( MatchObject updatedMatch ) {
		if ( updatedMatch.ToString().Equals( match.ToString() ) ) {
			return;
		}
		var currentState = MatchState;
		match = updatedMatch;
		if ( !match.State.Equals( currentState ) ) {
			// change state
			switch ( match.State ) {
			case ChainTypes.MatchState.Complete:
				// match complete
				if ( match.MatchWinners.OrEmpty().Length == 1 ) {
					if ( match.MatchWinners[ 0 ].Equals( me.Id ) ) {
						winner = me;
					} else
					if ( match.MatchWinners[ 0 ].Equals( opponent.Id ) ) {
						winner = opponent;
					}
				}
				if ( !MatchComplete.IsNull() ) {
					MatchComplete.Invoke( this );
				}
				Dispose();
				break;
			case ChainTypes.MatchState.InProgress:
				break;
			case ChainTypes.MatchState.WaitingOnPreviousMatches:
				break;
			}
		}
		OnMatchChanged.Invoke( this );
	}

	public void Dispose() {
		Repository.OnObjectUpdate -= Repository_OnObjectUpdate;
		MatchComplete = null;
	}
}