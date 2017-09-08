using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwitcherView : MonoBehaviour, IPointerClickHandler
{

	public enum SwitcherState {

		Enabled,
		Disabled

	}

	[SerializeField] Slider soundSwitcher;

	[SerializeField] Color enabledSwitcherColor;
	[SerializeField] Color disabledSwitcherColor;
	[SerializeField] Color enabledTextColor;
	[SerializeField] Color disabledTextColor;

	[SerializeField] Image targetSwitcherGraphic;
	[SerializeField] Text targetOnTextGraphic;
	[SerializeField] Text targetOffTextGraphics;

	SwitcherState currentState;


	void Awake() {
		Currentstate = SwitcherState.Enabled;
	}

	void UpdateState(SwitcherState state)
	{
		switch (state)
		{
			case SwitcherState.Enabled:
				soundSwitcher.value = 1;
				targetSwitcherGraphic.color = enabledSwitcherColor;
				targetOnTextGraphic.color = enabledTextColor;
				targetOffTextGraphics.color = disabledTextColor;
				break;
			case SwitcherState.Disabled:
				soundSwitcher.value = 0;
				targetSwitcherGraphic.color = disabledSwitcherColor;
				targetOnTextGraphic.color = disabledTextColor;
				targetOffTextGraphics.color = enabledTextColor;
				break;
		}
	}

	public void SwitchState() {
		switch ( currentState ) {
			case SwitcherState.Enabled:
				currentState = SwitcherState.Disabled;
				soundSwitcher.value = 0;
				targetSwitcherGraphic.color = disabledSwitcherColor;
				targetOnTextGraphic.color = disabledTextColor;
				targetOffTextGraphics.color = enabledTextColor;
				return;
			case SwitcherState.Disabled:
				currentState = SwitcherState.Enabled;
				soundSwitcher.value = 1;
				targetSwitcherGraphic.color = enabledSwitcherColor;
				targetOnTextGraphic.color = enabledTextColor;
				targetOffTextGraphics.color = disabledTextColor;
				return;
		}
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
		SwitchState();
	}

	public SwitcherState Currentstate
	{
		get { return currentState; }
		set
		{
			currentState = value;
			UpdateState(currentState);
		}
	}
}
