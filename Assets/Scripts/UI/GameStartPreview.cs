using System;
using Base.Data.Tournaments;
using UnityEngine;
using UnityEngine.UI;

public class GameStartPreview : BaseCanvasView {
    
    [SerializeField] Text tournamentStartTimeSeconds;
    [SerializeField] Text tournamentStartTimeMinutes;

    [SerializeField] MessagePopupView messagePopupView;

    TournamentObject currentTournament;
    DateTime currentDateTime = DateTime.Now;


    public void SetTournamentInformation( TournamentObject tournament ) {
        gameObject.SetActive( true );
        messagePopupView.HideAll();
        currentTournament = tournament;
        currentDateTime = tournament.StartTime.Value;
    }

    void Update() {
        if ( currentTournament != null && currentTournament.StartTime.HasValue && currentDateTime> DateTime.UtcNow) {
            var timeSpan = currentDateTime - DateTime.UtcNow;
            tournamentStartTimeMinutes.text = String.Format( "{0:00}", timeSpan.Minutes );
            tournamentStartTimeSeconds.text = ":" + String.Format("{0:00}", timeSpan.Seconds);

        }
    }
    
}
