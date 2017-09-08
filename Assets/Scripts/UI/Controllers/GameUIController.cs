using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class GameUIController : SingletonMonoBehaviour<GameUIController> {



	[SerializeField] GameHeaderView gameHeaderView;
	[SerializeField] GameSettingView settingsView;
	


	protected  override void Awake() {
		base.Awake();
		gameHeaderView.OnSettingsButton += Header_OnSettingsClick;
	}

	void Header_OnSettingsClick( ) {
	    if ( settingsView.gameObject.activeSelf ) {
	        settingsView.Hide();
	    } else {
	        settingsView.Show();
	    }
	}
    

}
