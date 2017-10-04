using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreateNewButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] Image hoverImage;
    [SerializeField] float normalImageScale;
    [SerializeField] float hoverImageScale;
    [SerializeField] float animationTimer;
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField] Color normalTextColor;
    [SerializeField] Color hoverTextColor;

    bool isHover;
    bool stopUpdating;


    public void OnPointerEnter( PointerEventData eventData ) {
        isHover = true;
        buttonText.color = hoverTextColor;
        stopUpdating = true;
    }

    public void OnPointerExit( PointerEventData eventData ) {
        isHover = false;
        stopUpdating = true;
    }

    void Update() {
        if ( !stopUpdating ) {
            return;
        }
        if ( isHover ) {
            if ( hoverImage.transform.localScale.y < hoverImageScale ) {
                hoverImage.transform.localScale = new Vector3( hoverImage.transform.localScale.x + animationTimer, hoverImage.transform.localScale.y + animationTimer, 1 );
            } else {
                hoverImage.transform.localScale = new Vector3( hoverImageScale, hoverImageScale, 1 );
                stopUpdating = false;
            }
        } else {
            if ( hoverImage.transform.localScale.y > normalImageScale ) {
                hoverImage.transform.localScale = new Vector3( hoverImage.transform.localScale.x - animationTimer, hoverImage.transform.localScale.y - animationTimer, 1 );
            } else {
                hoverImage.transform.localScale = new Vector3( normalImageScale, normalImageScale, 1 );
                buttonText.color = normalTextColor;
                stopUpdating = false;
            }
        }
    }

}
