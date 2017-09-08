using System;
using UnityEngine;
using UnityEngine.UI;


public class GlobalManager : SingletonMonoBehaviour<GlobalManager> {

    public event Action<InputField> OnCopyClick;
    public event Action<InputField> OnPasteClick;

    [SerializeField] CopypasteController copypasteController;

    InputField currentSelectObject;
    RaycastHit info;

	
    protected override  void Awake() {
        base.Awake();
        copypasteController.OnCopyClick += Copy;
        copypasteController.OnPasteClick += Paste;
        HideCopypastePanel();
    }

    void Update() {
        if ( Input.GetKeyUp( KeyCode.Escape ) ) {
            Application.Quit();
        }

        if ( Input.GetMouseButtonUp( 0 ) ) {
            copypasteController.Hide();
        }
    }

    public static string BufferString {
        get { return GUIUtility.systemCopyBuffer; }
        set { GUIUtility.systemCopyBuffer = value; }
    }

    public void ShowCopypastePanel(InputField current) {
        currentSelectObject = current;
        var panelPosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );
        copypasteController.transform.position =
            new Vector3( panelPosition.x + copypasteController.GetComponent<RectTransform>().rect.width / 2f,
                        panelPosition.y - copypasteController.GetComponent<RectTransform>().rect.height / 2f,
                        copypasteController.transform.position.z );
        copypasteController.Show();
    }

    public void HideCopypastePanel() {
        copypasteController.Hide();
    }

    void Copy() {
        if ( OnCopyClick != null ) {
            OnCopyClick(currentSelectObject);
        }
    }

    void Paste() {
        if ( OnPasteClick != null ) {
            OnPasteClick(currentSelectObject);
        }
    }
}