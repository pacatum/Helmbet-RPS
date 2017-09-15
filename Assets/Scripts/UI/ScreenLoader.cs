using UnityEngine;


public class ScreenLoader : MonoBehaviour {

	[SerializeField] GameObject loader;
    [SerializeField] float timeoutTimer = 30;
	bool isLoading;
    float currentTimeoutTimer;

	public bool IsLoading {
		get { return isLoading; }
		set {
			isLoading = value;
		    currentTimeoutTimer = timeoutTimer;
            if ( isLoading ) {
                ShowLoader();
			} else {
                HideLoader();
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

	public void LoadScreen(bool isLoad) {
		IsLoading = isLoad;
	}

	 void ShowLoader() {
		 loader.SetActive( true );
	}

	 void HideLoader() {
		 loader.SetActive(false);
	}
}
