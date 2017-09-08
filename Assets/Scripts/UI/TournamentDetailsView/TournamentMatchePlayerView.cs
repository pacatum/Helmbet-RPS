using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState {

    Winner,
    Undefined,
    Looser

}

public class TournamentMatchePlayerView : MonoBehaviour {


    [SerializeField] Sprite winnerSprite;
    [SerializeField] Sprite looserSprite;

	PlayerState currentPlayerState;
    Text usernameText;
    Image playerViewImage;

    [SerializeField] Color winnerUsernameColor;
    [SerializeField] Color looserUsernameColor;
    [SerializeField] Color notDefinedUsernameColor;


    void Awake() {
        usernameText = GetComponentInChildren<Text>();
        playerViewImage = GetComponent<Image>();
        usernameText.text = "";
    }

    public PlayerState CurrentPlayerState {
        get { return currentPlayerState; }
        set {
            currentPlayerState = value; 
            UpdateState( currentPlayerState );
        }
    }

    void UpdateState( PlayerState state ) {
        switch ( state ) {
            case PlayerState.Winner:
                playerViewImage.sprite = winnerSprite;
                usernameText.color = winnerUsernameColor;
                break;
            case PlayerState.Looser:
                playerViewImage.sprite = looserSprite;
                usernameText.color = looserUsernameColor;
                break;
            case PlayerState.Undefined:
                playerViewImage.sprite = looserSprite;
                usernameText.color = notDefinedUsernameColor;
                usernameText.text = "to be defined";
                break;
        }
    }


    public void SetUsername( string username ) {
        usernameText.text = username.Length > 10 ? username.Substring(0, 10) + "..." : username; 
    }
}
