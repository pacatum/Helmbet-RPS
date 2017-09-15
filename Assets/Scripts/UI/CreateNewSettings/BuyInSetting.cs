using System;
using System.Collections.Generic;
using Base.Data.Assets;
using Base.Data.Tournaments;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class BuyInSetting : SettingView {

    [SerializeField] SettingsFieldsView buyinSettingsFieldView;
    [SerializeField] Dropdown balanceDropdown;
    [SerializeField] Text availableAmounText;

    List<AssetObject> assetsData = new List<AssetObject>();
    AssetObject selectAssetObject = new AssetObject();


    public uint BuyInAssetId {
        get { return selectAssetObject.Id.Id; }
    }


    protected override void Awake() {
        base.Awake();
        buyinSettingsFieldView.OnSettingStateChange += State_OnChanged;
        buyinSettingsFieldView.OnInputfieldStateChange += SetInputPressed;

        settingsFieldsViews.Add( buyinSettingsFieldView );

        foreach ( var input in settingsFieldsViews ) {
            input.OnSettingFieldValidate += SetErrorIcon;
        }
        AuthorizationManager.OnAuthorizationChanged += UpdateBalances;
        AuthorizationController.Instance.OnBalanceUpdate += ChangeBalance;

        balanceDropdown.onValueChanged.AddListener( SelectDropdownValue );


    }

    private void SelectDropdownValue( int value ) {
        foreach ( var assetData in assetsData ) {
            if ( assetData.Symbol.Equals( balanceDropdown.options[value].text ) ) {
                selectAssetObject = assetData;
            }
        }

        ApiManager.Instance.Database
            .GetAccountBalance( AuthorizationManager.Instance.UserData.FullAccount.Account.Id.Id, selectAssetObject.Id.Id )
            .Then( result => SetAvailableBalanceText( ( result.Amount / Mathf.Pow( 10, selectAssetObject.Precision ) ) + selectAssetObject.Symbol ) );
    }

    void SetAvailableBalanceText( string amount ) {
        availableAmounText.text = "Available: " + amount;
    }


    void ChangeBalance() {
        UpdateBalances( AuthorizationManager.Instance.Authorization );
    }

    void UpdateBalances( AuthorizationManager.AuthorizationData data ) {
        assetsData.Clear();
        balanceDropdown.ClearOptions();
        if ( data.IsNull() ) {
            return;
        }

        if ( AuthorizationController.Instance.accountBalances.Count == 0 ) {
            var options = new List<Dropdown.OptionData>();
            options.Add( new Dropdown.OptionData( "PPY" ) );
            balanceDropdown.AddOptions( options );
            TournamentManager.Instance.GetAssetObject()
                .Then( asset => {
                    selectAssetObject = asset;
                    SetAvailableBalanceText( 0 + selectAssetObject.Symbol );
                    assetsData.Add( asset );
                } );
        } else {
            TournamentManager.Instance.GetAssetsObject( Array.ConvertAll( AuthorizationController.Instance.accountBalances.ToArray(), assetId => assetId.Asset.Id ) )
                .Then( objects => {
                    var options = new List<Dropdown.OptionData>();
                    foreach ( var assetObject in objects ) {
                        assetsData.Add( assetObject );
                        options.Add( new Dropdown.OptionData( assetObject.Symbol ) );
                    }
                    balanceDropdown.AddOptions( options );
                    selectAssetObject = objects[0];
                    SetAvailableBalanceText( ( AuthorizationController.Instance.accountBalances[0].Amount / Mathf.Pow( 10, selectAssetObject.Precision ) ) + selectAssetObject.Symbol );
                } );
        }
    }

    void SetInputPressed( SettingsFieldsView target ) {
        target.CurrentState = SettingsFieldsView.SettingsFieldState.Pressed;
    }

    protected override void FieldView_OnStateChange( SettingsFieldsView.SettingsFieldState state ) {
        buyinSettingsFieldView.CurrentState = state;
    }

    public double BuyInAmount {
        get { return buyinSettingsFieldView.CurrentDoubleValue; }
    }

    public override void Clear() {
        buyinSettingsFieldView.CurrentDoubleValue = 0.0;
        selectAssetObject = assetsData[0];
    }


}
