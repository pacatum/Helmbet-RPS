using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsGeneralView : BaseCanvasView {

	public event Action OnCancelClick;
	public event Action OnApplyClick;
	//public event Action OnSoundClick;

	[SerializeField] SwitcherView soundBtn;

	[SerializeField] Dropdown languageDropdown;
	[SerializeField] Dropdown handOptionDropdown;

	[SerializeField] Button cancelBtn; 
	[SerializeField] Button applyBtn;


	public override  void Awake() {
		base.Awake();
		cancelBtn.onClick.AddListener( Settings_OnCancelClick );
		applyBtn.onClick.AddListener( Settings_OnApplyClick );
		OnApplyClick += ApplySettingsChahges;

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
