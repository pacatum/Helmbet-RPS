using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberOfPlayers : SettingView {

	[SerializeField] SettingsFieldsView numberOfPlayersSettingsFieldView;
    [SerializeField] Dropdown numberOfPlayersDropdown;

    protected override void Awake() {
        base.Awake();
        settingsFieldsViews.Add( numberOfPlayersSettingsFieldView );
        numberOfPlayersSettingsFieldView.OnSettingStateChange += State_OnChanged;
        numberOfPlayersSettingsFieldView.OnInputfieldStateChange += SetInputPressed;

        foreach ( var input in settingsFieldsViews ) {
            input.OnSettingFieldValidate += SetErrorIcon;
        }
        
        numberOfPlayersDropdown.onValueChanged.AddListener( delegate {Validate_OnChange();});
    }

    void SetInputPressed(SettingsFieldsView target) {
		target.CurrentState = SettingsFieldsView.SettingsFieldState.Pressed;
	}

	protected override void FieldView_OnStateChange( SettingsFieldsView.SettingsFieldState state ) {
		numberOfPlayersSettingsFieldView.CurrentState = state;
	}


    public uint CurrentValue {
        get { return Convert.ToUInt32(numberOfPlayersDropdown.options[numberOfPlayersDropdown.value].text); }
    }

    public override bool IsFilledIn {
        get { return true; }
    }

    public override void Clear() {
        numberOfPlayersDropdown.value = 0;
    }

}
