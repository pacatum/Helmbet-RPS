using System;
using System.Collections.Generic;
using UnityEngine;


public class ChooseHandController : SingletonMonoBehaviour<ChooseHandController> {

	public enum HandColour {

		Black,
		SuperBlack,
		NaziWhite,
		White,
		GentleWhite
	}
	
	
	[Serializable]
	public struct HandSetting {

		public HandColour ColourOfHand;
		public Sprite HandPalmSprite;
		public Sprite HandScissorsSprite;
		public Material HandMaterial;
	}
	
	
    public event Action<HandColour> OnChooseColor;

    public List<HandSetting> HandsList = new List<HandSetting>();

  
    void Start() {
        SetCurrentChoosedHand((HandColour)PlayerPrefs.GetInt("PlayerChoosedHand"));
    }

    public void SetCurrentChoosedHand( HandColour color ) {
        if ( OnChooseColor != null ) {
            OnChooseColor( color );
        }
    }
}