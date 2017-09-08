using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class NumbersInputFieldView : InputFieldView {

	Regex expectedString = new Regex("^[-+]?[0-9]*[.,]?[0-9]+(?:[eE][-+]?[0-9]+)?$");

	protected override void Awake() {
		base.Awake();
		input.onValueChanged.AddListener( delegate { ValueChangeCheck(); } );
		input.onEndEdit.AddListener( delegate { ValueEndEditingCheck(); } );
	}

	protected override void ValueChangeCheck() {
		inputText = input.text;
		if( expectedString.IsMatch( inputText ) ) {
			UpdateInput( InputState.Normal );
			IsValidate = true;
		} else {
			UpdateInput(InputState.Hover);
			IsValidate = false;
		}
	}

	void ValueEndEditingCheck() {
		inputText = input.text;
		if( Char.IsPunctuation( inputText[0] ) ) {
			input.text = 0 + inputText;
		}
	}

}
