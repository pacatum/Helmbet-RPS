using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class StartTimePanelView : MonoBehaviour {

	[SerializeField] StartTime startTime;
    [SerializeField] RegistrationDeadlineCalendarView startDeadlineCalendarView;

    [SerializeField] private RegistrationDeadlineCalendarView registration;

     bool isActive;
     CreateNewView view;


    void Awake() {
        view = FindObjectOfType<CreateNewView>();
        registration.OnChangedValue += UpdateCalendar;
    }
 

    public bool IsActive {
        get { return isActive; }
        set {
            if ( isActive != value ) {
                isActive = value;
                UpdateViews( isActive );
            }
        }
    }

    public void UpdateCalendar() {
        startDeadlineCalendarView.CalendarButtonView.Calendar.ValidateDateTime = view.RegistrationDeadline;
        startDeadlineCalendarView.CalendarButtonView.Calendar.DrawCalendar();
        startTime.UpdateTime(view.RegistrationDeadline.AddMinutes( 10 ).ToLocalTime());
    }

    void UpdateViews( bool active ) {
        if ( active ) {
            UpdateCalendar();
            view.AddViewsToList( startTime );
            view.AddViewsToList( startDeadlineCalendarView );
        } else {
            view.RemoveViewFromList(startTime);
            view.RemoveViewFromList(startDeadlineCalendarView);
        }
    }

    public DateTime? StartDateTime {
        get {
            return new DateTime( startDeadlineCalendarView.CurrentValue.Year,
                                startDeadlineCalendarView.CurrentValue.Month,
                                startDeadlineCalendarView.CurrentValue.Day,
                                startTime.Hour,
                                startTime.Minute,
                                startTime.Second );
        }
    }


}
