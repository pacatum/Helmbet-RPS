using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetItemToDefaultByEscape : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public event Action OnEscapeClick;

    bool isHover;

    void Update() {
        if ( Input.GetKeyUp( KeyCode.Escape ) || ( Input.GetMouseButtonUp( 0 ) && !isHover ) ) {
            Escape_OnClick();
        }
    }

    void Escape_OnClick() {
        if ( OnEscapeClick != null ) {
            OnEscapeClick();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHover = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHover = false;
    }
}
