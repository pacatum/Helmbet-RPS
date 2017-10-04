using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TournamentResultState {

    Negative,
    Positive

}

public class TournamentResultView : MonoBehaviour {

    [SerializeField] Color winTextColor;
    [SerializeField] Color looseTextColor;
    [SerializeField] Text graphicsTarget;

    Image bgImage;
    TournamentResultState currentResultState;


    void Awake() {
        bgImage = GetComponent<Image>();
    }
    
    public TournamentResultState CurrentResultState {
        get { return currentResultState; }
        set {
            currentResultState = value;
            UpdateResultView( currentResultState );
        }
    }

    void UpdateResultView( TournamentResultState state ) {
        switch ( state ) {
            case TournamentResultState.Negative:
                bgImage.enabled = false;
                graphicsTarget.color = looseTextColor;
                break;
            case TournamentResultState.Positive:
                graphicsTarget.color = winTextColor;
                bgImage.enabled = true;
                break;
        }
    }

    public string ResultText {
        get { return graphicsTarget ? string.Empty : graphicsTarget.text; }
        set {
            if ( graphicsTarget != null ) {
                graphicsTarget.text = value;
            }
        }
    }

}
