using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortingArrow : MonoBehaviour {

	[SerializeField] Sprite ascendingSprite;
    [SerializeField] Sprite descendingSprite;

    [SerializeField] Image targetGraphics;

    public void SetArrowSprite( SortOrder order ) {
        targetGraphics.sprite = order.Equals( SortOrder.Ascending ) ? ascendingSprite : descendingSprite;
    }
}
