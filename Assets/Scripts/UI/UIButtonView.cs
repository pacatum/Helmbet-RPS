using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] GameObject hoverButtonItem;
    [SerializeField] GameObject normalButtonItem;
    [SerializeField] GameObject alternativeItem;

    GameObject currentNormalState;

    public void OnPointerEnter( PointerEventData eventData ) {
        hoverButtonItem.SetActive( true );
        alternativeItem.SetActive( false );
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
        normalButtonItem.SetActive(false);
        alternativeItem.SetActive(true);
    }
}
