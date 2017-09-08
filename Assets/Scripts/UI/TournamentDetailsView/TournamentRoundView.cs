using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class TournamentRoundView : MonoBehaviour {

    [SerializeField] Text roundTitleText;
    [SerializeField] TournamentMatcheView tournamentMatcheViewPrefab;
    [SerializeField] Transform tournamentMatcheContainer;

    private List<TournamentMatcheView> tournamentMatcheViews = new List<TournamentMatcheView>();

    public void SetRoundTitle( int roundNumber ) {

        switch ( roundNumber ) {
            case 6:
                roundTitleText.text = "1/32";
                break;
            case 5:
                roundTitleText.text = "1/16";
                break;
            case 4:
                roundTitleText.text = "1/8";
                break;
            case 3:
                roundTitleText.text = "1/4";
                break;
            case 2:
                roundTitleText.text = "1/2";
                break;
            case 1:
                roundTitleText.text = "FINAL";
                break;
            default:
                roundTitleText.text = "";
                break;

        }

        GetComponent<VerticalLayoutGroup>().enabled = true;
        GetComponentInChildren<VerticalLayoutGroup>().enabled = true;
    }

    public void ClearRoundTitle() {
        roundTitleText.text = "";
        GetComponent<VerticalLayoutGroup>().enabled = false;
        GetComponentInChildren<VerticalLayoutGroup>().enabled = false;
    }


    public TournamentMatcheView[] UpdateMatches( int playersCount, int roundNumberCount ) {

        RemoveList();

        int matchesCount = playersCount / 2;
        for ( int i = 0; i < matchesCount; i++ ) {
            var item = Instantiate( tournamentMatcheViewPrefab );
            item.transform.SetParent( tournamentMatcheContainer, false );
            tournamentMatcheViews.Add( item );
        }
        return tournamentMatcheViews.ToArray();

    }

    void RemoveList() {
        tournamentMatcheViews.Clear();
    }

}
