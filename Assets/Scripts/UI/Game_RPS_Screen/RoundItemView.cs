using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Gesture = Base.Config.ChainTypes.RockPaperScissorsGesture;


public class RoundItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	[SerializeField] Color looseColor;
	[SerializeField] Color winColor;
	[SerializeField] Color tieColor;
	[SerializeField] float deselectAplhaColor;

	[SerializeField] RoundChoiseItemView normalState;
	[SerializeField] RoundChoiseItemView hoverState;
	[SerializeField] Text roundNumber;

	List<RoundChoiseItemView> itemViewStates = new List<RoundChoiseItemView>();


	void Awake() {
		itemViewStates.Add( normalState );
		itemViewStates.Add( hoverState );
	}

	public void OnPointerEnter( PointerEventData eventData ) {
		SetHoverView();
	}

	public void OnPointerExit( PointerEventData eventData ) {
		SetNormalView();
	}

	void SetNormalView() {
		var color = roundNumber.color;
		color.a = deselectAplhaColor;
		roundNumber.color = color;
		normalState.gameObject.SetActive( true );
		hoverState.gameObject.SetActive( false );
	}

	void SetHoverView() {
		var color = roundNumber.color;
		color.a = 1f;
		roundNumber.color = color;
		normalState.gameObject.SetActive( false );
		hoverState.gameObject.SetActive( true );
	}

	public void UpdateItem( int roundnumber, Gesture? anotherPlayerChoise, Gesture? thisPlayerChoise, GameResult state ) {
	    roundNumber.text = roundnumber.ToString();
		UpdateRoundNumberColor( state );
		foreach ( var item in itemViewStates ) {
			item.UpdateItem( anotherPlayerChoise, thisPlayerChoise, state );
		}
		SetNormalView();
	}

	void UpdateRoundNumberColor( GameResult state ) {
		switch ( state ) {
		case GameResult.Win:
			roundNumber.color = winColor;
			break;
		case GameResult.Lose:
			roundNumber.color = looseColor;
			break;
		case GameResult.Draw:
			roundNumber.color = tieColor;
			break;
		}
	}
}