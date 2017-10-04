using UnityEngine;
using UnityEngine.UI;

public class GameSettingView : MonoBehaviour {

    [SerializeField] Button applyButton;
    [SerializeField] Button cancelButton;
    [SerializeField] SkinnedMeshRenderer playerMeshRendererHand;
    [SerializeField] Image CurrentChoosedHand;
    [SerializeField] ChooseHandButton ChooseHandButtonPrefab;
    [SerializeField] Transform LayoutGroup;

    int viewIndex;
    ChooseHandController.HandSetting currentHandSetting;
    ChooseHandController.HandSetting selectedHandSetting;


    void Awake() {
        applyButton.onClick.AddListener( ApplyOnClick );
        cancelButton.onClick.AddListener( Cancel_OnClick );
        ChooseHandController.Instance.OnChooseColor += SetCurrentChoosedHand;
        ChooseHandController.Instance.OnUpdateHandPreview += UpdateHandPreview;
        var currentColor = (ChooseHandController.HandColour) PlayerPrefs.GetInt( "PlayerChoosedHand" );
        currentHandSetting = ChooseHandController.Instance.HandsList.Find( h => h.ColourOfHand == currentColor );
        if ( playerMeshRendererHand != null ) {
            playerMeshRendererHand.material = currentHandSetting.HandMaterial;
        }
        gameObject.SetActive( false );
    }

    public void Show() {
        gameObject.SetActive( true );
        viewIndex = ChooseHandController.Instance.HandsList.IndexOf( currentHandSetting );
        ChangeViewIndex( 0 );
    }

    public void Hide() {
        Cancel_OnClick();
    }

    public void SetCurrentChoosedHand( ChooseHandController.HandColour hand ) {
        selectedHandSetting = ChooseHandController.Instance.HandsList.Find( h => h.ColourOfHand == hand );
    }

    public void UpdateHandPreview( ChooseHandController.HandSetting setting ) {
        CurrentChoosedHand.sprite = setting.HandPalmSprite;
    }

    public void ChangeViewIndex( int indexIncrement ) {
        viewIndex += indexIncrement;
        if ( viewIndex < 0 ) {
            viewIndex = ChooseHandController.Instance.HandsList.Count - 1;
        }
        if ( viewIndex == ChooseHandController.Instance.HandsList.Count ) {
            viewIndex = 0;
        }
        var nextIndex = viewIndex + 1;

        if ( nextIndex == ChooseHandController.Instance.HandsList.Count ) {
            nextIndex = 0;
        }
        foreach ( Transform child in LayoutGroup.transform ) {
            Destroy( child.gameObject );
        }

        var activeHandIndex = PlayerPrefs.GetInt( "PlayerChoosedHand" );
        var handButton = Instantiate( ChooseHandButtonPrefab );
        var handStruct = ChooseHandController.Instance.HandsList[viewIndex];
        handButton.SetUpHandButton( handStruct.HandScissorsSprite, activeHandIndex == viewIndex, handStruct.ColourOfHand );

        var nextHandButton = Instantiate( ChooseHandButtonPrefab );
        var nextHandStruct = ChooseHandController.Instance.HandsList[nextIndex];
        nextHandButton.SetUpHandButton( nextHandStruct.HandScissorsSprite, activeHandIndex == nextIndex, nextHandStruct.ColourOfHand );

        handButton.transform.SetParent( LayoutGroup.transform, false );
        nextHandButton.transform.SetParent( LayoutGroup.transform, false );
        handButton.transform.localScale = nextHandButton.transform.localScale = Vector3.one;
        handButton.transform.localPosition = new Vector3( handButton.transform.localPosition.x, handButton.transform.localPosition.y, 0 );
        nextHandButton.transform.localPosition = new Vector3( nextHandButton.transform.localPosition.x, nextHandButton.transform.localPosition.y, 0 );
    }

    void ApplyOnClick() {
        PlayerPrefs.SetInt( "PlayerChoosedHand", (int) selectedHandSetting.ColourOfHand );
        currentHandSetting = selectedHandSetting;
        var choosenHand =
            ChooseHandController.Instance.HandsList.Find( h => h.ColourOfHand == selectedHandSetting.ColourOfHand );
        if ( playerMeshRendererHand != null ) {
            playerMeshRendererHand.material = choosenHand.HandMaterial;
        }
        gameObject.SetActive( false );
    }

    void Cancel_OnClick() {
        PlayerPrefs.SetInt( "PlayerChoosedHand", (int) currentHandSetting.ColourOfHand );
        gameObject.SetActive( false );
    }

    void OnDestroy() {
        Cancel_OnClick();
    }

}
