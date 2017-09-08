using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundOver : MonoBehaviour
{

    public Image PlayerGesture;
    public Image OpponentGesture;
    public Text RoundNumber;

    public void SetUpRoundOver(Sprite playerGestureSprite, Sprite opponentGestureSprite, int roundNumber)
    {
        PlayerGesture.sprite = playerGestureSprite;
        OpponentGesture.sprite = opponentGestureSprite;
        RoundNumber.text = roundNumber.ToString();
    }
}
