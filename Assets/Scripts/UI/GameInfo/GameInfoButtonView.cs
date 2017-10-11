using UnityEngine;
using UnityEngine.UI;

public class GameInfoButtonView : ButtonView {

    [SerializeField] Image targetImageGraphic;
    [SerializeField] Color normalBgColor;
    [SerializeField] Color pressedBgColor;
    [SerializeField] Image iconImage;
    [SerializeField] Color normalIconColor;
    [SerializeField] Color pressedIconColor;


    void Awake() {
        Currentstate = ButtonState.Active;
    }

    protected override void SetNormalState() {
        TargetGraphicColor = activeColor;
        targetImageGraphic.color = normalBgColor;
        iconImage.color = normalIconColor;
    }

    protected override void SetPressedState() {
        TargetGraphicColor = pressedColor;
        targetImageGraphic.color = pressedBgColor;
        iconImage.color = pressedIconColor;
    }

}
