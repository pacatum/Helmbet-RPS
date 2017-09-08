using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FilterItemView : MonoBehaviour, IPointerClickHandler {

    public event Action<FilterItemView> OnItemClick;

    public event Action OnFilterItemChanged;

    [SerializeField] Text selectChoiseText;
    [SerializeField] Text endRangeSelectChoise;
    [SerializeField] FilterItemExpandView expandFilterItemview;


    void Awake() {
        expandFilterItemview.OnSelectChoiseChange += SetItemViewTitle;
    }

    void SetItemViewTitle( string firstValue, string secondValue ) {
        SelectChoise = firstValue;
        EndRangeSelectChoise = secondValue;

        if ( OnFilterItemChanged != null ) {
            OnFilterItemChanged();
        }
    }

    public void ShowExpandItem() {
        gameObject.SetActive( false );
        expandFilterItemview.gameObject.SetActive( true );
    }

    public void ShowCloseItem() {
        gameObject.SetActive( true );
        expandFilterItemview.gameObject.SetActive( false );
    }

    void IPointerClickHandler.OnPointerClick( PointerEventData eventData ) {
        if ( OnItemClick != null ) {
            OnItemClick( this );
        }
    }


    public string SelectChoise {
        get { return selectChoiseText==null ? string.Empty : selectChoiseText.text; }
        set {
            if ( selectChoiseText != null ) {
                selectChoiseText.text = value;
            }
        }
    }

    public string EndRangeSelectChoise {
        get { return endRangeSelectChoise == null ? string.Empty : endRangeSelectChoise.text; }
        set {
            if (endRangeSelectChoise != null ) {
                endRangeSelectChoise.text = value;
            }
        }
    }

    public void RestoreFilterItem() {
        expandFilterItemview.RestoreItemValuesToDefault();
    }
}
