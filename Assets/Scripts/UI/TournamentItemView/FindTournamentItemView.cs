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

}
