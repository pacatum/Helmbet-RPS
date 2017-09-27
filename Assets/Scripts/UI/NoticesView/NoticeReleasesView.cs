using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeReleasesView : MonoBehaviour {

    public event Action OnCloseClick;

    [SerializeField] Button closeViewButton;
    [SerializeField] Button UpdateButton;


    void Awake() {
        closeViewButton.onClick.AddListener( Close_OnClick );
    }

    void Close_OnClick() {
        if ( OnCloseClick != null ) {
            OnCloseClick();
        }
    }

}
