using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    public bool     IsRowStopped       = true;
    public float    xOriginPosition;
    public float    yStartPosition     = 1.6f;
    public float    yCenterPosition    = 0.6f;
    public int      minMoveTimes       = 50;
    public int      maxMoveTimes       = 100;

    public  RowItem rowPrefab;
    public  RowData rowData;
    public  AnimationCurve speedCurve;

    [HideInInspector]
    public GameObject[] rowItems;
    private float       yEndPosition;
    private const float ITEM_INTERVAL = 1.0f;

    public void StartRotating(int iIndex)
    {
        StartCoroutine("RollByIndex", iIndex);
    }

    public void ObjectInitialize()
    {
        IsRowStopped = true;
        InstantiateRowObject(rowData);
        AlignRowItems();
    }

    private void InstantiateRowObject(RowData iData)
    {
        if (!iData)
        {
            Debug.Log("<color=red> No RowData to instance.</color>");
            return;
        }

        rowItems = new GameObject[iData.itemData.Length];

        for (int i = 0; i < iData.itemData.Length; i++)
        {
            rowItems[i] = Instantiate(rowPrefab.gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
            RowItem aItem = rowItems[i].GetComponent<RowItem>();
            string aName = iData.itemData[i].itemName;
            int aOdds = iData.itemData[i].itemOdds;
            Sprite aSprite = iData.itemData[i].itemSprite;

            rowItems[i].transform.SetParent(gameObject.transform, true);
            aItem.SetItemName(aName);
            aItem.SetitemOdds(aOdds);
            aItem.SetItemSprite(aSprite);
        }
    }

    private void AlignRowItems()
    {
        if (rowItems == null)
        {
            Debug.Log("<color=red>There's no Item in Row!!</color>");
            return;
        }

        var yPos = yStartPosition;

        for (int i = (rowItems.Length - 1); i >= 0; i--)
        {
            rowItems[i].transform.position = new Vector2(xOriginPosition, yPos);
            yPos -= ITEM_INTERVAL;
        }
        yEndPosition = yPos;
    }

    private IEnumerator RollByIndex(int iIndex)
    {
        if(iIndex > (rowItems.Length - 1) || iIndex < 0)
        {
            Debug.Log(gameObject.name + "<color=red> < - 指定Index錯誤</color>");
            IsRowStopped = true;
            StopCoroutine("RollByIndex");
        }

        minMoveTimes = minMoveTimes < rowItems.Length ? rowItems.Length : minMoveTimes; //最少一圈
        var aTotalMoveTimes = Random.Range(minMoveTimes, maxMoveTimes);
        var aMinMoveTimes = (iIndex - GetCurrentItemIndex()); //計算當前Item至指定索引Item的最小移動次數
        aMinMoveTimes = aMinMoveTimes >= 0 ? aMinMoveTimes : (aMinMoveTimes + rowItems.Length);
        
        if (aMinMoveTimes != (aTotalMoveTimes % rowItems.Length)) //最小移動次數必須 = 移動總次數 % RowItem總數
        {
            aTotalMoveTimes = aTotalMoveTimes - ((aTotalMoveTimes % rowItems.Length) - aMinMoveTimes);
        }

        yield return StartCoroutine("RollByTimes", aTotalMoveTimes);
    }

    private IEnumerator RollByTimes(int iTimes)
    {
        IsRowStopped = false;
 
        float   aSpeed = 1.0f;
        int     aTimes = 1;
        int     aTotalMoveTimes = iTimes + 1;

        while(aTimes < aTotalMoveTimes)
        {
            aSpeed = speedCurve.Evaluate((float)aTimes / aTotalMoveTimes);
            aSpeed = aSpeed <= 0.0f ? 1.0f : aSpeed;
            aTimes++;
            yield return StartCoroutine("RollByOnce", aSpeed);
        }

        IsRowStopped = true;
    }

    private IEnumerator RollByOnce(float iSpeed)
    {
        float[] yPositions = new float[rowItems.Length];

        for(int i = 0; i < yPositions.Length; i++)
        {
            yPositions[i] = rowItems[i].transform.position.y - ITEM_INTERVAL;
        }

        while(!IsAllItemInPosition(yPositions))
        {
            for (int i = 0; i < rowItems.Length; i++)
            {
                var aTargetPosition = Mathf.MoveTowards(rowItems[i].transform.position.y, yPositions[i], Time.deltaTime * iSpeed); //1倍速大概3秒。
                rowItems[i].transform.position = new Vector2(xOriginPosition, aTargetPosition);
            }
            yield return 0;
        }

        ResetIfOverEndPosition();
    }

    private void ResetIfOverEndPosition()
    {
        for (int i = 0; i < rowItems.Length; i++)
        {
            if (rowItems[i].transform.position.y <= yEndPosition)
            {
                rowItems[i].transform.position = new Vector2(xOriginPosition, yStartPosition);
            }
        }
    }

    private bool IsAllItemInPosition(float[] iPosition)
    {
        bool InPosition = true;

        for (int i = 0; i < rowItems.Length; i++)
        {
            if(Mathf.Approximately(rowItems[i].transform.position.y, iPosition[i]) == false)
            {
                InPosition = false;
            }
        }
        return InPosition;
    }

    private Transform FindCurrentItem()
    {
        Transform aItem = null;

        for (int i = 0; i < rowItems.Length; i++)
        {
            if (Mathf.Approximately(rowItems[i].transform.position.y , yCenterPosition) == true)
            {
                return aItem = rowItems[i].transform;
            }
        }
        return aItem;
    }

    private int GetCurrentItemIndex()
    {
        int aIndex = 0;

        for (int i = 0; i < rowItems.Length; i++)
        {
            if (Mathf.Approximately(rowItems[i].transform.position.y, yCenterPosition) == true)
            {
                return aIndex = i;
            }
        }
        return aIndex;
    }
}
