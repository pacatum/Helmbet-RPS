using UnityEngine;
using UnityEngine.UI;


public class ChooseHandButton : MonoBehaviour {

    public Sprite HandActive;
    public Sprite HandInactive;
    public Image FrameImage;
    public Image HandImage;

    ChooseHandController.HandColour handColour;
    

    void Awake() {
        ChooseHandController.Instance.SetCurrentChoosedHand((ChooseHandController.HandColour)PlayerPrefs.GetInt("PlayerChoosedHand"));
    }

    public void SetUpHandButton( Sprite handScissorsSprite, bool isHandActive, ChooseHandController.HandColour colourOfHand ) {
        HandImage.sprite = handScissorsSprite;
        handColour = colourOfHand;
        SetUpFrameState( isHandActive );
    }

    public void ChooseNewHand() {
        ChooseHandController.Instance.SetCurrentChoosedHand( handColour );
        ChooseHandController.Instance.UpdateGamePreview(handColour);
        SetUpFrameState( true );

        var otherHands = FindObjectsOfType<ChooseHandButton>();
        foreach ( var hand in otherHands ) {
            if ( hand != this ) {
                hand.SetUpFrameState( false );
            }
        }
    }

    public void SetUpFrameState( bool isHandActive ) {
        FrameImage.sprite = isHandActive ? HandActive : HandInactive;
    }
}