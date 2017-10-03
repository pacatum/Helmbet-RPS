using System.Collections;
using System.Collections.Generic;
using Base.Config;
using Base.Data.Assets;
using Base.Data.Tournaments;
using TMPro;
using Tools;
using UnityEngine;
using GameObject = UnityEngine.GameObject;

public class TournamentDetailsItemView : MonoBehaviour {

    [SerializeField] TextMeshProUGUI tournamentIdText;
    [SerializeField] TextMeshProUGUI tournamentGameText;
    [SerializeField] TextMeshProUGUI tournamentStartTimeText;
    [SerializeField] TextMeshProUGUI tournamentRegisterDeadlineText;
    [SerializeField] TextMeshProUGUI tournamentnumberOfPlayersText;
    [SerializeField] TextMeshProUGUI tournamentBuyinText;
    [SerializeField] TextMeshProUGUI tournamentJackpotText;
    [SerializeField] GameObject liveMessage;


    public IEnumerator UpdateItemView( TournamentObject tournament ) {
        var asset = new AssetObject();
        asset = null;
        TournamentManager.Instance.GetAssetObject( tournament.Options.BuyIn.Asset.Id )
            .Then( assetResult => asset = assetResult );
        while ( asset.IsNull() ) {
            yield return null;
        }

        tournamentIdText.text = "#RPS" + tournament.Id;
        tournamentGameText.text = "RPS";
        tournamentnumberOfPlayersText.text = tournament.RegisteredPlayers.ToString();
        tournamentStartTimeText.text = tournament.StartTime.Value.ToString( "dd MMM, yyyy. hh:mm" );
        tournamentRegisterDeadlineText.text = tournament.Options.RegistrationDeadline.ToString("dd MMM, yyyy. hh:mm");
        tournamentBuyinText.text = tournament.Options.BuyIn.Amount/Mathf.Pow( 10, asset.Precision ) + asset.Symbol;
        tournamentJackpotText.text = tournament.PrizePool/Mathf.Pow(10, asset.Precision) + asset.Symbol;

        liveMessage.SetActive( tournament.State.Equals( ChainTypes.TournamentState.InProgress ) );
    }

}
