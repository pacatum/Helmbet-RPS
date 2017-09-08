using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterItemExpandView : MonoBehaviour {

    public event Action<string, string> OnSelectChoiseChange;
    string currentChoise;

    [SerializeField] Text selectChoiseText;
    [SerializeField] Text endRangeSelectChoise;
    [SerializeField] List<FilterChoiseItemView> items = new List<FilterChoiseItemView>();
    [SerializeField] FilterChoiseInputView firstInput;
    [SerializeField] FilterChoiseInputView secondInput;
    [SerializeField] Button closeItemViewButton;

    [SerializeField] int firstInputDefaultValue;
    [SerializeField] int secondInputDefaultValue;
    [SerializeField] int indexDefaultValue;


    void Awake() {
        RestoreItemValuesToDefault();
        closeItemViewButton.onClick.AddListener( HideExpandView );

        foreach ( var item in items ) {
            item.OnValueChange += SwitchFilterItems;
        }

        if ( firstInput != null ) {
            firstInput.OnValueChange += InputsOnChanged;
        }
        if ( secondInput != null ) {
            secondInput.OnValueChange += InputsOnChanged;
        }
        RestoreItemValuesToDefault();
    }

    void HideExpandView() {
        FilterGamesController.Instance.SwitchItemView( null );
    }

    public string CurrentChoise {
        get { return currentChoise; }
        set { currentChoise = value; }
    }

    void SwitchFilterItems( FilterChoiseItemView target ) {
        foreach ( var item in items ) {
            if ( item.Equals( target ) ) {
                item.CurrentState = FilterItemState.Select;
                Choise_OnChanged( item.SelectChoise );
                currentChoise = item.SelectChoise;
            } else {
                item.CurrentState = FilterItemState.Normal;
            }
        }
    }

    void Choise_OnChanged( string firstValue, string secondValue = "" ) {
        if ( OnSelectChoiseChange != null ) {
            OnSelectChoiseChange( firstValue, secondValue );
            SelectChoise = firstValue;
            EndRangeSelectChoise = secondValue;
        }
    }

    void InputsOnChanged() {
        string firstValue = "";
        string secondValue = "";

        if (firstInput != null && secondInput != null && Int32.Parse( firstInput.InputText ) > Int32.Parse( secondInput.InputText ) ) {
            firstInput.SetError();
            secondInput.SetError();
            FilterGamesController.Instance.SetApplyButton( false );
            return;
        }

        FilterGamesController.Instance.SetApplyButton( true );

        if ( firstInput != null ) {
            firstValue = firstInput.InputText;
        firstInput.SetNormal();
        }
        if ( secondInput != null ) {
            secondValue = secondInput.InputText;
        secondInput.SetNormal();
        }

        Choise_OnChanged( firstValue, secondValue );
    }

    public string SelectChoise {
        get { return selectChoiseText ? string.Empty : selectChoiseText.text; }
        set {
            if ( selectChoiseText != null ) {
                selectChoiseText.text = value;
            }
        }
    }

    public string EndRangeSelectChoise {
        get { return endRangeSelectChoise == null ? string.Empty : endRangeSelectChoise.text; }
        set {
            if ( endRangeSelectChoise != null ) {
                endRangeSelectChoise.text = value;
            }
        }
    }


    public void RestoreItemValuesToDefault() {
        FilterGamesController.Instance.SetApplyButton( true );


        if ( items.Count > 0 ) {
            items[indexDefaultValue].OnChoosen();
        } else {
            if ( firstInput != null ) {
                firstInput.SetNormal();
                firstInput.ChangeValue( firstInputDefaultValue.ToString() );
                SelectChoise = firstInputDefaultValue.ToString();
            }
            if ( secondInput != null ) {
                secondInput.SetNormal();
                secondInput.ChangeValue( secondInputDefaultValue.ToString() );
                EndRangeSelectChoise = secondInputDefaultValue.ToString();
            }

            Choise_OnChanged( firstInputDefaultValue.ToString(), secondInputDefaultValue.ToString() );
        }
        //InputsOnChanged();

    }

}
