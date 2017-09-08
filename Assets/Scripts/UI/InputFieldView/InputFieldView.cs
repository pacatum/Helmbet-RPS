using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler {


    public enum InputState {

        Normal,
        Hover,
        Pressed,
        Error

    }


    public event Action<InputFieldView> OnInputClick;
    public event Action OnValueChange;

    [SerializeField] protected Sprite inputHover;
    [SerializeField] protected Sprite inputNormal;
    [SerializeField] protected Sprite inputPressed;
    [SerializeField] protected Sprite inputError;
    [SerializeField] protected Sprite inputFull;
    [SerializeField] protected GameObject inputHint;
    [SerializeField] protected GameObject inputPressedEffect;

    protected InputState currentState;
    protected Image image;
    protected InputField input;
    protected string inputText;
    protected RectTransform rectTransform;
    protected string selectedText;
    protected bool isSelect;
    protected int startInsertIndex = 0;

    public bool IsValidate { get; set; }
    private bool doubleClick = false;
    private float lastClickTime;
    public float catchTime = 0.25f;

    public string InputText {
        get { return inputText; }
    }

    protected virtual void Awake() {
        input = GetComponent<InputField>();
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        CurrentState = InputState.Normal;


        GlobalManager.Instance.OnCopyClick += Copy;
        GlobalManager.Instance.OnPasteClick += Paste;

    }

    protected bool InputPressedEffect {
        set {
            if ( inputPressedEffect != null ) {
                inputPressedEffect.gameObject.SetActive( value );
            }
        }
    }

    protected virtual void Start() {
    }

    protected virtual void Update() {
        if (Input.GetMouseButtonUp( 1 )&& isSelect ) {
            var startIndex = input.selectionAnchorPosition < input.selectionFocusPosition
                ? input.selectionAnchorPosition
                : input.selectionFocusPosition;
            var length = input.selectionAnchorPosition < input.selectionFocusPosition
                ? input.selectionFocusPosition - input.selectionAnchorPosition
                : input.selectionAnchorPosition - input.selectionFocusPosition;
            selectedText = input.text.Substring( startIndex, length );
            GlobalManager.Instance.ShowCopypastePanel( input );
            startInsertIndex = input.selectionAnchorPosition;
            doubleClick = false;
        }

        if ( Input.GetMouseButtonUp( 0 ) ) {
            if ( Time.time - lastClickTime < catchTime ) {
                doubleClick = true;
            } else {
                doubleClick = false;
            }
            lastClickTime = Time.time;
        }
    }


    protected void Copy(InputField target) {
        if ( !input.Equals( target )) {
            return;
        }
        GlobalManager.BufferString = selectedText;
    }

    protected void Paste(InputField target) {
        if ( !input.Equals( target ) ) {
            return;
        }

        if ( input.text == "" ) {
            input.text = GlobalManager.BufferString;
        } else {
            var pasteString = input.text.Insert( startInsertIndex, GlobalManager.BufferString );
            input.text = pasteString;
            inputText = input.text;
        }
       CurrentState = InputState.Normal;
    }

    public InputState CurrentState {
        get { return currentState; }
        set {
            currentState = value;
            if ( input != null ) {
                UpdateInput( currentState );
            }
        }
    }

    protected void UpdateInput( InputState state ) {
        InputPressedEffect = false;
        ActiveHint( false );
        switch ( state ) {
            case InputState.Normal:
                image.sprite = ( input.text.Length > 0 ) ? inputFull : inputNormal;
                break;
            case InputState.Hover:
                image.sprite = inputHover;
                break;
            case InputState.Error:
                image.sprite = inputError;
                break;
            case InputState.Pressed:
                InputPressedEffect = true;
                image.sprite = inputPressed;
                if ( !IsValidate ) {
                    ActiveHint( true );
                }
                break;
        }
    }

    protected virtual void ActiveHint( bool active ) {
        if ( inputHint != null ) {
            inputHint.SetActive( active );
        }
    }

    protected virtual void ValueChangeCheck() {

    }

    public void OnPointerClick( PointerEventData eventData ) {

    }

    public void OnPointerEnter( PointerEventData eventData ) {
        if ( currentState != InputState.Pressed ) {
            UpdateInput( InputState.Hover );
        }
    }

    public void OnPointerExit( PointerEventData eventData ) {
        if ( currentState != InputState.Pressed ) {
            UpdateInput( InputState.Normal );
        }
    }

    protected void InputText_OnChange() {
        if ( OnValueChange != null ) {
            OnValueChange();
        }
    }

    public void OnSelect( BaseEventData eventData ) {
        isSelect = true;
        if ( OnInputClick != null ) {
            OnInputClick( this );
        }
    }

    public void OnDeselect( BaseEventData eventData ) {
        isSelect = false;
        CurrentState = InputState.Normal;
    }

    public void Clear() {
        if ( input != null ) {
            input.text = "";
        }
        inputText = "";
    }

}
