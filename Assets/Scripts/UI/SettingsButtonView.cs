using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonView : ButtonView {

    [SerializeField] Image buttonIcon;
    [SerializeField] Color iconActiveColor;


    protected override void SetNormalState() {
        base.SetNormalState();
        buttonIcon.color = iconActiveColor;
    }

    protected override void SetPressedState() {
        base.SetPressedState();
        buttonIcon.color = pressedColor;

    }

}
