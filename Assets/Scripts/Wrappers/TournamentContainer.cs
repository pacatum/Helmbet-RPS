using System;
using System.Collections.Generic;
using Base;
using Base.Config;
using Base.Data;
using Base.Data.Accounts;
using Base.Data.Tournaments;
using Tools;


public class TournamentContainer {

	readonly Dictionary<SpaceTypeId, MatchContainer> completedMatches = new Dictionary<SpaceTypeId, MatchContainer>();
	readonly List<MatchObject> startedMatches = new List<MatchObject>();

	readonly Action<MatchContainer> newMatchCallback = match => { };
	readonly Action<MatchContainer> matchCompleteCallback = match => { };

	MatchContainer currentMatch;
	TournamentObject tournament;
	TournamentDetailsObject tournamentDetails;


	public SpaceTypeId Id {
		get { return tournament.Id; }
	}

	public ChainTypes.TournamentState TournamentState {
		get { return tournament.State; }
	}

	public MatchContainer CurrentMatch {
		get { return currentMatch; }
	}

	public SpaceTypeId[] StartedMatches {
		get { return Array.ConvertAll( startedMatches.ToArray(), match => match.Id ); }
	}

	public Dictionary<SpaceTypeId, MatchContainer> CompletedMatches {
		get { return completedMatches; }
	}

	public TournamentContainer( Action<MatchContainer> newMatchCallback, Action<MatchContainer> matchCompleteCallback ) {
		this.newMatchCallback = newMatchCallback ?? (match => { });
		this.matchCompleteCallback = matchCompleteCallback ?? (match => { });
	}

	public void UpdateTournamentInfo( TournamentObject tournament, TournamentDetailsObject tournamentDetails ) {
		this.tournament = tournament;
		this.tournamentDetails = tournamentDetails;
	}

	public void NewMatch( MatchObject newMatch, AccountObject me, AccountObject opponent, Action<MatchContainer, GameContainer> newGameCallback, Action<MatchContainer, GameContainer> gameCompleteCallback, Action<MatchContainer, GameContainer> gameExpectedMoveCallback ) {
		startedMatches.Add( newMatch );
		var game = newMatch.Games.Last();
		Repository.GetInPromise( game, () => ApiManager.Instance.Database.GetGame( game.Id ) ).Then( newGame => {
			if ( !currentMatch.IsNull() && !currentMatch.Id.Equals( newMatch.Id ) ) {
				completedMatches[ currentMatch.Id ] = currentMatch;
			}
			currentMatch = new MatchContainer( completedMatches.Count + 1, newMatch, me, opponent, completedMatch => {
				matchCompleteCallback.Invoke( completedMatches[ completedMatch.Id ] = completedMatch );
			} );
			newMatchCallback.Invoke( currentMatch );
			currentMatch.OnNewGame += newGameCallback;
			currentMatch.OnGameComplete += gameCompleteCallback;
			currentMatch.OnGameExpectedMove += gameExpectedMoveCallback;
			currentMatch.NewGame( newGame ).CheckState();
		} ).Catch( error => Unity.Console.Error( Unity.Console.SetRedColor( error ) ) );
	}

	public uint NumberOfPlayers {
		get { return tournament.Options.NumberOfPlayers; }
	}
}