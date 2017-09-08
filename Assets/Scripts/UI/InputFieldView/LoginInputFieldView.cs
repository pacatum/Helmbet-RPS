using System;
using System.Text.RegularExpressions;

public class LoginInputFieldView : InputFieldView {

	Regex expectedLetters = new Regex( "^[a-z0-9-]$" );
    Regex expectedString = new Regex("^[a-z][a-z0-9_-]{2,63}$");


    protected override  void Start() {
		input.onValueChanged.AddListener( delegate { ValueChangeCheck(); } );
	}

    protected override void ValueChangeCheck() {
        inputText = input.text;

        for ( int i = 0; i < inputText.Length; i++ ) {
            if ( !expectedLetters.IsMatch( inputText[i].ToString() ) ) {
                input.text = inputText = inputText.Remove( i, 1 );
            }
        }

        if ( expectedString.IsMatch( inputText )
             && Char.IsLetterOrDigit( inputText[inputText.Length - 1] ) && ContainDigitOrSeparator( inputText ) ) {
            ActiveHint( false );
            IsValidate = true;
        } else {
            ActiveHint( true );
            IsValidate = false;
        }
        InputText_OnChange();
    }

    bool ContainDigitOrSeparator( string inputText ) {
		for ( int i = 0; i < inputText.Length; i++ ) {
			if ( expectedString.IsMatch( inputText ) &&
			     ( Char.IsDigit( inputText[i] ) || Char.IsPunctuation( inputText[i] ) ) ) {
				return true;
			}
		}
		return false;
	}

}
