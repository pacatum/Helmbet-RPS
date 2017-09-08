using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseHandView : MonoBehaviour {

    public event Action OnApplyClick;

    [SerializeField] Button applyButton;


    void Awake() {
        applyButton.onClick.AddListener( ApplyChoosenHand );
    }

    void ApplyChoosenHand() {
        if ( OnApplyClick != null ) {
            OnApplyClick();
        }
    }
}
