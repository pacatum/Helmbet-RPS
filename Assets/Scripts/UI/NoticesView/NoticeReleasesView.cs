using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoticeReleasesView : BaseCanvasView {

    public event Action OnCloseClick;

    [SerializeField] Button closeViewButton;
    [SerializeField] Button UpdateButton;

    [SerializeField] TextMeshProUGUI releasesText;
    [SerializeField] GameObject noticesButton;

    string versionURL;
    string currentVersion;

    public override void Awake() {
        closeViewButton.onClick.AddListener( Close_OnClick );
        UpdateManager.OnNewVersionAvailable += UpdateVersionInfo;
        UpdateButton.onClick.AddListener( GoToURL );
    }

    void Close_OnClick() {
        if ( OnCloseClick != null ) {
            OnCloseClick();
        }
    }

    public override void Show() {
        base.Show();
        releasesText.text = "Current version: Helmbet-RPS v" + UpdateManager.CURRENT_VERSION;
        UpdateButton.gameObject.SetActive( false );
        noticesButton.SetActive( false );
    }

    void UpdateVersionInfo( UpdateManager.VersionInfo version ) {
        noticesButton.SetActive( true );
        releasesText.text = "New version is available now: Helmbet-RPS v" + version.Version + "\n" + version.Title;
        versionURL = version.Url;
        UpdateButton.gameObject.SetActive( true );
        UpdateButton.GetComponentInChildren<TextMeshProUGUI>().text = "Update to v" + version.Version;
    }

    void GoToURL() {
        if ( !versionURL.Equals( string.Empty ) ) {
            Application.OpenURL( versionURL );
        }
    }

}
