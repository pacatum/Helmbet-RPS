using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleView : MonoBehaviour {

    public event Action<bool> OnExactTimeAndDate;

    [SerializeField] bool extactTimeAndDate;
    [SerializeField] Image inactiveToggleImage;
	[SerializeField] Color inactiveColor;
	[SerializeField] Color activeColor;
	[SerializeField] Text targetGraphics;

	Toggle toggle;

	void Awake() {
		toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(value => UpdateToggle( value )  );
	}

	public void UpdateToggle( bool active ) {
        

		if ( active ) {
		    toggle.isOn = true;
			inactiveToggleImage.enabled = false;
			targetGraphics.color = activeColor;
		    Toggle_OnUpdateAction();

		} else {
		    toggle.isOn = false;
            inactiveToggleImage.enabled = true;
			targetGraphics.color = inactiveColor;
		}
	}

    void Toggle_OnUpdateAction() {
        if ( OnExactTimeAndDate != null ) {
            OnExactTimeAndDate( extactTimeAndDate );
        }
    }

    public void SetInteructable( bool value ) {
        toggle.interactable = value;
    }

}
