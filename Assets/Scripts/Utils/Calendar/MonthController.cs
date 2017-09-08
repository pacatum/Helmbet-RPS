using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MonthController : MonoBehaviour {

    public event Action<string, int> OnMonthChange;

    [SerializeField] Button reduceButton;
    [SerializeField] Button increaseButton;

    int currentMounth;
    string monthTitle;


    void Awake() {
        reduceButton.onClick.AddListener( ReduceMonth );
        increaseButton.onClick.AddListener( IncreaseMonth );
        
        CurrentMonth = DateTime.UtcNow.Month;
    }

    void Start() {
    }

    public int CurrentMonth {
        get { return currentMounth; }
        set {
            currentMounth = value;
            UpdateMonth( currentMounth );
        }
    }

    void UpdateMonth( int month ) {

        switch ( month ) {
            case 1:
                monthTitle = "JAN";
                break;
            case 2:
                monthTitle = "FEB";
                break;
            case 3:
                monthTitle = "MAR";
                break;
            case 4:
                monthTitle = "APR";
                break;
            case 5:
                monthTitle = "MAY";
                break;
            case 6:
                monthTitle = "JUN";
                break;
            case 7:
                monthTitle = "JUL";
                break;
            case 8:
                monthTitle = "AUG";
                break;
            case 9:
                monthTitle = "SEP";
                break;
            case 10:
                monthTitle = "OCT";
                break;
            case 11:
                monthTitle = "NOV";
                break;
            case 12:
                monthTitle = "DEC";
                break;

        }

        Month_OnChange( monthTitle, currentMounth );
    }

    void Month_OnChange( string monthTitl, int monthNumber ) {
        if ( OnMonthChange != null ) {
            OnMonthChange( monthTitl, monthNumber );
        }
    }

    void ReduceMonth() {
        currentMounth--;
        if ( currentMounth <= 0 ) {
            CurrentMonth = 12;
        } else {
            CurrentMonth = currentMounth;
        }
    }

    void IncreaseMonth() {
        currentMounth++;
        if ( currentMounth > 12 ) {
            CurrentMonth = 1;
        } else {
            CurrentMonth = currentMounth;
        }
    }

}
