using System.Collections;
using UnityEngine;


public class ScreenLoader : MonoBehaviour {

	[SerializeField] GameObject loader;
    [SerializeField] float timeoutTimer = 30;
	bool isLoading;
    float currentTimeoutTimer;

	public bool IsLoading {
		get { return isLoading; }
		set {
		    if ( value && isLoading ) {
		        return;
		    } else {
                isLoading = value;
		        currentTimeoutTimer = timeoutTimer;
		        if ( isLoading ) {
		            ShowLoader();
		            StartCoroutine( StopLoader() );
		        } else {
		            HideLoader();
		        }
		    }
		}
	}
    

    void Update() {
        if ( isLoading ) {
            currentTimeoutTimer -= Time.deltaTime;
            if (currentTimeoutTimer <= 0 ) {
                IsLoading = false;
                currentTimeoutTimer = timeoutTimer;
            }
        }
    }

    IEnumerator StopLoader() {
        yield return new WaitForSecondsRealtime( timeoutTimer );
        IsLoading = false;
    }

	public void LoadScreen(bool isLoad) {
		IsLoading = isLoad;
	}

	 void ShowLoader() {
		 loader.SetActive( true );
	}

	 void HideLoader() {
        StopCoroutine( StopLoader() );
		loader.SetActive(false);
	}
}
