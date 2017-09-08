using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopypasteController : MonoBehaviour {

    public event Action OnCopyClick;
    public event Action OnPasteClick;

    [SerializeField] Button copyButton;
    [SerializeField] Button pasteButton;


    void Awake() {
        copyButton.onClick.AddListener( Copy );
        pasteButton.onClick.AddListener( Paste );
    }

    public void Show() {
        gameObject.SetActive( true );
    }

    public void Hide() {
        gameObject.SetActive( false );
    }

    void Copy() {
        if ( OnCopyClick != null ) {
            OnCopyClick();
        }
        Hide();
    }

    void Paste() {
        if ( OnPasteClick != null ) {
            OnPasteClick();
        }
        Hide();
    }

}
