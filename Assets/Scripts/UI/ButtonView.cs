using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonState{

    Active,
    Pressed,
    Inactive
}

public class ButtonView : MonoBehaviour, IPointerClickHandler {

    public event Action<ButtonView> OnButtonClick;

    [SerializeField] protected Color activeColor;
    [SerializeField] protected Color pressedColor;
    [SerializeField] protected Text targetGraphic;
    [SerializeField] protected TextMeshProUGUI targetTextMeshProUgui;

    [SerializeField] protected GameObject selectLine;

    ButtonState currentstate;

    private Color TargetGraphicColor {
        get { return targetGraphic == null ? targetTextMeshProUgui.color : targetGraphic.color; }
        set {
            if ( targetGraphic != null ) {
                targetGraphic.color = value;
            } else {
                targetTextMeshProUgui.color = value;
            }
        }
    }

    public void UpdateState( ButtonState state ) {
        switch ( state ) {
            case ButtonState.Active:
                SetNormalState();
                break;
            case ButtonState.Pressed:
                SetPressedState();
                break;
        }
    }
    

    protected virtual void SetPressedState() {
        TargetGraphicColor = pressedColor;
        selectLine.gameObject.SetActive(true);
    }

    protected virtual void SetNormalState() {
        TargetGraphicColor = activeColor;
        selectLine.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if ( OnButtonClick != null ) {
            OnButtonClick( this );
        }
    }

    public ButtonState Currentstate {
        get { return currentstate; }
        set {
            currentstate = value;
            UpdateState( currentstate );
        }
    }

}


