using System;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class AccountView : BaseCanvasView {


    public event Action OnLogoutButton;

    bool isActive;

    [SerializeField] GameObject accountPanel;
    [SerializeField] Button logoutButton;
    [SerializeField] BalanceItemView balanceItemViewPrefab;
    [SerializeField] Transform balanceItemsPivot;


    public override void Awake() {
        base.Awake();
        logoutButton.onClick.AddListener( OnLogout_Click );
        UpdateAccountBalances();
        AuthorizationController.Instance.OnBalanceUpdate += UpdateAccountBalances;
        Hide();
    }

    public bool IsActive {
        get { return isActive; }
        set {
            isActive = value;
            if ( isActive ) {
                 UpdateAccountBalances() ;
            }
        }
    }


    void UpdateAccountBalances() {
        if ( AuthorizationManager.Instance.UserData.IsNull() ) {
            return;
        }
        
        for ( int i = 0; i < balanceItemsPivot.childCount; i++ ) {
            Destroy( balanceItemsPivot.GetChild( i ).gameObject );
        }

        if ( AuthorizationController.Instance.accountBalances.Count == 0 ) {
            ApiManager.Instance.Database.GetAsset()
                .Then( asset => {
                    var item = Instantiate( balanceItemViewPrefab );
                    item.transform.SetParent( balanceItemsPivot, false );
                    item.BalanceAmount = 0 + " " +
                                         asset.Symbol;
                    item.BalanceDescription = SetBalanceDecsription( asset.Symbol );
                } );
        } else {
            ApiManager.Instance.Database
                .GetAssets( Array.ConvertAll( AuthorizationController.Instance.accountBalances.ToArray(),
                                             asset => asset.Asset.Id ) )
                .Then( objects => {
                    for ( int i = 0; i < objects.Length; i++ ) {
                        var item = Instantiate( balanceItemViewPrefab );
                        item.transform.SetParent( balanceItemsPivot, false );
                        item.BalanceAmount = AuthorizationController.Instance.accountBalances[i].Amount /
                                             Mathf.Pow( 10, objects[i].Precision ) + " " +
                                             objects[i].Symbol;
                        item.BalanceDescription = SetBalanceDecsription( objects[i].Symbol );
                    }
                } );
        }
    }

    string SetBalanceDecsription(string symbol) {
        switch ( symbol ) {
            case "PPY":
                return "Base Core Tokens".ToUpper();

        }
        return string.Empty;
    }

    void OnLogout_Click() {
        IsActive = false;
        if ( OnLogoutButton != null ) {
            OnLogoutButton();
        }
    }

    public override void Show() {
        base.Show();
        IsActive = true;
    }

    public override void Hide() {
        base.Hide();
        IsActive = false;
        
    }

}
