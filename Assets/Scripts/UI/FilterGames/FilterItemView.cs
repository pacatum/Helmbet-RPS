using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FilterItemView : MonoBehaviour, IPointerClickHandler {

    public event Action<FilterItemView> OnItemClick;
    public event Action OnFilterItemChanged;

    [SerializeField] TextMeshProUGUI selectChoiseText;
    [SerializeField] TextMeshProUGUI endRangeSelectChoise;
    [SerializeField] FilterItemExpandView expandFilterItemview;
    [SerializeField] GameObject line;


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
        expandFilterItemview.ShowExpandView();
    }

    public void ShowCloseItem() {
        gameObject.SetActive( true );
        expandFilterItemview.gameObject.SetActive( false );
    }

    public void ShowDivider(bool isActive) {
        line.SetActive( isActive );
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
