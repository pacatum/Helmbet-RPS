
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class RegistrationDeadlineCalendarView : SettingView {

    public event Action OnChangedValue;

    [SerializeField] CalendarButtonView calendar;

    public CalendarButtonView CalendarButtonView {
        get { return calendar; }
    }

    DateTime selectDate;

    protected override void Awake() {
        base.Awake();
        calendar.OnSettingStateChange += State_OnChanged;
        calendar.OnDateChanged += SetRegistrationTime;
        calendar.Calendar.ValidateDateTime = DateTime.UtcNow;
    }

    protected override void FieldView_OnStateChange( SettingsFieldsView.SettingsFieldState state ) {
    }

    void Calendar_OnStateChange( CalendarButtonView.CalendarState state ) {
        calendar.CurrentState = state;
    }

    protected override void SetNormalView() {
        base.SetNormalView();
        Calendar_OnStateChange( CalendarButtonView.CalendarState.Inactive );
    }

    protected override void SetPressedView() {
        base.SetPressedView();
        Calendar_OnStateChange( CalendarButtonView.CalendarState.Hover );
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
