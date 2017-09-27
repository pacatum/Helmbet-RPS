using System;
using System.Collections;
using System.Collections.Generic;
using Base.Data;
using Base.Data.Pairs;
using UnityEngine;
using UnityEngine.UI;

public class AddNewPlayerToWhitelistView : MonoBehaviour {

    public event Action<UserNameAccountIdPair> OnPlayerAdded;


    private string inputText;
    [SerializeField] InputField input;
    [SerializeField] WhiteListUserView userItemView;
    [SerializeField] Transform usersContainer;


    List<WhiteListUserView> userGameObjects = new List<WhiteListUserView>();
    SetItemToDefaultByEscape setItemToDefaultByEscape;

    private SetItemToDefaultByEscape SetItemToDefaultComponent {
        get { return setItemToDefaultByEscape == null ? setItemToDefaultByEscape = GetComponent<SetItemToDefaultByEscape>() : setItemToDefaultByEscape; }
    }

    void Awake() {
        input.onValueChanged.AddListener( delegate {OnValue_Changed();});
        SetItemToDefaultComponent.OnEscapeClick += Hide;
        Hide();
    }

    public void Show() {
        gameObject.SetActive( true );
        LookupAccounts( "" );
    }

    public void Hide() {
        Clear();

        input.text = string.Empty;
        gameObject.SetActive( false );
    }

    void OnValue_Changed() {
        inputText = input.text;
        LookupAccounts( inputText );
    }

    void LookupAccounts( string username ) {
        Clear();
        ApiManager.Instance.Database.LookupAccounts( username, 10 )
            .Then( accountsResult => {
                foreach ( var account in accountsResult ) {
                    var user = Instantiate( userItemView );
                    user.Init( account );
                    user.OnItemClick += UserViewOnClick;
                    user.transform.SetParent( usersContainer, false );
                    userGameObjects.Add( user );
                }
            } );
    }

    void UserViewOnClick(UserNameAccountIdPair id) {
        if ( OnPlayerAdded != null ) {
            OnPlayerAdded( id );
        }
    }

    void Clear() {
        if ( userGameObjects.Count == 0 ) {
            return;
        }

        inputText = string.Empty;

        foreach ( var user in userGameObjects ) {
            Destroy( user.gameObject );
        }
        userGameObjects.Clear();
    }
}
