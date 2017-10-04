using System;
using UnityEngine;

public class RegistrationDeadlineCalendarView : SettingView {

    public event Action OnChangedValue;

    [SerializeField] CalendarButtonView calendar;
    DateTime selectDate;


    public CalendarButtonView CalendarButtonView {
        get { return calendar; }
    }
    
    protected override void Awake() {
        base.Awake();
        calendar.OnSettingStateChange += State_OnChanged;
        calendar.OnDateChanged += SetRegistrationTime;
        calendar.Calendar.ValidateDateTime = DateTime.UtcNow;
    }

    protected override void FieldView_OnStateChange( SettingsFieldsView.SettingsFieldState state ) {
    }

    void Calendar_OnStateChange( CalendarState state ) {
        calendar.CurrentState = state;
    }

    protected override void SetNormalView() {
        base.SetNormalView();
        Calendar_OnStateChange(CalendarState.Inactive );
    }

    protected override void SetPressedView() {
        base.SetPressedView();
        Calendar_OnStateChange(CalendarState.Hover );
    }

    public DateTime CurrentValue {
        get { return selectDate; }
    }

    public override bool IsFilledIn {
        get { return true; }
    }

    public void SetRegistrationTime( DateTime date ) {
        selectDate = date;
        if ( OnChangedValue != null ) {
            OnChangedValue();
        }
        Validate_OnChange();
    }

}
