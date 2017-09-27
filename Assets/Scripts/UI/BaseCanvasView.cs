using System.Collections;
using UnityEngine;

public enum FilterType {

    Participate,
    Game,
    Players,
    Currency, 
    BuyIn

}

public abstract class BaseCanvasView : MonoBehaviour {

    [SerializeField] float screenFaderTimer = 1f;

    private CanvasGroup canvasGroup;

    private CanvasGroup currenCanvasGroup {
        get { return canvasGroup == null ? canvasGroup = GetComponent<CanvasGroup>() : canvasGroup; }
    }


    public virtual void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual IEnumerator Init() {
        yield break;
    }



    public virtual void Show() {

        gameObject.SetActive( true );
        StopCoroutine( "FadeScreen" );
        StartCoroutine( FadeScreen(currenCanvasGroup.alpha, 1f ) );
    }

    public virtual void Hide() {
        if ( gameObject.activeSelf && gameObject.activeInHierarchy) {
            StopCoroutine( "FadeScreen" );
            StartCoroutine( FadeScreen(currenCanvasGroup.alpha, 0f ) );
        }
    }

    private IEnumerator FadeScreen( float fadeFrom, float fadeTo ) {
        float currentTime = 0f;
        while (currenCanvasGroup.alpha != fadeTo ) {
            currenCanvasGroup.alpha = Mathf.Lerp( fadeFrom, fadeTo, currentTime * screenFaderTimer );
            currentTime += Time.deltaTime;
            yield return null;
        }
        if ( fadeTo == 0f ) {
            gameObject.SetActive( false );
        }
    }

}
