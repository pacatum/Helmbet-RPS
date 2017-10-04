using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupView : MonoBehaviour {

    [SerializeField] ToggleView twoMinutesToggle;
    [SerializeField] ToggleView exactTimeAndDateToggle;
    [SerializeField] StartTimePanelView startTimeView;

    bool extactTimeAndDate;
    CreateNewView createNewView;


    void Awake() {
        createNewView = FindObjectOfType<CreateNewView>();
        twoMinutesToggle.OnExactTimeAndDate += ToggleGroup_OnUpdate;
        ToggleGroup_OnUpdate( false );
        exactTimeAndDateToggle.OnExactTimeAndDate += ToggleGroup_OnUpdate;
    }

    void ToggleGroup_OnUpdate( bool active ) {
        extactTimeAndDate = active;
        SetStartTimePanel( active );
    }

    public bool ExtactTimeAndDate {
        get { return extactTimeAndDate; }
        set {
            extactTimeAndDate = value;
            SetStartTimePanel( value );
        }
    }

    public void SetExaxtTimeAndDateToggle() {
        if ( createNewView.NumberOfPlayers >= 8 ) {
            twoMinutesToggle.SetInteructable( false );
            twoMinutesToggle.UpdateToggle( false );
            exactTimeAndDateToggle.UpdateToggle( true );
            ExtactTimeAndDate = true;
        } else {
            twoMinutesToggle.SetInteructable( true );
        }
    }

    public void Clear() {
        ExtactTimeAndDate = false;
        twoMinutesToggle.UpdateToggle( true );
        exactTimeAndDateToggle.UpdateToggle( false );
        twoMinutesToggle.GetComponent<Toggle>().isOn = true;
        exactTimeAndDateToggle.GetComponent<Toggle>().isOn = false;
    }

    void SetStartTimePanel( bool active ) {
        startTimeView.gameObject.SetActive( active );
        startTimeView.IsActive = active;
    }

    public DateTime? StartDateTime {
        get { return extactTimeAndDate ? startTimeView.StartDateTime : null; }
    }

    public uint? StartDelay {
        get {
            uint? delay = 120;
            return extactTimeAndDate ? null : delay;
        }
    }

}
