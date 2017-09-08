using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DayItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Action<DayItemView> OnSelectDay;

    public Text dayNumberText;
    [SerializeField] Color hoverImageColor;
    [SerializeField] Color selectImageColor;
    [SerializeField] Color pressedImageColor;
    [SerializeField] Color normalColor;
    [SerializeField] Color inactiveTextColor;

    private int dayNumber;
    Image backgroundImage;


    public bool IsSelect { get; set; }
    public bool IsInactive { get; set; }

    void Awake() {
        GetComponent<Button>().onClick.AddListener( OnPointerClick );
        backgroundImage = GetComponent<Image>();
    }

    public int DayNumber {
        get { return dayNumber; }
        set {
            dayNumber = value;
            dayNumberText.text = dayNumber.ToString();
        }
    }

    public void OnPointerClick() {
        if ( OnSelectDay != null && !IsInactive ) {
            OnSelectDay( this );
        }
    }

    public void OnPointerEnter( PointerEventData eventData ) {
        if ( !IsSelect && !IsInactive ) {
            backgroundImage.color = hoverImageColor;
        }
    }

    public void OnPointerExit( PointerEventData eventData ) {
        if ( !IsSelect && !IsInactive ) {
            backgroundImage.color = normalColor;
        }
    }

    public void SetSelectColor() {
        if ( !IsInactive ) {
            IsSelect = true;
            backgroundImage.color = selectImageColor;
        }
    }

    public void SetNormalColor() {
        if ( !IsInactive ) {
            IsSelect = false;
            backgroundImage.color = normalColor;
        }
    }

    public void SetInactiveState() {
        IsInactive = true;
        dayNumberText.color = inactiveTextColor;
    }

    public void SetActiveState() {
        IsInactive = false;
        SetNormalColor();
    }

}
