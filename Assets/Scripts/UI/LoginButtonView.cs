using System.Collections;
using System.Collections.Generic;
using TMPro;
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
	TextMeshProUGUI targetText;

	void Awake() {
		targetImage = GetComponent<Image>();
		targetText = GetComponentInChildren<TextMeshProUGUI>();
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
