using System;
using System.Collections.Generic;
using Base;
using Base.Config;
using Base.Data;
using Base.Data.Accounts;
using Base.Data.Tournaments;
using Tools;
using Gesture = Base.Config.ChainTypes.RockPaperScissorsGesture;


public enum GameResult {
	
	Win,
	Lose,
	Draw
}


public sealed class GameManager : SingletonMonoBehaviour<GameManager> {

	public event Action<MatchContainer> OnNewMatch = match => { };
	public event Action<MatchContainer> OnMatchComplete = match => { };

	public event Action<MatchContainer, GameContainer> OnNewGame = ( match, game ) => { };
	public event Action<MatchContainer, GameContainer> OnGameComplete = ( match, game ) => { };
	public event Action<MatchContainer, GameContainer> OnGameExpectedMove = ( match, game ) => { };


	AccountObject me;
	TournamentContainer currentTournament;
	Dictionary<SpaceTypeId, TournamentContainer> myTournaments = new Dictionary<SpaceTypeId, TournamentContainer>();


	public TournamentContainer CurrentTournament {
		get { return currentTournament; }
	}

	public MatchContainer CurrentMatch {
		get { return CurrentTournament.IsNull() ? null : CurrentTournament.CurrentMatch; }
	}

	public GameContainer CurrentGame {
		get { return CurrentMatch.IsNull() ? null : CurrentMatch.CurrentGame; }
	}

	public void SetCurrentTournament( SpaceTypeId tournament ) {
		if ( IsTournamentExist( tournament ) ) {
			currentTournament = myTournaments[ tournament ];
		}
	}

	public TournamentContainer[] AllTournaments {
		get { return new List<TournamentContainer>( myTournaments.Values ).ToArray(); }
	}

	public bool IsTournamentExist( SpaceTypeId tournament ) {
		return myTournaments.ContainsKey( tournament );
	}

	public int GetCompletedMatches( SpaceTypeId id ) {
		return myTournaments[ id ].CompletedMatches.Count;
	}

	public uint GetTournamentNumberOfPlayers( SpaceTypeId id ) {
		return myTournaments[ id ].NumberOfPlayers;
	}

	protected override void Awake() {
		base.Awake();
		AuthorizationManager.OnAuthorizationChanged -= AuthorizationManager_OnAuthorizationChanged;
		AuthorizationManager.OnAuthorizationChanged += AuthorizationManager_OnAuthorizationChanged;
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		AuthorizationManager.OnAuthorizationChanged -= AuthorizationManager_OnAuthorizationChanged;
		Repository.OnObjectUpdate -= Repository_OnObjectUpdate;
	}

	void AuthorizationManager_OnAuthorizationChanged( AuthorizationManager.AuthorizationData authorization ) {
		Repository.OnObjectUpdate -= Repository_OnObjectUpdate;
		if ( authorization.IsNull() ) {
			me = null;
			return;
		}
		me = authorization.UserNameData.FullAccount.Account;
		Repository.OnObjectUpdate += Repository_OnObjectUpdate;
	}

	public void UpdateMetches( TournamentObject info ) {
	    SetCurrentTournament( info.Id );

        ApiManager.Instance.Database.GetTournamentDetails( info.TournamentDetails.Id ).Then( detail => {
			ApiManager.Instance.Database.GetMatches( Array.ConvertAll( detail.Matches, match => match.Id ) ).Then( matches => {
				var myMatch = Array.Find( matches, match => match.State.Equals( ChainTypes.MatchState.InProgress ) && match.Players.Contains( me.Id ) );
				ApiManager.Instance.Database.GetGames( Array.ConvertAll( myMatch.Games, game => game.Id ) ).Then( games => {
					var existTournament = IsTournamentExist( myMatch.Tournament );
					var existMatch = existTournament && myTournaments[ myMatch.Tournament ].StartedMatches.Contains( myMatch.Id );
					if ( !existTournament ) {
						myTournaments[ myMatch.Tournament ] = new TournamentContainer( match => OnNewMatch.Invoke( match ), match => OnMatchComplete.Invoke( match ) );
					}
					myTournaments[ myMatch.Tournament ].UpdateTournamentInfo( info, detail );
					var opponent = Array.Find( myMatch.Players, account => !me.Id.Equals( account ) );
					Repository.GetInPromise( opponent, () => ApiManager.Instance.Database.GetAccount( opponent.Id ) ).Then( account => {
						if ( !existMatch ) {
							myTournaments[ myMatch.Tournament ].NewMatch( myMatch, me, account,
																		 ( match, game ) => OnNewGame.Invoke( match, game ),
																		 ( match, game ) => OnGameComplete.Invoke( match, game ),
																		 ( match, game ) => OnGameExpectedMove.Invoke( match, game ) );
						}
						var completedGames = myTournaments[ myMatch.Tournament ].CurrentMatch.CompletedGames;
						foreach ( var game in games ) {
							if ( !completedGames.ContainsKey( game.Id ) ) {
							    if ( game.State.Equals( ChainTypes.GameState.Complete ) ) {
							        ( completedGames[game.Id] = new GameContainer( completedGames.Count + 1, game, me, account ) )
							            .CheckState();
							    } else {
							        myTournaments[myMatch.Tournament].CurrentMatch.NewGame( game ).CheckState();

							    }
							}
						}
					} );
				} );
			} );
		} );
	}

	void Repository_OnObjectUpdate( IdObject idObject ) {
		if ( !idObject.SpaceType.Equals( SpaceType.Match ) ) {
			return;
		}
		var updatedMatch = idObject as MatchObject;
		if ( !updatedMatch.Players.Contains( me.Id ) ) {
			return;
		}
		if ( updatedMatch.State.Equals( ChainTypes.MatchState.WaitingOnPreviousMatches ) ) {
			return;
		}
		Repository.GetInPromise( updatedMatch.Tournament, () => ApiManager.Instance.Database.GetTournament( updatedMatch.Tournament.Id ) ).Then( tournament => {
			Repository.GetInPromise( tournament.TournamentDetails, () => ApiManager.Instance.Database.GetTournamentDetails( tournament.TournamentDetails.Id ) ).Then( tournamentDetails => {
				var existTournament = IsTournamentExist( updatedMatch.Tournament );
				var existMatch = existTournament && myTournaments[ updatedMatch.Tournament ].StartedMatches.Contains( updatedMatch.Id );
				if ( !existTournament ) {
					myTournaments[ updatedMatch.Tournament ] = new TournamentContainer( match => OnNewMatch.Invoke( match ), match => OnMatchComplete.Invoke( match ) );
				}
				myTournaments[ updatedMatch.Tournament ].UpdateTournamentInfo( tournament, tournamentDetails );
				if ( !existMatch && updatedMatch.State.Equals( ChainTypes.MatchState.InProgress ) ) {
					var opponent = Array.Find( updatedMatch.Players, account => !me.Id.Equals( account ) );
					Repository.GetInPromise( opponent, () => ApiManager.Instance.Database.GetAccount( opponent.Id ) ).Then( account => {
						myTournaments[ updatedMatch.Tournament ].NewMatch( updatedMatch, me, account,
																		  ( match, game ) => OnNewGame.Invoke( match, game ),
																		  ( match, game ) => OnGameComplete.Invoke( match, game ),
																		  ( match, game ) => OnGameExpectedMove.Invoke( match, game ) );
					} );
				}
			} );
		} );
	}

	public void EndGame() {
		Repository.OnObjectUpdate -= Repository_OnObjectUpdate;
	}

	public void RockMove() {
		MakeMove( Gesture.Rock );
	}

	public void PaperMove() {
		MakeMove( Gesture.Paper );
	}

	public void ScissorsMove() {
		MakeMove( Gesture.Scissors );
	}

	void MakeMove( Gesture gesture ) {
		if ( CurrentGame.IsNull() ) {
			return;
		}
		CurrentGame.MakeMove( gesture );
	}
}