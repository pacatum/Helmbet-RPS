﻿
using System;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SearchView : MonoBehaviour, IDeselectHandler {

    public event Action<string> OnValueChange;

    string searchFilterText;
    InputField input;
    private string selectedText;
    protected int startInsertIndex = 0;
    bool copyPaste;


    void Awake() {

        input = GetComponent<InputField>();
        searchFilterText = input.text;
        input.onEndEdit.AddListener( delegate {
            Value_OnChange();
        } );

        GlobalManager.Instance.OnCopyClick += Copy;
        GlobalManager.Instance.OnPasteClick += Paste;

    }

    void Value_OnChange() {
        if ( !searchFilterText.Equals( input.text ) ) {
            searchFilterText = input.text;
            if ( OnValueChange != null ) {
                OnValueChange( searchFilterText );
            }
            gameObject.SetActive( false );
        }
    }

    void Update() {
        if ( Input.GetMouseButtonUp( 1 ) ) {
            var startIndex = input.selectionAnchorPosition < input.selectionFocusPosition
                ? input.selectionAnchorPosition
                : input.selectionFocusPosition;
            var length = input.selectionAnchorPosition < input.selectionFocusPosition
                ? input.selectionFocusPosition - input.selectionAnchorPosition
                : input.selectionAnchorPosition - input.selectionFocusPosition;
            selectedText = input.text.Substring( startIndex, length );
            GlobalManager.Instance.ShowCopypastePanel( input );
            startInsertIndex = input.selectionAnchorPosition;
        }

        if ( Input.GetKeyUp( KeyCode.Return )) {
            Value_OnChange();

            gameObject.SetActive(false);
        }
        
    }

    protected void Copy( InputField target ) {
        if ( !input.Equals( target ) ) {
            return;
        }
        GlobalManager.BufferString = selectedText;
    }

    protected void Paste( InputField target ) {
        if ( !input.Equals( target ) ) {
            return;
        }

        if ( input.text == "" ) {
            input.text = GlobalManager.BufferString;
        } else {
            var pasteString = input.text.Insert( startInsertIndex, GlobalManager.BufferString );
            input.text = pasteString;
        }
    }

 

    void IDeselectHandler.OnDeselect( BaseEventData eventData ) {
      //  gameObject.SetActive( false );
    }

    public void Show() {
        gameObject.SetActive( true );
        input.OnSelect( new PointerEventData( EventSystem.current ) );
    }

    public void Clear() {
        if ( input != null ) {
            input.text = searchFilterText = string.Empty;
        }

    }

}
