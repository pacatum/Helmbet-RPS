using UnityEngine;
using UnityEngine.UI;

public class LastRoundChoiseView : MonoBehaviour {

    [SerializeField] Text opponentNickNameText;


    public void SetLastStep( string opponentNickname ) {
        opponentNickNameText.text = "ROUND AGAINST: " + opponentNickname;
    }

}
