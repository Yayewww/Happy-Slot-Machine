using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RowData", menuName = "ScriptableObjects/RowData", order = 2)]
public class RowData : ScriptableObject
{
    public ItemData[] itemData;
}
