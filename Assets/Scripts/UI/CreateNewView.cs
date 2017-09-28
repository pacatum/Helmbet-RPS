using System;
using System.Collections.Generic;
using Base.Data;
using Base.Transactions.Tournaments;
using UnityEngine;
using UnityEngine.UI;

public class CreateNewView : BaseCanvasView {

    public event Action OnCancelClick;
    public event Action OnGameInfoClick;

    [SerializeField] Button cancelButton;
    [SerializeField] Button gameMinInfoButton;
    [SerializeField] Button gameMaxInfoButton;
    [SerializeField] LoginButtonView createAndJoinButton;
    [SerializeField] LoginButtonView createButton;

    [SerializeField] RegistrationDeadlineCalendarView registrationDeadlineCalendarView;
    [SerializeField] RegistrationTime registrationTime;
    [SerializeField] NumberOfPlayers numberOfPlayers;
    [SerializeField] BuyInSetting buyInSetting;
    [SerializeField] WhitelistController whitelistController;

    [SerializeField] ToggleGroupView toggleGroup;
    [SerializeField] CreateNewErrorView ErrorFiledView;
    [SerializeField] ScreenLoader loader;
    [SerializeField] CreateNewTournamentPopupView createNewTournamentPopupView;
    [SerializeField] RectTransform scrollContainer;

    List<SettingView> settingsView = new List<SettingView>();
    List<LoginButtonView> buttons = new List<LoginButtonView>();
    bool allIsValidate;
    bool isJoin;


    public DateTime RegistrationDeadline {
        get {
            return new DateTime( registrationDeadlineCalendarView.CurrentValue.Year,
                                registrationDeadlineCalendarView.CurrentValue.Month,
                                registrationDeadlineCalendarView.CurrentValue.Day,
                                registrationTime.Hour,
                                registrationTime.Minute,
                                registrationTime.Second ).ToUniversalTime();
        }
    }

    public DateTime? StartTime {
        get {
            if ( toggleGroup.StartDateTime.HasValue ) {
                return toggleGroup.StartDateTime.Value.ToUniversalTime();
            }
            return null;
        }
    }

    public uint NumberOfPlayers {
        get { return numberOfPlayers.CurrentValue; }
    }

    public double BuyIn {
        get { return buyInSetting.BuyInAmount; }
    }
    

    public override void Show() {
        base.Show();
        scrollContainer.anchoredPosition = new Vector2(scrollContainer.anchoredPosition.x, 0f );
        Clear();
    }

    void Clear() {
        foreach ( var setting in settingsView ) {
            setting.Clear();
        }
        toggleGroup.Clear();
    }

    void OpenGameInfoView() {
        if ( OnGameInfoClick != null ) {
            OnGameInfoClick();
        }
    }
    

    public override void Awake() {
        base.Awake();

        settingsView.Add( registrationDeadlineCalendarView );
        settingsView.Add( registrationTime );

        settingsView.Add( numberOfPlayers );
        settingsView.Add( buyInSetting );
        settingsView.Add( whitelistController );

        buttons.Add( createAndJoinButton );
        buttons.Add( createButton );

        registrationDeadlineCalendarView.OnStateChanged += SwitchSettingState;
        registrationTime.OnStateChanged += SwitchSettingState;
        numberOfPlayers.OnStateChanged += SwitchSettingState;
        buyInSetting.OnStateChanged += SwitchSettingState;

        gameMaxInfoButton.onClick.AddListener( OpenGameInfoView );
        gameMinInfoButton.onClick.AddListener(OpenGameInfoView);

        foreach ( var setting in settingsView ) {
            setting.OnValidateChange += SettingsViewValidate_OnChange;
        }

        numberOfPlayers.OnValidateChange += toggleGroup.SetExaxtTimeAndDateToggle;

        cancelButton.onClick.AddListener( Cancel_OnClick );
        createAndJoinButton.GetComponent<Button>().onClick.AddListener( CreateNewTournamentAndJoin );
        createButton.GetComponent<Button>().onClick.AddListener( CreateTournament );

        ErrorFiledView.CorrectlyFilledInCount = 0;
        SettingsViewValidate_OnChange();
        ErrorFiledView.SetErrorMessage( false );

    }

    void SettingsViewValidate_OnChange() {


        ErrorFiledView.CorrectlyFilledInMaxCount = toggleGroup.ExtactTimeAndDate
            ? settingsView.Count
            : settingsView.Count + 1;

        int filledCount = 0;

        foreach ( var setting in settingsView ) {
            if ( setting.IsFilledIn ) {
                filledCount++;
            } else {
                CheckErrors();
            }
        }
        if ( !toggleGroup.ExtactTimeAndDate ) {
            filledCount++;
        }

        if ( filledCount == ErrorFiledView.CorrectlyFilledInMaxCount ) {
            allIsValidate = true;
            ErrorFiledView.SetErrorMessage( false );
            SwitchButtonsState( LoginButtonView.ButtonState.Active );
        } else {
            allIsValidate = false;
            SwitchButtonsState( LoginButtonView.ButtonState.Inactive );
        }

        ErrorFiledView.CorrectlyFilledInCount = filledCount;
    }


    void CheckErrors() {
        var tempList = new List<SettingView>( settingsView );
        foreach ( var setting in tempList ) {
            if ( !setting.IsFilledIn ) {
                ErrorFiledView.SetErrorMessage( true );
                UpdateErrorMessage( setting );
                break;
            }
        }
    }

    void SwitchSettingState( SettingView target ) {
        foreach ( var view in settingsView ) {
            if ( view.Equals( target ) ) {
                view.CurrentState = SettingView.SettingState.Pressed;
            } else {
                view.CurrentState = SettingView.SettingState.Normal;
            }
        }
    }

    void UpdateErrorMessage( SettingView view ) {
        ErrorFiledView.SetError( view.Name, view.ErrorMessage );
    }

    void Cancel_OnClick() {
        if ( OnCancelClick != null ) {
            OnCancelClick();
        }
    }

    void Create() {
        if ( allIsValidate ) {
            loader.IsLoading = true;
            TournamentTransactionService.GenerateCreateTournamentOperation( NewTournament() )
                .Then( operation => {
                    loader.IsLoading = false;
                    createNewTournamentPopupView.SetTournamentInformation( NewTournament(), isJoin );
                } );
        }
    }

    void CreateNewTournamentAndJoin() {
        isJoin = true;
        Create();
    }

    void CreateTournament() {
        isJoin = false;
        Create();
    }

	public CreateTournamentData NewTournament() {
		return new CreateTournamentData {
			buyInAssetId = buyInSetting.BuyInAssetId,
			buyInAmount = buyInSetting.BuyInAmount,
			whitelist = whitelistController.GetWhitelistIds.Length > 0
				? whitelistController.GetWhitelistIds
				: new SpaceTypeId[ 0 ],
			account = AuthorizationManager.Instance.Authorization.UserNameData.FullAccount.Account.Id,
			registrationDeadline = RegistrationDeadline,
			numberOfPlayers = numberOfPlayers.CurrentValue,
			numberOfWins = 5,
			startTime = StartTime,
			startDelay = toggleGroup.StartDelay,
			roundDelay = 10,
			insuranceEnabled = false,
			timePerCommitMove = 15,
			timePerRevealMove = 9
		};
	}

    void SwitchButtonsState( LoginButtonView.ButtonState state ) {
        foreach ( var button in buttons ) {
            button.UpdateState( state );
        }
    }

    public void AddViewsToList( SettingView view ) {
        settingsView.Add( view );
        view.OnStateChanged += SwitchSettingState;
        view.OnValidateChange += SettingsViewValidate_OnChange;
        SettingsViewValidate_OnChange();
    }

    public void RemoveViewFromList( SettingView view ) {
        settingsView.Remove( view );
        view.OnStateChanged -= SwitchSettingState;
        view.OnValidateChange -= SettingsViewValidate_OnChange;
        SettingsViewValidate_OnChange();
    }


    void OnEnable() {
        var calendars = FindObjectsOfType<CalendarControl>();
        foreach ( var item in calendars ) {
            item.gameObject.SetActive( false );
        }
    }
    
}
