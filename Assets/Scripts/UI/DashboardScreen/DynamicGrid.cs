using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGrid : MonoBehaviour {

    [SerializeField] int maxColumnCount = 6;
    [SerializeField] int minColumnCount = 3;

    [SerializeField] float maxCellWidth;
    [SerializeField] float minCellWidth;

    [SerializeField] float cellSpacing;

    [SerializeField] RectTransform tournamentItemViewPrefab;
    [SerializeField] RectTransform gridRectTransform;
    [SerializeField] GridLayoutGroup gridLayoutGroup;
    [SerializeField] RectTransform maxSizeOfPage;

    int maxItemsOnPage;
    int numberOfPages;

    int currentColums;
    int currentRows;


    void Awake() {
        ResizeGrid();
    }

    void OnRectTransformDimensionsChange() {
        ResizeGrid();
    }

    public void ResizeGrid() {
        var columsWithMinWidth = (int) ( ( gridRectTransform.rect.width - cellSpacing ) / ( minCellWidth + cellSpacing ) );
        var columsWithMaxnWidth = (int) ( ( gridRectTransform.rect.width - cellSpacing ) / ( maxCellWidth + cellSpacing ) );

        var restWithMinCellSize = gridRectTransform.rect.width - cellSpacing - minCellWidth * columsWithMinWidth;
        var restWithMaxCellSize = gridRectTransform.rect.width - cellSpacing - maxCellWidth * columsWithMaxnWidth;

        if ( restWithMinCellSize < restWithMaxCellSize ) {
            var preferSize = ( gridRectTransform.rect.width - cellSpacing * ( columsWithMinWidth + 1 ) ) / columsWithMinWidth;
            gridLayoutGroup.cellSize = new Vector2( Mathf.Min( preferSize, maxCellWidth ), gridLayoutGroup.cellSize.y );
            gridLayoutGroup.constraintCount = Mathf.Min( maxColumnCount, columsWithMinWidth );
            gridLayoutGroup.childAlignment = gridRectTransform.childCount < columsWithMinWidth ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter;
            currentColums = Mathf.Min( maxColumnCount, columsWithMinWidth );
        } else {
            var preferSize = ( gridRectTransform.rect.width - cellSpacing * ( columsWithMaxnWidth + 1 ) ) / columsWithMaxnWidth;
            gridLayoutGroup.cellSize = new Vector2( Mathf.Min( preferSize, minCellWidth ), gridLayoutGroup.cellSize.y );
            gridLayoutGroup.constraintCount = Mathf.Min( maxColumnCount, columsWithMaxnWidth );
            gridLayoutGroup.childAlignment = gridRectTransform.childCount < columsWithMaxnWidth ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter;
            currentColums = Mathf.Min( maxColumnCount, columsWithMaxnWidth );
        }
        currentRows = (int) ( ( maxSizeOfPage.rect.height - cellSpacing ) / ( gridLayoutGroup.cellSize.y + cellSpacing ) );
    }

    public int ColumnCount {
        get { return currentColums; }
    }

    public int RowCount {
        get { return currentRows; }
    }

}
