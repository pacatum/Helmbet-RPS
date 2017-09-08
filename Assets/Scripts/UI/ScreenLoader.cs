using System;
using System.Collections;
using UnityEngine;

public class ScreenLoader : MonoBehaviour {

	//public event Action OnLoad;

	[SerializeField] GameObject loader;
	bool isLoading;

	public bool IsLoading {
		get { return isLoading; }
		set {
			isLoading = value;
			if( isLoading ) {
				ShowLoader();
			} else {
				HideLoader();
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
