using UnityEngine;
using UnityEngine.EventSystems;

public enum ButtonViewState {

    Normal,
    Alternative,
    Hover, 
    Pressed

}

public class UIButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] GameObject hoverButtonItem;
    [SerializeField] GameObject normalButtonItem;
    [SerializeField] GameObject alternativeItem;

    GameObject currentNormalState;


    public void OnPointerEnter( PointerEventData eventData ) {
       UpdateColor( ButtonViewState.Hover );
    }

    public void OnPointerExit( PointerEventData eventData ) {
        hoverButtonItem.SetActive( false );
        alternativeItem.SetActive( true );
    }

    public void SetNormalState() {
        normalButtonItem.SetActive( true );
        alternativeItem.SetActive( false );
    }

    public void SetAlternativeState() {
        normalButtonItem.SetActive( false );
        alternativeItem.SetActive( true );
    }

    void SetHoverState() {
        hoverButtonItem.SetActive(true);
        alternativeItem.SetActive(false);
    }

    public void UpdateColor( ButtonViewState state ) {
        switch ( state ) {
            case ButtonViewState.Alternative:
                SetAlternativeState();
                break;
            case ButtonViewState.Normal:
                SetNormalState();
                break;
            case ButtonViewState.Hover:
                SetHoverState();
                break;
        }
    }

}
