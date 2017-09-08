using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum FilterItemState {

    Select,
    Normal

}

public class FilterChoiseItemView : MonoBehaviour, IPointerClickHandler {

    public event Action<FilterChoiseItemView> OnValueChange;

    [SerializeField] Color selectColor;
    [SerializeField] Color normalColor;
    [SerializeField] Text targetText;
    [SerializeField] GameObject selectChoiseIcon;

    FilterItemState currentState;

    void Awake() {
        if ( OnValueChange != null ) {
            OnValueChange( this );
        }
    }

    public FilterItemState CurrentState {
        get { return currentState; }
        set {
            if ( currentState != value ) {
                currentState = value;
                UpdateState( currentState );
            }
        }
    }


    public string SelectChoise {
        get { return targetText == null ? string.Empty : targetText.text; }
    }

    public void OnPointerClick( PointerEventData eventData ) {
        OnChoosen();
    }

    void UpdateState( FilterItemState state ) {
        switch ( state ) {
            case FilterItemState.Normal:
                targetText.color = normalColor;
                selectChoiseIcon.SetActive( false );
                break;
            case FilterItemState.Select:
                targetText.color = selectColor;
                selectChoiseIcon.SetActive( true );
                break;
        }
    }

    public void OnChoosen() {
        if ( OnValueChange != null ) {
            OnValueChange( this );
        }
    }
}
