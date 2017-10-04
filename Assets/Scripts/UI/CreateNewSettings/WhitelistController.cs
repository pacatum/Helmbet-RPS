using System.Collections.Generic;
using Base.Data;
using Base.Data.Pairs;
using UnityEngine;
using UnityEngine.UI;

public class WhitelistController : SettingView {

    [SerializeField] Button addWhitelistButton;
    [SerializeField] AddNewPlayerToWhitelistView whitelistView;

    [SerializeField] UsernameItemView userPrefab;
    [SerializeField] Transform whitelistUsernamesContainer;
    [SerializeField] Button addButton;
    [SerializeField] MessagePopupView messagePopupView;

    CreateNewView createNewView;
    List<SpaceTypeId> whitelistIds = new List<SpaceTypeId>();
    List<UsernameItemView> usernameItemViews = new List<UsernameItemView>();


    public SpaceTypeId[] GetWhitelistIds {
        get { return whitelistIds.ToArray(); }
    }

    protected override void Awake() {
        addWhitelistButton.onClick.AddListener( OpenWhitelistView );
        createNewView = FindObjectOfType<CreateNewView>();
        whitelistView.OnPlayerAdded += AddNewUser;
    }

    void OpenWhitelistView() {
        whitelistView.Show();
    }

    void AddNewUser( UserNameAccountIdPair user ) {
        if ( whitelistIds.Contains( user.Id ) ) {
            messagePopupView.SerErrorPopup( "This user is already added to the whitelist!" );
        } else {
            AddUser( user );
            Validate_OnChange();
            whitelistView.Hide();
        }
    }

    void AddUser( UserNameAccountIdPair user ) {
        whitelistIds.Add( user.Id );
        var addedUser = Instantiate( userPrefab );
        addedUser.SetUsernameInfo( user.UserName, user.Id );
        usernameItemViews.Add( addedUser );
        addedUser.transform.SetParent( whitelistUsernamesContainer, false );
        whitelistUsernamesContainer.GetComponent<RectTransform>().sizeDelta = new Vector2( whitelistUsernamesContainer.GetComponent<RectTransform>().rect.width, addedUser.GetComponent<RectTransform>().rect.height * usernameItemViews.Count );
        addedUser.OnUserRemoved += RemoveUser;
    }

    void RemoveUser( UsernameItemView user ) {
        whitelistIds.Remove( user.CurrentSpaceTypeId );
        usernameItemViews.Remove( user );
        Destroy( user.gameObject );
        whitelistUsernamesContainer.GetComponent<RectTransform>().sizeDelta = new Vector2( whitelistUsernamesContainer.GetComponent<RectTransform>().rect.width, user.GetComponent<RectTransform>().rect.height * usernameItemViews.Count );
        Validate_OnChange();
    }

    protected override void SetHoverView() {
    }

    protected override void SetNormalView() {
    }

    protected override void SetPressedView() {
    }

    public override void Clear() {
        whitelistIds.Clear();
        foreach ( var item in usernameItemViews ) {
            Destroy( item.gameObject );
        }
        usernameItemViews.Clear();
        whitelistUsernamesContainer.GetComponent<RectTransform>().sizeDelta = new Vector2( whitelistUsernamesContainer.GetComponent<RectTransform>().rect.width,
                                                                                          0f );
        whitelistView.Hide();
    }

    public override bool IsFilledIn {
        get { return whitelistIds.Count == 0 || whitelistIds.Count >= createNewView.NumberOfPlayers; }
    }

}
