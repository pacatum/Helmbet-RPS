using System;
using System.Collections;
using System.Collections.Generic;
using Base.Data;
using UnityEngine;
using UnityEngine.UI;

public class UsernameItemView : MonoBehaviour {

    public event Action<UsernameItemView> OnUserRemoved;

    [SerializeField] Button removeButton;
    [SerializeField] Text usernameText;

    SpaceTypeId currentSpaceTypeId;

    public SpaceTypeId CurrentSpaceTypeId {
        get { return currentSpaceTypeId; }
    }

    void Awake() {
        removeButton.onClick.AddListener( RemoveUserFromWhitelist );
    }

    void RemoveUserFromWhitelist() {
        if ( OnUserRemoved != null ) {
            OnUserRemoved( this );
        }
    }

    public void SetUsernameInfo(string username, SpaceTypeId userId) {
        currentSpaceTypeId = userId;
        usernameText.text = username;
    }
}
