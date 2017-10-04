using System.Collections;
using System.Collections.Generic;
using Base.Data.Tournaments;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using Base.Config;
using Base.Data.Assets;
using UnityEngine.EventSystems;

using TMPro;
using Tools;

public class BaseTournamentItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    [SerializeField] RectTransform itemRectTransform;

    [SerializeField] protected TextMeshProUGUI idText;
    [SerializeField] protected TextMeshProUGUI playerRegisteredText;
    [SerializeField] protected TextMeshProUGUI maxPlayersText;
    [SerializeField] protected TextMeshProUGUI buyInText;
    [SerializeField] protected TextMeshProUGUI jackpotText;
    [SerializeField] protected RectTransform numberOfPlayersObject;
    [SerializeField] protected TournamentItemFooterView footerView;

    [SerializeField] protected UnityEngine.GameObject idTextTitle;
    [SerializeField] protected UnityEngine.GameObject numberOfPlayersTextTitle;
    [SerializeField] protected UnityEngine.GameObject buyInTextTitle;
    [SerializeField] protected UnityEngine.GameObject jackpotTextTitle;
    [SerializeField] protected UnityEngine.GameObject winnerTextTitle;
    [SerializeField] protected UnityEngine.GameObject resultTextTitle;


    protected Image itemBackground;
    protected TournamentObject currentTournament;
    protected TournamentDetailsObject tournamentDetailsObject;
    protected AssetObject currentAsset;

    [Header( "Item width" )] [SerializeField] private float increaseWidth = 140;
    [SerializeField] float animationTimer;
    [SerializeField] Image itemShadowImage;

    protected bool isHover;
    protected bool stopUpdating;
    float hoverWidth;
    float normalWidth;


    public virtual TournamentObject CurrentTournament {
        get { return currentTournament; }
        set { currentTournament = value; }
    }

    protected virtual void Start() {
        isHover = false;
        stopUpdating = true;
        itemBackground = GetComponent<Image>();
        normalWidth = 0;
        hoverWidth = normalWidth + increaseWidth;
        itemRectTransform.sizeDelta = new Vector2( normalWidth, itemRectTransform.rect.height );
    }

    public bool IsHover {
        get { return isHover; }
        set {
            isHover = value;
            if ( !isHover ) {
                UpdateView( false );
                GetComponent<Canvas>().sortingOrder = 3;
                footerView.UpdateFooter( currentTournament, tournamentDetailsObject, false );
                stopUpdating = false;
            }
        }
    }


    public string ID {
        get { return ( idText == null ) ? string.Empty : idText.text; }
        protected set {
            if ( idText != null ) {
                idText.text = value;
            }
        }
    }

    public string PlayerRegistered {
        get { return ( playerRegisteredText == null ) ? string.Empty : playerRegisteredText.text; }
        protected set {
            if ( playerRegisteredText != null ) {
                playerRegisteredText.text = value;
            }
        }
    }

    public string MaxPlayers {
        get { return ( maxPlayersText == null ) ? string.Empty : maxPlayersText.text; }
        protected set {
            if ( maxPlayersText != null ) {
                maxPlayersText.text = value;
            }
        }
    }

    public string BuyIn {
        get { return ( buyInText == null ) ? string.Empty : buyInText.text; }
        protected set {
            if ( buyInText != null ) {
                buyInText.text = value;
            }
        }
    }

    public string Jackpot {
        get { return ( jackpotText == null ) ? string.Empty : jackpotText.text; }
        protected set {
            if ( jackpotText != null ) {
                jackpotText.text = value;
            }
        }
    }

    public virtual IEnumerator UpdateItem( TournamentObject info ) {
        if ( gameObject.activeSelf && gameObject.activeInHierarchy ) {
            CurrentTournament = info;

            var detailsObject = new List<TournamentDetailsObject>();
            yield return TournamentManager.Instance.GetTournamentDetailsObject( info.Id.Id, detailsObject );
            tournamentDetailsObject = detailsObject[0];

            ID = "#RPS" + info.Id;
            PlayerRegistered = info.RegisteredPlayers.ToString();
            MaxPlayers = info.Options.NumberOfPlayers.ToString();

            if ( currentAsset.IsNull() || !currentAsset.Id.Equals( currentTournament.Options.BuyIn.Asset ) ) {
                AssetObject asset = null;
                TournamentManager.Instance.GetAssetObject( currentTournament.Options.BuyIn.Asset.Id )
                    .Then( assetResult => asset = assetResult );
                while ( asset.IsNull() ) {
                    yield return null;
                }
                currentAsset = asset;
            }

            var buyIn = Decimal.Parse( ( info.Options.BuyIn.Amount / Math.Pow( 10, currentAsset.Precision ) ).ToString(), NumberStyles.Float );

            BuyIn = buyIn + currentAsset.Symbol;
            Jackpot = buyIn * info.Options.NumberOfPlayers + currentAsset.Symbol;
            UpdateStartTime();
        }
    }

    protected virtual void UpdateActions() {
    }

    protected virtual void UpdateStartTime() {
    }

    protected virtual string SetDateTime( string dateTime ) {
        var time = DateTime.MinValue;
        if ( DateTime.TryParse( dateTime, out time ) ) {
            return time.ToLocalTime().ToString( "ddMMM, yyyy. HH:mm tt" );
        }
        return time.ToString( "ddMMM, yyyy. HH:mm tt" );
    }


    public virtual void UpdateTournament( TournamentObject tournament ) {
    }

    public void UpdateDetails( TournamentObject tournament ) {
        StartCoroutine( UpdateItem( tournament ) );
    }

    protected void Update() {

        if ( stopUpdating ) {
            return;
        }

        if ( isHover ) {
            if ( itemRectTransform.sizeDelta.x < hoverWidth ) {
                itemRectTransform.sizeDelta = new Vector2( itemRectTransform.sizeDelta.x + animationTimer, itemRectTransform.sizeDelta.y );
                var color = itemShadowImage.color;
                color.a += animationTimer;
                itemShadowImage.color = color;
            } else {
                itemRectTransform.sizeDelta = new Vector2( hoverWidth, itemRectTransform.sizeDelta.y );
                stopUpdating = true;
            }
        } else {
            if ( itemRectTransform.sizeDelta.x > normalWidth ) {
                itemRectTransform.sizeDelta = new Vector2( itemRectTransform.sizeDelta.x - animationTimer, itemRectTransform.sizeDelta.y );
                var color = itemShadowImage.color;
                color.a -= animationTimer;
                itemShadowImage.color = color;
            } else {
                itemRectTransform.sizeDelta = new Vector2( normalWidth, itemRectTransform.sizeDelta.y );
                stopUpdating = true;
            }
        }
    }

    public void OnPointerExit( PointerEventData eventData ) {
        UpdateView( false );
        GetComponent<Canvas>().sortingOrder = 3;
        isHover = false;
        footerView.UpdateFooter( currentTournament, tournamentDetailsObject, false );
        stopUpdating = false;
    }

    public void OnPointerEnter( PointerEventData eventData ) {
        UpdateView( true );
        GetComponent<Canvas>().sortingOrder = 6;
        isHover = true;
        footerView.UpdateFooter( currentTournament, tournamentDetailsObject, true );
        stopUpdating = false;
    }

    public void OnPointerClick( PointerEventData eventData ) {
        footerView.OpenGameScreen();
    }

    protected virtual void UpdateView( bool isHover ) {
        idTextTitle.SetActive( isHover );
        numberOfPlayersTextTitle.SetActive( isHover );
        buyInTextTitle.SetActive( isHover );
        jackpotTextTitle.SetActive( isHover );
        if ( isHover ) {
            idText.GetComponent<RectTransform>().pivot = buyInText.GetComponent<RectTransform>().pivot =
                jackpotText.GetComponent<RectTransform>().pivot = numberOfPlayersObject.pivot = new Vector2( 1f, 0.5f );

            idText.GetComponent<RectTransform>().anchorMax = buyInText.GetComponent<RectTransform>().anchorMax = jackpotText.GetComponent<RectTransform>().anchorMax = numberOfPlayersObject.anchorMax =
                idText.GetComponent<RectTransform>().anchorMin = buyInText.GetComponent<RectTransform>().anchorMin = jackpotText.GetComponent<RectTransform>().anchorMin = numberOfPlayersObject.anchorMin = new Vector2( 1f, 1f );

            idText.GetComponent<RectTransform>().anchoredPosition = new Vector2( -27, idText.GetComponent<RectTransform>().anchoredPosition.y );
            buyInText.GetComponent<RectTransform>().anchoredPosition = new Vector2( -27, buyInText.GetComponent<RectTransform>().anchoredPosition.y );
            jackpotText.GetComponent<RectTransform>().anchoredPosition = new Vector2( -27, jackpotText.GetComponent<RectTransform>().anchoredPosition.y );
            numberOfPlayersObject.anchoredPosition = new Vector2( -27, numberOfPlayersObject.GetComponent<RectTransform>().anchoredPosition.y );

        } else {
            idText.GetComponent<RectTransform>().pivot = playerRegisteredText.GetComponent<RectTransform>().pivot = buyInText.GetComponent<RectTransform>().pivot =
                jackpotText.GetComponent<RectTransform>().pivot = numberOfPlayersObject.pivot = new Vector2( 0.5f, 0.5f );

            idText.GetComponent<RectTransform>().anchorMax = buyInText.GetComponent<RectTransform>().anchorMax = jackpotText.GetComponent<RectTransform>().anchorMax =
                numberOfPlayersObject.anchorMax = idText.GetComponent<RectTransform>().anchorMin = buyInText.GetComponent<RectTransform>().anchorMin = jackpotText.GetComponent<RectTransform>().anchorMin = numberOfPlayersObject.anchorMin = new Vector2( 0.5f, 1f );

            idText.GetComponent<RectTransform>().anchoredPosition = new Vector2( 0, idText.GetComponent<RectTransform>().anchoredPosition.y );
            buyInText.GetComponent<RectTransform>().anchoredPosition = new Vector2( 0, buyInText.GetComponent<RectTransform>().anchoredPosition.y );
            jackpotText.GetComponent<RectTransform>().anchoredPosition = new Vector2( 0, jackpotText.GetComponent<RectTransform>().anchoredPosition.y );
            numberOfPlayersObject.anchoredPosition = new Vector2( 0, numberOfPlayersObject.GetComponent<RectTransform>().anchoredPosition.y );
        }
    }


}
