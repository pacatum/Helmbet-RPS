using System.Text.RegularExpressions;

public class PasswordFieldView : InputFieldView {

	Regex expectedString = new Regex("^.{22,}$");


	protected  void Start() {
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
