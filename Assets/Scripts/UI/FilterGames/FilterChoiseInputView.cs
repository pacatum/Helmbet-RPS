﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class FilterChoiseInputView : MonoBehaviour {

    public event Action OnValueChange;

    public string InputText;
    InputField input;
    Image image;


    void Awake() {
        input = GetComponent<InputField>();
        image = GetComponent<Image>();
        InputText = input.text;
        input.onValueChanged.AddListener( delegate {
            OnValueEndEdit();
        } );
    }

    void OnValueEndEdit() {
        if ( input.text == "" ) {
            input.text = "0";
        }
        if ( Char.IsPunctuation( input.text[0] ) ) {
            var tempText = "0" + input.text;
            input.text = tempText;
        }

        if ( input.text.Length > 1 && input.text[0].ToString() == "0" && !Char.IsPunctuation( input.text[1] ) ) {
            var tempText = input.text.Remove( 0, 1 );
            input.text = tempText;
        }

        if ( !InputText.Equals( input.text )) {
            InputText = input.text;
            if ( OnValueChange != null ) {
                OnValueChange();
            }
        }
    }

    public void ChangeValue( string value ) {
        if ( input == null ) {
            return;
        }
        input.text = InputText = value;

    }

    public void SetError() {
        if ( image == null ) {
            return;
        }
        image.color = Color.red;
    }

    public void SetNormal() {
        if ( image == null ) {
            return;
        }
        image.color = Color.white;
    }
}
