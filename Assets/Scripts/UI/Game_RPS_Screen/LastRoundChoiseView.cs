using Base.Config;
using UnityEngine;
using UnityEngine.UI;
using Gesture = Base.Config.ChainTypes.RockPaperScissorsGesture;

public class LastRoundChoiseView : MonoBehaviour {

    [SerializeField] Text opponentNickNameText;



    public void SetLastStep( string opponentNickname ) {
        opponentNickNameText.text = "ROUND AGAINST: " + opponentNickname;

    }

}
