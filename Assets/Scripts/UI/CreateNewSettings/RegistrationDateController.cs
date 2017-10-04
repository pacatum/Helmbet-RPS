using System;
using UnityEngine;

public enum DateTimeType {

    RegistrationDeadline,
    StartTime

}

public class RegistrationDateController : MonoBehaviour {

    [SerializeField] RegistrationDeadlineCalendarView calendarView;
    [SerializeField] RegistrationTime timeView;
    [SerializeField] DateTimeType currentDateTimeType;


    public DateTime CurrentDateTime {
        get {
            return new DateTime( calendarView.CurrentValue.Year,
                                calendarView.CurrentValue.Month,
                                calendarView.CurrentValue.Day,
                                timeView.Hour,
                                timeView.Minute,
                                timeView.Second );
        }
    }

    public bool IsFilledIn {
        get {
            return CurrentDateTime > new DateTime( DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day + 1,
                                                  DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second );
        }
    }

}
