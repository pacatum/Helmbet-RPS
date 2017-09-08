using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PasswordFieldView : InputFieldView {

	Regex expectedString = new Regex("^.{22,}$");
	//public event Action OnValueChange;

	protected  override void Start() {
		input.onValueChanged.AddListener( delegate { ValueChangeCheck(); } );
		input.asteriskChar = "•"[0];
	}

	protected override void ValueChangeCheck() {
		inputText = input.text;
		if ( expectedString.IsMatch( input.text ) ) {
			IsValidate = true;
            ActiveHint( false );
		} else {
			IsValidate = false;
		    ActiveHint(true);
        }
		InputText_OnChange();
	}
}
