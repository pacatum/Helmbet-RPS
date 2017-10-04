using UnityEngine;
using UnityEngine.UI;

public class ResultCounterController : MonoBehaviour {

    public Text LoseText;
    public Text TieText;
    public Text WinText;

    int tieAmount;
    int loseAmount;
    int winAmount;


    private void Awake() {
        ClearCounter();
    }

    public int LoseAmount {
        get { return loseAmount; }
        private set {
            loseAmount = value;
            LoseText.text = loseAmount.ToString();
        }
    }

    public int TieAmount {
        get { return tieAmount; }
        set {
            tieAmount = value;
            TieText.text = tieAmount.ToString();
        }
    }

    public int WinAmount {
        get { return winAmount; }
        private set {
            winAmount = value;
            WinText.text = winAmount.ToString();
        }
    }

    public void ClearCounter() {
        WinAmount = 0;
        LoseAmount = 0;
        TieAmount = 0;
    }

    public void CountWin() {
        WinAmount += 1;
    }

    public void CountTie() {
        TieAmount += 1;
    }

    public void CountLose() {
        LoseAmount += 1;
    }

}