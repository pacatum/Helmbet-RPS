using System;
using System.Collections.Generic;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class AccountView : BaseCanvasView {


    public event Action OnLogoutButton;

    [SerializeField] GameObject accountPanel;
    [SerializeField] Button logoutButton;
    [SerializeField] BalanceItemView balanceItemViewPrefab;
    [SerializeField] Transform balanceItemsPivot;
    [SerializeField] TextMeshProUGUI balanceText;
    [SerializeField] Button closeButton;

    List<BalanceItemView> items = new List<BalanceItemView>();
    bool isActive;

    public override void Awake() {
        base.Awake();
        logoutButton.onClick.AddListener( OnLogout_Click );
        closeButton.onClick.AddListener( Hide );
        AuthorizationController.Instance.OnBalanceUpdate += UpdateAccountBalances;
        Hide();
    }

    public bool IsActive {
        get { return isActive; }
        set {
            isActive = value;
            if ( isActive ) {
                UpdateAccountBalances();
            }
        }
    }

    void ClearItemList() {
        foreach ( var item in items ) {
            Destroy( item.gameObject );
        }
        items.Clear();
    }

    void UpdateAccountBalances() {
        if ( AuthorizationManager.Instance.UserData.IsNull() ) {
            return;
        }
        if ( AuthorizationController.Instance.accountBalances.Count == 0 ) {
            ApiManager.Instance.Database.GetAsset()
                .Then( asset => {
                    ClearItemList();
                    var item = Instantiate( balanceItemViewPrefab );
                    item.transform.SetParent( balanceItemsPivot, false );
                    item.BalanceAmount = 0 + " " + asset.Symbol;
                    item.BalanceDescription = SetBalanceDecsription( asset.Symbol );
                    items.Add( item );
                    balanceText.text = item.BalanceAmount;
                } );
        } else {
            ApiManager.Instance.Database
                .GetAssets( Array.ConvertAll( AuthorizationController.Instance.accountBalances.ToArray(), asset => asset.Asset.Id ) )
                .Then( objects => {
                    ClearItemList();
                    for ( int i = 0; i < objects.Length; i++ ) {
                        var item = Instantiate( balanceItemViewPrefab );
                        items.Add( item );
                        item.transform.SetParent( balanceItemsPivot, false );
                        item.BalanceAmount = AuthorizationController.Instance.accountBalances[i].Amount / Mathf.Pow( 10, objects[i].Precision ) + " " + objects[i].Symbol;
                        item.BalanceDescription = SetBalanceDecsription( objects[i].Symbol );
                    }
                    balanceText.text = items[0].BalanceAmount;
                } );
        }
    }

    string SetBalanceDecsription( string symbol ) {
        switch ( symbol ) {
            case "PPY":
                return "Peerplays Core Tokens".ToUpper();
        }
        return string.Empty;
    }

    void OnLogout_Click() {
        IsActive = false;
        if ( OnLogoutButton != null ) {
            OnLogoutButton();
        }
    }

    void Update() {
        if ( Input.GetKeyUp( KeyCode.Escape ) ) {
            Hide();
        }
    }

    public override void Show() {
        base.Show();
        closeButton.gameObject.SetActive( true );
        IsActive = true;
    }

    public override void Hide() {
        base.Hide();
        closeButton.gameObject.SetActive( false );
        IsActive = false;

    }

}
