using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Base.Data;
using Base.Data.Accounts;
using Tools;
using UnityEngine;

public class AuthorizationController : SingletonMonoBehaviour<AuthorizationController> {

    public event Action OnBalanceUpdate;
    public List<AssetData> accountBalances = new List<AssetData>();


    protected override void Awake() {
        base.Awake();
        Repository.OnObjectUpdate += UpdateBalance;
        AuthorizationManager.OnAuthorizationChanged += UpdateAccountBalances;
    }

    void UpdateBalance( IdObject idObject ) {
        if ( !idObject.SpaceType.Equals( SpaceType.AccountBalance ) ) {
            return;
        }
        var accountBalance = idObject as AccountBalanceObject;
        if ( accountBalance.IsNull() ) {
            return;
        }

        var me = AuthorizationManager.Instance.Authorization;
        if ( !accountBalance.Owner.Equals( me.UserNameData.FullAccount.Account.Id ) ) {
            return;
        }

        UpdateAccountBalances( me );
    }

    void UpdateAccountBalances( AuthorizationManager.AuthorizationData data ) {
        if ( data.IsNull() ) {
            return;
        }

        var me = data.UserNameData.FullAccount;

        ApiManager.Instance.Database.GetAccountBalances( me.Account.Id.Id, Array.ConvertAll( me.Balances, balance => balance.AssetType.Id ) )
            .Then( result => {
                accountBalances.Clear();
                accountBalances.AddRange( result );
                if ( OnBalanceUpdate != null ) {
                    OnBalanceUpdate();
                }
            } );
    }

}
