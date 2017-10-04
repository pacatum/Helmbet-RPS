using System;
using System.Collections;
using Base.Data.Tournaments;

public class SceduleTournamentItemView : BaseTournamentItemView {

    public event Action<SceduleTournamentItemView> OnPlayClick;
    public event Action<SceduleTournamentItemView> OnCancelClick;
    public event Action<SceduleTournamentItemView> OnJoinClick;

    protected void Awake() {
        if ( footerView.JoinButton != null ) {
            footerView.JoinButton.onClick.AddListener( JoinBtn_Click );
        }
        if ( footerView.CancelButton != null ) {
            footerView.CancelButton.onClick.AddListener( CancelBtn_Click );
        }
        if ( footerView.PlayButton != null ) {
            footerView.PlayButton.onClick.AddListener( PlayBtn_Click );
        }
    }

    protected virtual void PlayBtn_Click() {
    }

    protected virtual void CancelBtn_Click() {
        if ( OnCancelClick != null ) {
            OnCancelClick( this );
        }
    }

    protected virtual void JoinBtn_Click() {
        if ( OnJoinClick != null ) {
            OnJoinClick( this );
        }
    }

    public override IEnumerator UpdateItem( TournamentObject info ) {
        yield return StartCoroutine( base.UpdateItem( info ) );
        yield return footerView.SetUp( currentTournament, tournamentDetailsObject );
    }

    public override void UpdateTournament( TournamentObject tournament ) {
        if ( !gameObject.activeInHierarchy || !gameObject.activeSelf ) {
            return;
        }
        StartCoroutine( UpdateItem( tournament ) );
    }

}
