using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CalendarButtonView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler{

    public event Action<DateTime> OnDateChanged;

    [SerializeField] Text selectDateText;

    public enum CalendarState {

        Pressed,
        Hover,
        Inactive

    }

    public event Action<SettingView.SettingState> OnSettingStateChange;

    [SerializeField] Color inactiveColor;
    [SerializeField] Color activeColor;

    [SerializeField] Image calendarIcon;
    [SerializeField] Image bgImage;
    [SerializeField] CalendarControl calendar;

    bool calendarIsOpen;
    CalendarState currentState;
    DateTime selectDate;

    public CalendarControl Calendar {
        get { return calendar; }
    }

    void Awake() {
        calendar.OnDateChanged += SetRegistratinDate;
        calendar.OnCloseCalendar += SetCalendar;
        calendar.DrawCalendar();
    }

    void Start() {
        selectDateText.text = calendar.SelectDay + " " + calendar.SelectMonth + " " + calendar.SelectYear;
        CalendarIsOpen = false;
    }

    protected bool CalendarIsOpen {
        get { return calendarIsOpen; }
        set {
            calendarIsOpen = value;
            SetCalendar( calendarIsOpen );
        }
    }

    public CalendarState CurrentState {
        get { return currentState; }
        set {

            currentState = value;
            UpdateCalendarState( currentState );
        }
    }

    void UpdateCalendarState( CalendarState state ) {
        switch ( state ) {
            case CalendarState.Hover:
                SetHoverView();
                break;
            case CalendarState.Pressed:
                SetPressedView();
                break;
            case CalendarState.Inactive:
                SetNormalView();
                break;
        }
    }

    public void OnPointerClick( PointerEventData eventData ) {
        if (OnSettingStateChange != null)
        {
            OnSettingStateChange(SettingView.SettingState.Pressed);
        }
        CurrentState = CalendarState.Pressed;
    }

    protected void SetNormalView() {
        if ( currentState != CalendarState.Pressed ) {
            bgImage.color = inactiveColor;
            calendarIcon.enabled = false;
            CalendarIsOpen = false;
        }
    }

    protected void SetPressedView() {
        bgImage.color = activeColor;
        calendarIcon.enabled = true;
        CalendarIsOpen = !CalendarIsOpen;
    }

    protected void SetHoverView() {
        bgImage.color = activeColor;
        calendarIcon.enabled = true;
    }

    void SetCalendar( bool active ) {
        
        calendarIsOpen = active;
        calendar.gameObject.SetActive( active );
     }


    public void OnPointerEnter( PointerEventData eventData ) {
        if ( currentState != CalendarState.Pressed ) {
            CurrentState = CalendarState.Hover;
        }
    }

    public void OnPointerExit( PointerEventData eventData ) {
        if ( currentState != CalendarState.Pressed ) {
            CurrentState = CalendarState.Inactive;
        }
    }

    public void SetRegistratinDate( DateTime date ) {
        selectDate = date;
        DateOnChanged( date );
        selectDateText.text = selectDate.Day +" " + MonthStringController.GetMonth( selectDate.Month ) + " " + selectDate.Year;
    }

    void DateOnChanged( DateTime date ) {
        if ( OnDateChanged != null ) {
            OnDateChanged( date );
        }
    }


}
