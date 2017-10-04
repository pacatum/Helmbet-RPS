using System;
using System.Collections;
using System.Collections.Generic;
using Base.Data.Accounts;
using Base.Data.Tournaments;
using TMPro;
using UnityEngine;

public class HistoryTournamentItemView : BaseTournamentItemView {

    [SerializeField] TextMeshProUGUI winnerText;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] TMP_FontAsset anotherPlayerWinnerFont;
    [SerializeField] TMP_FontAsset thisPlayerWinnerFont;


    public string Winner {
        get { return winnerText ? string.Empty : winnerText.text; }
        protected set {
            if ( winnerText != null ) {
                winnerText.text = value;
            }
        }
    }

    public override IEnumerator UpdateItem( TournamentObject info ) {
        var winnerAccounts = new List<AccountObject>();
        yield return StartCoroutine( base.UpdateItem( info ) );

        yield return TournamentManager.Instance.GetMatcheWinnerAccountsObjects( info.Id.Id, winnerAccounts );
        if ( winnerAccounts[0].Name == AuthorizationManager.Instance.UserData.UserName ) {
            winnerText.font = thisPlayerWinnerFont;
            resultText.text = Utils.GetFormatedDecimaNumber( ( ( info.PrizePool - info.Options.BuyIn.Amount )/ Math.Pow( 10, currentAsset.Precision ) ).ToString() ) + currentAsset.Symbol;
        } else {
            winnerText.font = anotherPlayerWinnerFont;
            resultText.text = "-" + buyInText.text;
        }
        Winner = Utils.GetFormatedString( winnerAccounts[0].Name, 7 );
        yield return footerView.SetUp( currentTournament, tournamentDetailsObject );
    }
    
    protected override void UpdateView( bool isHover ) {
        base.UpdateView( isHover );

        winnerTextTitle.SetActive( isHover );
        resultTextTitle.SetActive( isHover );

        if ( isHover ) {
            winnerText.GetComponent<RectTransform>().pivot = resultText.GetComponent<RectTransform>().pivot = new Vector2( 1f, 0.5f );
            winnerText.GetComponent<RectTransform>().anchorMax = resultText.GetComponent<RectTransform>().anchorMax =
                winnerText.GetComponent<RectTransform>().anchorMin = resultText.GetComponent<RectTransform>().anchorMin = new Vector2( 1f, 1f );

            winnerText.GetComponent<RectTransform>().anchoredPosition = new Vector2( -27, winnerText.GetComponent<RectTransform>().anchoredPosition.y );
            resultText.GetComponent<RectTransform>().anchoredPosition = new Vector2( -27, resultText.GetComponent<RectTransform>().anchoredPosition.y );
        } else {
            winnerText.GetComponent<RectTransform>().pivot = resultText.GetComponent<RectTransform>().pivot = new Vector2( 0.5f, 0.5f );
            winnerText.GetComponent<RectTransform>().anchorMax = resultText.GetComponent<RectTransform>().anchorMax =
                winnerText.GetComponent<RectTransform>().anchorMin = resultText.GetComponent<RectTransform>().anchorMin = new Vector2( 0.5f, 1f );

            winnerText.GetComponent<RectTransform>().anchoredPosition = new Vector2( 0, winnerText.GetComponent<RectTransform>().anchoredPosition.y );
            resultText.GetComponent<RectTransform>().anchoredPosition = new Vector2( 0, resultText.GetComponent<RectTransform>().anchoredPosition.y );
        }
    }

}
