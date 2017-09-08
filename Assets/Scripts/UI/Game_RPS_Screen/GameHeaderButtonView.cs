using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum GameHeaderButtonState {

    Normal,
    Hover,
    HeaderHover,
    Pressed

}

public class GameHeaderButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] Color normalColor;
    [SerializeField] Color hoverColor;
    [SerializeField] Color hoverHeaderColor;

    GameHeaderButtonState currentState;
    Image image;

    private Image CurrentImage {
        get { return image.IsNull() ? image = GetComponent<Image>() : image; }
    }

    void Awake() {
        image = GetComponent<Image>();
    }

    public GameHeaderButtonState CurrentState {
        get { return currentState; }
        set {
            currentState = value;
            UpdateState( currentState );
        }
    }

    void UpdateState( GameHeaderButtonState state ) {
        switch ( state ) {
            case GameHeaderButtonState.HeaderHover:
                CurrentImage.color = hoverHeaderColor;
                break;
            case GameHeaderButtonState.Hover:
            case GameHeaderButtonState.Pressed:
                CurrentImage.color = hoverColor;
                break;
            case GameHeaderButtonState.Normal:
                CurrentImage.color = normalColor;
                break;
        }
    }

    public void OnPointerEnter( PointerEventData eventData ) {
        if ( CurrentState != GameHeaderButtonState.Pressed ) {
            CurrentState = GameHeaderButtonState.Hover;
        }
    }

    public void OnPointerExit( PointerEventData eventData ) {
        if ( CurrentState != GameHeaderButtonState.Pressed ) {
            CurrentState = GameHeaderButtonState.HeaderHover;
        }
    }

}
