using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginButtonView : MonoBehaviour {

	public enum ButtonState {

		Active,
		Inactive

	}

	[SerializeField] Sprite activeSprite;
	[SerializeField] Sprite inactiveSprite;
	[SerializeField] Color activeTextColor;
	[SerializeField] Color inactiveTextColor;

	Image targetImage;
	Text targetText;

	void Awake() {
		targetImage = GetComponent<Image>();
		targetText = GetComponentInChildren<Text>();
		UpdateState(ButtonState.Inactive);
	}

	public void UpdateState( ButtonState state ) {
		switch ( state ) {
			case ButtonState.Active:
				targetImage.sprite = activeSprite;
				targetText.color = activeTextColor;
				break;
			case ButtonState.Inactive:
				targetImage.sprite = inactiveSprite;
				targetText.color = inactiveTextColor;
				break;
		}
	}

}
