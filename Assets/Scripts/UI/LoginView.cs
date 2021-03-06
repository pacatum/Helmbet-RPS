﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginView : BaseCanvasView {

	public event Action OnLoginClick;
    public event Action<string> OnLoginDone;

	[SerializeField] GameObject loginMessage;
	[SerializeField] GameObject errorMessage;
	[SerializeField] InputFieldView loginInputField;
	[SerializeField] InputFieldView passwordInputField;
	[SerializeField] LoginButtonView loginBtn;
    [SerializeField] Button registrationButton;
    [SerializeField] Button linkButton;
    [SerializeField] Button settingsButton;
    [SerializeField] ScreenLoader loader;
    [SerializeField] string connectionURL;

	List<InputFieldView> inputFields = new List<InputFieldView>();
	string username;
	string password;
    GameObject selectedGameObject;


    public override void Awake() {
		base.Awake();
		loginBtn.GetComponent<Button>().onClick.AddListener( Login_OnButtonClick );
		inputFields.Add(loginInputField);
		inputFields.Add(passwordInputField);
		loginInputField.OnInputClick += SwitchInputFieldState;
		passwordInputField.OnInputClick += SwitchInputFieldState;
		loginInputField.OnValueChange += CheckInputs;
		passwordInputField.OnValueChange += CheckInputs;
        registrationButton.onClick.AddListener( OpenRegistrationPageInBrowser );
        linkButton.onClick.AddListener( OpenRegistrationPageInBrowser );
        settingsButton.onClick.AddListener( ShowSettings );
        CheckInputs();
    }

    void ShowSettings() {
        UIManager.Instance.CurrentState = UIManager.ScreenState.Settings;
    }

    void OpenRegistrationPageInBrowser() {
        Application.OpenURL(connectionURL);
    }

    void Start() {
        selectedGameObject = loginInputField.gameObject;
        EventSystem.current.SetSelectedGameObject(selectedGameObject, null);
        loginInputField.CurrentState = InputFieldView.InputState.Pressed;
    }

    void Update() {
        Tabulation();
        if ( Input.GetKeyUp( KeyCode.Return ) ) {
            Login_OnButtonClick();
        }
    }

    void Tabulation() {
        if ( Input.GetKeyUp( KeyCode.Tab ) ) {
            var currentSelectedObject = EventSystem.current.currentSelectedGameObject;
            selectedGameObject = currentSelectedObject.Equals( loginInputField.gameObject ) ? passwordInputField.gameObject : loginInputField.gameObject;
            EventSystem.current.SetSelectedGameObject( selectedGameObject, null );
        }
    }

    void Initialization() {
        loader.IsLoading = true;
        username = loginInputField.InputText;
        password = passwordInputField.InputText;

        AuthorizationManager.Instance.AuthorizationBy( username, password )
            .Then( result => {
                loader.IsLoading = false;
                if ( result ) {
                    Login_OnDone();
                } else {
                    ShowErrorMessage();
                }
            } )
            .Catch( exception => {
                ShowErrorMessage();
                loader.IsLoading = false;
            } );
    }

    void Login_OnDone() {
        HideErrorMessage();
        if ( OnLoginDone != null ) {
            OnLoginDone(AuthorizationManager.Instance.Authorization.UserNameData.UserName);
        }
        if ( OnLoginClick != null ) {
            OnLoginClick();
        }
    }


    void SwitchInputFieldState( InputFieldView target ) {
        GlobalManager.Instance.HideCopypastePanel();
        foreach ( var input in inputFields ) {
            input.CurrentState = input.Equals( target ) ? InputFieldView.InputState.Pressed : InputFieldView.InputState.Normal;
        }
    }

    void Login_OnButtonClick() {
		if ( InputsIsValidates() ) {
			Initialization();
		}
	}

	void ShowErrorMessage() {
        loginInputField.CurrentState = InputFieldView.InputState.Error;
        loginMessage.SetActive( false );
		errorMessage.SetActive( true );
	}

	void HideErrorMessage() {
	    loginMessage.SetActive(true);
        errorMessage.SetActive( false );
	}

    void CheckInputs() {
        loginBtn.UpdateState( InputsIsValidates() ? LoginButtonView.ButtonState.Active : LoginButtonView.ButtonState.Inactive );
    }

    bool InputsIsValidates() {
		foreach( var input in inputFields ) {
			if( !input.IsValidate ) {
				return false;
			}
		}
		return true;
	}

    public override void Show() {
        loginInputField.Clear();
        passwordInputField.Clear();
        loginInputField.CurrentState = InputFieldView.InputState.Normal;
        passwordInputField.CurrentState = InputFieldView.InputState.Normal;
        base.Show();
    }

}
