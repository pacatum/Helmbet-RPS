using System;
using UnityEngine;
using UnityEngine.UI;

public class FindTournamentItemView : BaseTournamentItemView {

	public event Action<FindTournamentItemView> OnJoinClick;

	[SerializeField] Button joinButton;

	void Awake() {
		if( joinButton != null ) {
			joinButton.onClick.AddListener( JoinBtn_Click );
		}
	}

	protected virtual void JoinBtn_Click() {
		if( OnJoinClick != null ) {
			OnJoinClick( this );
		}
	}

	//protected override void UpdateActions( ) {
	//	//base.UpdateActions( state );
	//	//if( state.Equals( ChainTypes.TournamentState.AcceptingRegistrations ) && currentTournament.RegisteredPlayers <
	//	//    currentTournament.Options.NumberOfPlayers ) {
	//	//	joinButton.gameObject.SetActive( true );
	//	//} else {
	//	//	joinButton.gameObject.SetActive(false);
	//	//}
	//}

}
