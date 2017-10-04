using System;
using UnityEngine;

public class StartTime : SettingView {

    [SerializeField] SettingsFieldsView hourFiledView;
    [SerializeField] SettingsFieldsView minuteFiledView;
    [SerializeField] SettingsFieldsView secondFiledView;

    private CreateNewView createNewView;


    protected override void Awake() {
        base.Awake();
        createNewView = FindObjectOfType<CreateNewView>();
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

    protected override void FieldView_OnStateChange( SettingsFieldsView.SettingsFieldState state ) {
        foreach ( var input in settingsFieldsViews ) {
            input.CurrentState = state;
        }
    }

    public void UpdateTime(DateTime time) {
        hourFiledView.CurrentIntValue = time.Hour;
        minuteFiledView.CurrentIntValue = time.Minute;
        secondFiledView.CurrentIntValue = time.Second;
    }

    void SwitchPressedInputs( SettingsFieldsView target ) {
        foreach ( var input in settingsFieldsViews ) {
            input.CurrentState = input.Equals( target ) ? SettingsFieldsView.SettingsFieldState.Pressed : input.CurrentState = SettingsFieldsView.SettingsFieldState.Hover;
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
            if ( createNewView.StartTime != null ) {
                return  createNewView.StartTime.Value>=createNewView.RegistrationDeadline.AddMinutes( 10 );
            }
            return false;
        }
    }

}
