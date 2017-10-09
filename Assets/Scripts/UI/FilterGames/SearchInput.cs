using System;
using UnityEngine;
using UnityEngine.UI;

public class SearchInput : MonoBehaviour {

    public event Action OnValueChange;

    [SerializeField] SearchView searchInput;
    public string searchFilterText;
    InputField input;


    void Awake() {
        input = GetComponent<InputField>();
        searchInput.OnValueChange += Value_OnChange;
        GetComponentInParent<Button>().onClick.AddListener( searchInput.Show );
    }

    void Value_OnChange( string searchText ) {
        searchFilterText = searchText;
        input.text = searchFilterText;
        if ( OnValueChange != null ) {
            OnValueChange();
        }
    }
    
    public void ClearInput() {
        searchFilterText = string.Empty;
        if ( input != null ) {
            input.text = string.Empty;
        }
        searchInput.Clear();
    }

}
