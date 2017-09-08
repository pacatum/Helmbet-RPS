using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPasswordView : BaseCanvasView {


	public event Action OnCancelClick;
	public event Action OnApplyClick;

	[SerializeField] GameObject errorMessage;
	[SerializeField] PasswordFieldView currentPasswordInput;
	[SerializeField] PasswordFieldView newPasswordInput;
	[SerializeField] PasswordFieldView newPasswordConformInput;

	[SerializeField] Button cancelBtn;
	[SerializeField] Button applyBtn;

	List<PasswordFieldView> inputFields = new List<PasswordFieldView>();


	public override void Awake() {
		base.Awake();
		//cancelBtn.onClick.AddListener( Settings_OnCancelClick );
		//applyBtn.onClick.AddListener( Settings_OnApplyClick );
		OnApplyClick += ApplySettingsChahges;

		inputFields.Add( currentPasswordInput );
		inputFields.Add( newPasswordConformInput );
		inputFields.Add( newPasswordInput );
	}

	void Input_OnChangeState( InputFieldView target ) {
		foreach( var input in inputFields ) {
			if( input.Equals( target ) ) {
				input.CurrentState = InputFieldView.InputState.Hover;
			} else {
				input.CurrentState = InputFieldView.InputState.Normal;
			}
		}
	}

	void ApplySettingsChahges() {
		return;
	}

	void Settings_OnApplyClick() {
		if( OnApplyClick != null ) {
			OnApplyClick();
		}
	}

	void Settings_OnCancelClick() {
		if( OnCancelClick != null ) {
			OnCancelClick();
		}
	}
}
