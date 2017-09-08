using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CalendarControl : MonoBehaviour, ISelectHandler {

    public event Action<DateTime> OnDateChanged;
    public event Action<bool> OnCloseCalendar;

    [SerializeField] Text selectDay;
    [SerializeField] Text selectMonth;
    [SerializeField] Text selectYear;

    [SerializeField] MonthController monthController;

    [SerializeField] DayItemView dayItemViewPrefab;
    [SerializeField] Transform daysContainer;
    

    [Header( "Month" )]
    [SerializeField] Button reduceButton;
    [SerializeField] Button increaseButton;


    DateTime selectDate;
    List<DayItemView> dayItems = new List<DayItemView>();

    public DateTime ValidateDateTime { get; set; }

    public string SelectDay {
        get { return selectDay.text; }
    }

    public string SelectMonth {
        get { return selectMonth.text; }
    }

    public string SelectYear {
        get { return selectYear.text; }
    }

    void Awake() {
        selectDate = DateTime.UtcNow;
        UpdateDatetime();
        reduceButton.onClick.AddListener(ReduceMonth);
        increaseButton.onClick.AddListener(IncreaseMonth);
    }

    public void DrawCalendar() {
        for ( int i = 0; i < daysContainer.childCount; i++ ) {
            Destroy( daysContainer.GetChild( i ).gameObject );
            dayItems.Clear();
        }

        for ( var i = 0; i < 35; i++ ) {
            var item = Instantiate( dayItemViewPrefab );
            item.SetActiveState();
            if ( i + 1 <= DateTime.DaysInMonth( selectDate.Year, selectDate.Month ) ) {
                item.DayNumber = i + 1;
                if ( new DateTime( selectDate.Year, selectDate.Month, i + 1 ) <
                     new DateTime( ValidateDateTime.Year, ValidateDateTime.Month, ValidateDateTime.Day ) ) {
                    item.GetComponent<Button>().interactable = false;
                    item.SetInactiveState();
                }
            } else {
                item.DayNumber = i - DateTime.DaysInMonth( selectDate.Day, selectDate.Month ) + 1;
                item.GetComponent<Button>().interactable = false;
                item.SetInactiveState();
            }
            item.OnSelectDay += SwitchDayState;

            item.transform.SetParent( daysContainer, false );
            dayItems.Add( item );
        }
        
        dayItems[selectDate.Day-1].SetSelectColor();
        DateOnChanged();
    }

    void UpdateDatetime() {
        selectDay.text = selectDate.Day.ToString();
        selectMonth.text = UpdateMonth( selectDate.Month );
        selectYear.text = selectDate.Year.ToString();

        DrawCalendar();
    }
    

    void DateOnChanged() {
        if ( OnDateChanged != null ) {
            OnDateChanged( selectDate);
        }
    }

    public void OnSelect( BaseEventData eventData ) {
        if ( OnCloseCalendar != null ) {
            OnCloseCalendar( false );
        }
    }

    void SwitchDayState( DayItemView target ) {
        foreach ( var dayItem in dayItems ) {
            if ( dayItem.Equals( target ) ) {
                dayItem.SetSelectColor();
                selectDate = new DateTime( selectDate.Year, selectDate.Month, dayItem.DayNumber );
            } else {
                dayItem.SetNormalColor();
            }
        }
        UpdateDatetime();
    }

    string UpdateMonth( int month ) {
        switch ( month ) {
            case 1:
                return "JAN";
            case 2:
                return "FEB";
            case 3:
                return "MAR";
            case 4:
                return "APR";
            case 5:
                return "MAY";
            case 6:
                return "JUN";
            case 7:
                return "JUL";
            case 8:
                return "AUG";
            case 9:
                return "SEP";
            case 10:
                return "OCT";
            case 11:
                return "NOV";
            case 12:
                return "DEC";

        }
        return string.Empty;

    }

    void ReduceMonth() {
       selectDate= selectDate.AddMonths( -1 );
       UpdateDatetime();
    }

    void IncreaseMonth() {
        selectDate = selectDate.AddMonths(1);
        UpdateDatetime();
    }

}
