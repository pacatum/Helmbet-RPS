using UnityEngine;
using UnityEngine.UI;

public class NoticesView : BaseCanvasView {

    [SerializeField] Button closeButton;
    [SerializeField] SettingsButtonView releasesBtn;
    [SerializeField] NoticeReleasesView noticeReleasesView;


    public override void Awake() {
        base.Awake();
        noticeReleasesView.OnCloseClick += Close;
        closeButton.onClick.AddListener( Close );
    }

    void Close() {
        UIManager.Instance.CurrentState = UIManager.Instance.PreviousState;
    }

    public override void Show() {
        base.Show();
        noticeReleasesView.Show();
    }

}
