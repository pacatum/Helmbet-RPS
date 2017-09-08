using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsFieldsView : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler {

	public enum SettingsFieldState {

		Inactive,
		Hover,
		Pressed

	}

    public enum ValidationType {

        Decimal,
        Hour,
        Minute,
        Second

    }


	public event Action<SettingView.SettingState> OnSettingStateChange;
	public event Action<SettingsFieldsView> OnInputfieldStateChange;
    public event Action OnSettingFieldValidate;

    [SerializeField] ValidationType currentValudationType;

	[SerializeField] protected Image pressedEffectImage;
	[SerializeField] protected Sprite hoverSprite;
	[SerializeField] protected Sprite pressedSprite;
	[SerializeField] protected GameObject line;
	[SerializeField] protected Color inactiveColor;
	[SerializeField] protected Color activeColor;

	[SerializeField] protected Image bgImage;

    protected bool isFilledIn;
    protected SettingsFieldState currentState;
    protected InputField input;
    protected string inputText;

    public bool IsFilledIn {
        get { return isFilledIn; }
    }

    protected virtual void Awake() {
	    input = GetComponent<InputField>();
        input.onValueChanged.AddListener(delegate { InputValue_OnChange(); });
	    input.onEndEdit.AddListener(delegate { InputValue_OnChange(); });
        CheckFilledIn( input.text );
        SetInactiveState();
	}

    protected virtual void InputValue_OnChange() {
       
        inputText = input.text;
        if ( input.text == "" ) {
            input.text = 0.ToString();
        }
        if ( Char.IsPunctuation( inputText[0] ) ) {
            input.text = inputText.Remove(0, 1);
        }

        if (inputText.Length>1 && !Char.IsPunctuation(inputText[1]) && Int32.Parse( inputText[0].ToString() ) == 0 ) {
           input.text =  inputText.Remove( 0, 1 );
        }
        if ( currentValudationType == ValidationType.Decimal && Char.IsPunctuation( inputText[0] ) ) {
            input.text = 0 + inputText;
        }
        inputText = input.text;
        UpdateValidation( inputText );
    }

    void CheckFilledIn( string text ) {
        isFilledIn = false;
        int integerNum = 0;
        switch ( currentValudationType ) {
            case ValidationType.Decimal:
                double num = Double.Parse( text );
                if ( num > 0.0 ) {
                    isFilledIn = true;
                }
                break;
            case ValidationType.Hour:
                Int32.TryParse( text, out integerNum );
                if ( integerNum >= 0 && integerNum <= 24 ) {
                    isFilledIn = true;
                } else {
                    inputText = 0.ToString();
                    input.text = 0.ToString();
                    isFilledIn = true;
                }
                break;
            case ValidationType.Minute:
                Int32.TryParse( text, out integerNum );
                if ( integerNum >= 0 && integerNum <= 59 ) {
                    isFilledIn = true;

                } else {
                    inputText = 0.ToString();
                    input.text = 0.ToString();
                    isFilledIn = true;
                }
                break;
            case ValidationType.Second:
                if ( Int32.TryParse( text, out integerNum ) && integerNum >= 0 && integerNum <= 59 ) {
                    isFilledIn = true;
                } else {
                    inputText = 0.ToString();
                    input.text = 0.ToString();
                    isFilledIn = true;
                }
                break;
        }

    }

    void UpdateValidation( string text ) {
        CheckFilledIn( text );
        FiledState_OnChange();
    }

    protected virtual void FiledState_OnChange() {
        if ( OnSettingFieldValidate != null ) {
            OnSettingFieldValidate();
        }
    }

    public uint CurrentValue {
        get {
            uint value = 0;
            UInt32.TryParse( inputText, out value );
            return value;
        }
    }

    public double CurrentDoubleValue {
        get {
            double value = 0.0;
            Double.TryParse( inputText, out value );
            return value;
        }
        set { input.text = value.ToString(); }
    }

    public int CurrentIntValue {
        get { return Convert.ToInt32(inputText); }
        set { input.text = value.ToString(); }
    }

    public SettingsFieldState CurrentState {
		get { return currentState; }
		set {
			if ( currentState != value ) {
				currentState = value;
				UpdateState( currentState );
			}
		}
	}

    protected void UpdateState( SettingsFieldState state ) {
		switch ( state ) {
			case SettingsFieldState.Hover:
				SetHoverState();
				break;
			case SettingsFieldState.Pressed:
				SetPressedState();
				break;
			case SettingsFieldState.Inactive:
				SetInactiveState();
				break;

		}
	}

    protected void SetInactiveState() {
		pressedEffectImage.color = inactiveColor;
		bgImage.enabled = false;
		line.SetActive( true );
	}

    protected void SetHoverState() {
		pressedEffectImage.color = inactiveColor;
		bgImage.enabled = true;
		bgImage.sprite = hoverSprite;
		line.SetActive(false);
	}

    protected void SetPressedState() {
		pressedEffectImage.color = activeColor;
		bgImage.enabled = true;
		bgImage.sprite = pressedSprite;
		line.SetActive(false);
	}

    public void OnSelect( BaseEventData eventData ) {
        if ( OnSettingStateChange != null ) {
            OnSettingStateChange( SettingView.SettingState.Pressed );
        }
        if ( OnInputfieldStateChange != null ) {
            OnInputfieldStateChange( this );
        }
    }

    public void OnPointerEnter( PointerEventData eventData ) {
		//if ( currentState != SettingsFieldState.Pressed ) {
		//	CurrentState = SettingsFieldState.Hover;
		//}
	}

	public void OnPointerExit( PointerEventData eventData ) {
		//if ( currentState != SettingsFieldState.Pressed ) {
		//	CurrentState = SettingsFieldState.Inactive;
		//}
	}


    public void ClearInput() {
        input.text = "0";
    }
}
