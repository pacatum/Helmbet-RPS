using System;
using System.Collections;
using System.Collections.Generic;
using Base.Data.Accounts;
using Base.Data.Tournaments;
using UnityEngine;
using UnityEngine.UI;

public class HistoryTournamentItemView : BaseTournamentItemView {

    [SerializeField] Text winnerText;
    [SerializeField] TournamentResultView resultText;
    [SerializeField] Font anotherPlayerWinnerFont;
    [SerializeField] Font thisPlayerWinnerFont;

    public string Winner {
        get { return winnerText ? string.Empty : winnerText.text; }
        protected set {
            if ( winnerText != null ) {
                winnerText.text = value;
            }
        }
    }

    protected override void UpdateStartTime() {
        if ( currentTournament.StartTime.HasValue ) {
            DateTime time = currentTournament.StartTime.Value.ToLocalTime();
            StartTime = time.ToString( "ddMMM, yyyy. HH:mm tt" );
        }
    }

    public override IEnumerator UpdateItem( TournamentObject info) {

        var winnerAccounts = new List<AccountObject>();
        yield return StartCoroutine( base.UpdateItem( info) );

        yield return TournamentManager.Instance.GetMatcheWinnerAccountsObjects( info.Id.Id, winnerAccounts );

        resultText.ResultText = "-" + buyInText.text;
        resultText.CurrentResultState = TournamentResultState.Negative;
        winnerText.font = anotherPlayerWinnerFont;

        Winner = Utils.GetFormatedString(winnerAccounts[0].Name, 7);

        if ( winnerAccounts[0].Name == AuthorizationManager.Instance.UserData.UserName ) {
            winnerText.font = thisPlayerWinnerFont;
            resultText.ResultText =
                Utils.GetFormatedDecimaNumber( ( ( info.PrizePool - info.Options.BuyIn.Amount ) /
                                                 Math.Pow( 10, currentAsset.Precision ) ).ToString() ) + currentAsset.Symbol;
            resultText.CurrentResultState = TournamentResultState.Positive;
        }

    }


}
