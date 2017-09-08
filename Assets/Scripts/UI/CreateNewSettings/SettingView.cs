using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public enum SettingState {

        Normal,
        Pressed,
        Hover

    }

    public event Action<SettingView> OnStateChanged;
    public event Action OnValidateChange;

    [SerializeField] protected Color activeColor;
    [SerializeField] protected Color inactiveColor;

    [SerializeField] protected Text inactiveText;
    [SerializeField] protected Image errorIcon;
    [SerializeField] protected string errorMessage;
    [SerializeField] protected string inputfieldTitle;

    protected Image bgImage;
    protected SettingState currentState;
    protected List<SettingsFieldsView> settingsFieldsViews = new List<SettingsFieldsView>();
    protected Animator animator;


    public SettingState CurrentState {
        get { return currentState; }
        set {
            if ( currentState != value ) {
                currentState = value;
                UpdateView( currentState );
            }
        }
    }

    public string Name {
        get { return inputfieldTitle; }
    }

    public virtual string ErrorMessage {
        get { return errorMessage; }
    }

    public virtual bool IsFilledIn {
        get {
            foreach ( var settingfield in settingsFieldsViews ) {
                if ( !settingfield.IsFilledIn ) {
                    return false;
                }
            }
            return true;
        }
    }

    protected virtual void Awake() {
        bgImage = GetComponent<Image>();
        animator = inactiveText.GetComponent<Animator>();
        errorIcon.gameObject.SetActive(false);
        SetNormalView();
    }

    protected void UpdateView( SettingState state ) {
        switch ( state ) {
            case SettingState.Normal:
                SetNormalView();
                break;
            case SettingState.Hover:
                SetHoverView();
                break;
            case SettingState.Pressed:
                SetPressedView();
                break;
        }
    }

    protected virtual void FieldView_OnStateChange( SettingsFieldsView.SettingsFieldState state ) {

    }


    protected void State_OnChanged( SettingState state ) {
        CurrentState = state;
        if ( OnStateChanged != null ) {
            OnStateChanged( this );
        }
    }


    protected virtual void SetNormalView() {
        bgImage.color = inactiveColor;
        animator.SetTrigger( "Normal" );
        FieldView_OnStateChange( SettingsFieldsView.SettingsFieldState.Inactive );
    }

    protected virtual void SetHoverView() {
        bgImage.color = activeColor;
        animator.SetTrigger( "Pressed" );
        FieldView_OnStateChange( SettingsFieldsView.SettingsFieldState.Hover );

    }

    protected virtual void SetPressedView() {
        bgImage.color = activeColor;
        //animator.SetTrigger("Pressed");
    }

    public void OnPointerEnter( PointerEventData eventData ) {
        if ( currentState != SettingState.Pressed ) {
            UpdateView( SettingState.Hover );
        }
    }

    public void OnPointerExit( PointerEventData eventData ) {
        if ( currentState != SettingState.Pressed ) {
            UpdateView( SettingState.Normal );
        }
    }

    public void OnPointerClick( PointerEventData eventData ) {
        if ( OnStateChanged != null ) {
            OnStateChanged( this );
        }
    }

    public void SetErrorIcon() {
        errorIcon.gameObject.SetActive( false );
        foreach ( var inputField in settingsFieldsViews ) {
            if ( !inputField.IsFilledIn ) {
                errorIcon.gameObject.SetActive( true );
            }
        }
        Validate_OnChange();
    }

    protected void Validate_OnChange() {
        if ( OnValidateChange != null ) {
            OnValidateChange();
        }
    }


    public virtual void Clear() {
    }
}
