using System;
using UnityEngine;

public class FilterCurrencyExpandView : FilterItemExpandView {

    [SerializeField] FilterChoiseItemView itemPrefab;
    [SerializeField] Transform itemsContainer;
    private bool firstUpdate = true;


    protected override void Awake() {
        UpdateCurrencies();
    }

    void UpdateCurrencies() {
        TournamentManager.Instance.GetAssetsObject( Array.ConvertAll( AuthorizationController.Instance.accountBalances.ToArray(), assetId => assetId.Asset.Id ) )
            .Then( objects => {

                foreach ( var item in items ) {
                    Destroy( item.gameObject );
                }
                items.Clear();

                InstantiateItem( "ANY" );
                foreach ( var assetObject in objects ) {
                    InstantiateItem( assetObject.Symbol );
                }
                base.Awake();
            } );
    }

    public override void ShowExpandView() {
        if ( firstUpdate ) {
            UpdateCurrencies();
            firstUpdate = false;
        }

        base.ShowExpandView();
    }

    void InstantiateItem( string value ) {
        var item = Instantiate(itemPrefab);
        item.SelectChoise = value;
        item.transform.SetParent( itemsContainer, false );
        items.Add(item);
    }

}
