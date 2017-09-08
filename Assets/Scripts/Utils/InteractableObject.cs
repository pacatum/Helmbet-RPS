using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public event Action<CursorState> OnCursorChange;


	void Awake() {
		OnCursorChange += UIController.Instance.SwitchCursorState;
	}

	void Cursor_OnChange( CursorState state ) {
		if ( OnCursorChange != null ) {
			OnCursorChange( state );
		}
	}

	public void OnPointerEnter( PointerEventData eventData ) {
		Cursor_OnChange( CursorState.Hover );
	}

	public void OnPointerExit( PointerEventData eventData ) {
		Cursor_OnChange( CursorState.Normal );
	}


}
