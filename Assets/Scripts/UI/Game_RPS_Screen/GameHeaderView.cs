﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameHeaderView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public event Action OnSettingsButton;
    public event Action OnMinimazeButton;

    [SerializeField] Color headerHoverColor;
    [SerializeField] Color headerNormalColor;
    [SerializeField] Color itemsHoverColor;
    [SerializeField] Color itemsNormalColor;
    [SerializeField] GameHeaderButtonView settingsButton;
    [SerializeField] GameHeaderButtonView soundButton;
    [SerializeField] GameHeaderButtonView minimazeWindowButton;
    [SerializeField] Image gameIcon;
    [SerializeField] Text gameTitleText;
    [SerializeField] GameObject volumeSettingView;
    [SerializeField] Button closeSoundView;

    Image bgImage;


    void Awake() {
        bgImage = GetComponent<Image>();
        settingsButton.GetComponent<Button>().onClick.AddListener( OnSettingsClick );
        soundButton.GetComponent<Button>().onClick.AddListener( OnVolumeSettingClick );
        minimazeWindowButton.GetComponent<Button>().onClick.AddListener( OnMinimaxeButtonClick );
        SetNormalState();
        volumeSettingView.SetActive( false );
        closeSoundView.onClick.AddListener( HideSoundView );
    }

    public void OnPointerEnter( PointerEventData eventData ) {
        SetHoverState();
    }

    public void OnPointerExit( PointerEventData eventData ) {
        if ( !volumeSettingView.activeSelf ) {
            SetNormalState();
        }
    }

    void SetNormalState() {
        settingsButton.CurrentState = GameHeaderButtonState.Normal;
        soundButton.CurrentState = GameHeaderButtonState.Normal;
        minimazeWindowButton.CurrentState = GameHeaderButtonState.Normal;
        bgImage.color = headerNormalColor;
        gameIcon.color = itemsNormalColor;
        gameTitleText.color = itemsNormalColor;
    }

    void SetHoverState() {
        bgImage.color = headerHoverColor;
        settingsButton.CurrentState = GameHeaderButtonState.HeaderHover;
        if ( soundButton.CurrentState != GameHeaderButtonState.Pressed ) {
            soundButton.CurrentState = GameHeaderButtonState.HeaderHover;
        }
        minimazeWindowButton.CurrentState = GameHeaderButtonState.HeaderHover;
        gameIcon.color = itemsHoverColor;
        gameTitleText.color = itemsHoverColor;
    }

    void OnSettingsClick() {
        if ( OnSettingsButton != null ) {
            OnSettingsButton();
        }
    }

    void OnVolumeSettingClick() {
        if ( volumeSettingView.activeSelf ) {
            HideSoundView();
        } else {
            ShowSoundView();
        }
        soundButton.CurrentState = volumeSettingView.activeSelf ? GameHeaderButtonState.Pressed : GameHeaderButtonState.HeaderHover;
    }

    void OnMinimaxeButtonClick() {
        if ( OnMinimazeButton != null ) {
            OnMinimazeButton();
        }
    }

    void ShowSoundView() {
        closeSoundView.gameObject.SetActive( true );
        soundButton.CurrentState = GameHeaderButtonState.Pressed;
        volumeSettingView.SetActive( true );
    }

    void HideSoundView() {
        closeSoundView.gameObject.SetActive(false);
        soundButton.CurrentState = GameHeaderButtonState.HeaderHover;
        volumeSettingView.SetActive(false);
        //SetNormalState();
    }

}
