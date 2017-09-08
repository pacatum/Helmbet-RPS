using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalanceItemView : MonoBehaviour {

    [SerializeField] Text balanceAmount;
    [SerializeField] Text balaneDescription;


    public string BalanceAmount {
        get { return balanceAmount == null ? string.Empty : balanceAmount.text; }
        set {
            if ( balanceAmount != null ) {
                balanceAmount.text = value;
            }
        }
    }

    public string BalanceDescription {
        get { return balaneDescription == null ? string.Empty : balaneDescription.text; }
        set {
            if ( balaneDescription != null ) {
                balaneDescription.text = value;
            }
        }
    }

}
