using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RowItem : MonoBehaviour
{
    private string itemName { get; set; }
    private int itemOdds { get; set; }

    void Awake()
    {
        gameObject.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
    }

    public string GetItemName() { return itemName; }
    public int GetitemOdds() { return itemOdds; }

    public void SetItemName(string iName)
    {
        itemName = iName;
    }

    public void SetitemOdds(int iOdds)
    {
        itemOdds = iOdds;
    }

    public void SetItemSprite(Sprite iSprite)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = iSprite;
    }
}
