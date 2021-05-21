using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RowItem : MonoBehaviour
{
    private string itemName { get; set; }
    private int itemWeight { get; set; }

    void Awake()
    {
        gameObject.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
    }

    public string GetItemName() { return itemName; }
    public int GetItemWeight() { return itemWeight; }

    public void SetItemName(string iName)
    {
        itemName = iName;
    }

    public void SetItemWeight(int iWeight)
    {
        itemWeight = iWeight;
    }

    public void SetItemSprite(Sprite iSprite)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = iSprite;
    }
}
