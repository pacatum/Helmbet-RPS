using System;
using UnityEngine;

public class RegistrationTime : SettingView {

    [SerializeField] SettingsFieldsView hourFiledView;
    [SerializeField] SettingsFieldsView minuteFiledView;
    [SerializeField] SettingsFieldsView secondFiledView;
    [SerializeField] CreateNewView createNewView;


    protected override void Awake() {
        base.Awake();
        settingsFieldsViews.Add( hourFiledView );
        settingsFieldsViews.Add( minuteFiledView );
        settingsFieldsViews.Add( secondFiledView );

        hourFiledView.OnInputfieldStateChange += SwitchPressedInputs;
        minuteFiledView.OnInputfieldStateChange += SwitchPressedInputs;
        secondFiledView.OnInputfieldStateChange += SwitchPressedInputs;

        hourFiledView.OnSettingStateChange += State_OnChanged;
        minuteFiledView.OnSettingStateChange += State_OnChanged;
        secondFiledView.OnSettingStateChange += State_OnChanged;

        hourFiledView.OnSettingFieldValidate += SetErrorIcon;
        minuteFiledView.OnSettingFieldValidate += SetErrorIcon;
        secondFiledView.OnSettingFieldValidate += SetErrorIcon;
    }

    void Start() {
        Clear();
    }

    public override void Clear() {
        hourFiledView.CurrentIntValue = DateTime.Now.Hour + 1;
        minuteFiledView.CurrentIntValue = DateTime.Now.Minute;
        secondFiledView.CurrentIntValue = DateTime.Now.Second;
    }

    protected override void FieldView_OnStateChange( SettingsFieldsView.SettingsFieldState state ) {
        foreach ( var input in settingsFieldsViews ) {
            input.CurrentState = state;
        }
    }

    void SwitchPressedInputs( SettingsFieldsView target ) {
        foreach ( var input in settingsFieldsViews ) {
            if ( input.Equals( target ) ) {
                input.CurrentState = SettingsFieldsView.SettingsFieldState.Pressed;
            } else {
                input.CurrentState = SettingsFieldsView.SettingsFieldState.Hover;
            }
        }
    }

    public int Hour {
        get { return hourFiledView.CurrentIntValue; }
    }

    public int Minute {
        get { return minuteFiledView.CurrentIntValue; }
    }

    public int Second {
        get { return secondFiledView.CurrentIntValue; }
    }

    public override bool IsFilledIn {
        get {
            return ( new DateTime( createNewView.RegistrationDeadline.Year, createNewView.RegistrationDeadline.Month, createNewView.RegistrationDeadline.Day ) -
                     new DateTime( DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day ) ).TotalDays >= 0;
        }
    }

}
