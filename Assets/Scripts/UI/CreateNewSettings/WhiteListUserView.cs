using System;
using Base.Data.Pairs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WhiteListUserView : MonoBehaviour, IPointerClickHandler {

    public event Action<UserNameAccountIdPair> OnItemClick;

    [SerializeField] Text usernameText;
    UserNameAccountIdPair currentUser;


    public void Init( UserNameAccountIdPair account ) {
        usernameText.text = account.UserName;
        currentUser = account;
    }

    public void OnPointerClick( PointerEventData eventData ) {
        if ( OnItemClick != null ) {
            OnItemClick( currentUser );
        }
    }

}
