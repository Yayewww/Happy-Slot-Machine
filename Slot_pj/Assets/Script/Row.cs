using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    public bool     IsRowStopped       = true;
    public string   stoppedSlot;
    public float    xOriginPosition;
    public float    yStartPosition     = 1.6f;
    public float    yCenterPosition    = 0.6f;
    public float    minRollDuration    = 4.0f;
    public float    maxRollDuration    = 6.0f;
    public int      assignTimes        = 5;
    public int      assignIndex        = 0;

    public  RowItem rowPrefab;
    public  RowData rowData;

    public GameObject[] rowItems;
    private float   yEndPosition;
    //公式 : MOVE_INTERVAL * MOVE_TIMES = ITEM_INTERVAL <-實際測量各圖片間距是 "1"。
    private const float ITEM_INTERVAL = 1.0f;
    private const float MOVE_INTERVAL = 0.25f;
    private const float MOVE_TIMES    = 4.0f;


    private void Start()
    {
        IsRowStopped = true;
        InstantiateRowObject(rowData);
        AlignRowItems();
        GameControl.HandlePulled += StartRotating;
    }

    private void StartRotating()
    {
        stoppedSlot = "";
        StartCoroutine("AssignIndexToRoll", assignIndex);
    }

    private IEnumerator BasicRandomRoll()
    {
        IsRowStopped = false;
        var aTimeInterval = 0.025f;
        var aDuration = Random.Range(minRollDuration, maxRollDuration);
        var aRollTimes = Mathf.RoundToInt(aDuration / aTimeInterval);
        var aMoveInterval = MOVE_INTERVAL;

        if((aRollTimes % MOVE_TIMES) != 0.0f)//使總RollTimes是MOVE_TIMES的倍數。
        {
            var aRemain = aRollTimes % MOVE_TIMES;
            aRollTimes += (int)(MOVE_TIMES - aRemain);
        }

        var aTimes = 0;
        while(aTimes < aRollTimes)
        {
            for (int i = 0; i < rowItems.Length; i++)
            {
                rowItems[i].transform.position = new Vector2(xOriginPosition, rowItems[i].transform.position.y - aMoveInterval);
                ResetIfOverEndPosition();
            }

            if (aTimes > Mathf.RoundToInt(aRollTimes * 0.25f)) { aTimeInterval = 0.0125f; }
            if (aTimes > Mathf.RoundToInt(aRollTimes * 0.5f)) { aTimeInterval = 0.01f; }
            if (aTimes > Mathf.RoundToInt(aRollTimes * 0.75f)) { aTimeInterval = 0.05f; }
            if (aTimes > Mathf.RoundToInt(aRollTimes * 0.95f)) { aTimeInterval = 0.1f; }

            aTimes++;
            yield return new WaitForSeconds(aTimeInterval);
        }

        IsRowStopped = true;
    }

    private IEnumerator AssignIndexToRoll(int iIndex)
    {
        IsRowStopped = false;
        if(iIndex > (rowItems.Length - 1) || iIndex < 0)
        {
            iIndex = 0;
            Debug.Log("指定Index不能大於 : " + (rowItems.Length - 1));
        }

        float aSpeed = 8.0f;
        var aDuration = Random.Range(minRollDuration, maxRollDuration);
        var aTotalTimes = Mathf.RoundToInt(aDuration / 0.025f);

        //計算目標最小移動次數
        var aMinMoveTimes = (iIndex - GetCurrentItemIndex());
        aMinMoveTimes = aMinMoveTimes >= 0 ? aMinMoveTimes : (aMinMoveTimes + rowItems.Length);
        //計算最小移動數 = 移動總數%元素總數
        if(aMinMoveTimes != (aTotalTimes % rowItems.Length))
        {
            aTotalTimes = aTotalTimes - ((aTotalTimes % rowItems.Length) - aMinMoveTimes);
        }


        int aTimes = 0;
        while (aTimes < aTotalTimes)
        {
            if (aTimes > Mathf.RoundToInt(aTotalTimes * 0.1f)) { aSpeed = 10.0f; }
            if (aTimes > Mathf.RoundToInt(aTotalTimes * 0.25f)) { aSpeed = 6.0f; }
            if (aTimes > Mathf.RoundToInt(aTotalTimes * 0.75f)) { aSpeed = 3.0f; }
            if (aTimes > Mathf.RoundToInt(aTotalTimes * 0.95f)) { aSpeed = 2.0f; }

            aTimes++;
            yield return StartCoroutine("OnceRoll", aSpeed);
        }

        IsRowStopped = true;

    }

    private IEnumerator AssignTimesToRoll(int iTimes)
    {
        IsRowStopped = false;
 
        float   aSpeed = 1.0f;
        int     aTimes = 0;
        int     aTotalTimes = iTimes;

        while(aTimes < aTotalTimes)
        {
            if (aTimes > Mathf.RoundToInt(aTotalTimes * 0.1f)) { aSpeed = 2.0f; }
            if (aTimes > Mathf.RoundToInt(aTotalTimes * 0.25f)) { aSpeed = 4.0f; }
            if (aTimes > Mathf.RoundToInt(aTotalTimes * 0.75f)) { aSpeed = 2.0f; }
            if (aTimes > Mathf.RoundToInt(aTotalTimes * 0.95f)) { aSpeed = 1.0f; }

            aTimes++;
            yield return StartCoroutine("OnceRoll", aSpeed);
        }

        IsRowStopped = true;
    }

    private IEnumerator OnceRoll(float iSpeed)
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
                var aTargetPosition = Mathf.MoveTowards(rowItems[i].transform.position.y, yPositions[i], Time.deltaTime * iSpeed);
                rowItems[i].transform.position = new Vector2(xOriginPosition, aTargetPosition);
            }
            yield return 0;
        }

        ResetIfOverEndPosition();
    }

    bool IsAllItemInPosition(float[] iPosition)
    {
        bool InPosition = true;

        for (int i = 0; i < rowItems.Length; i++)
        {
            //if (rowItems[i].transform.position.y != iPosition[i]) { InPosition = false; }
            if(Mathf.Approximately(rowItems[i].transform.position.y, iPosition[i]) == false)
            {
                InPosition = false;
            }
        }
        return InPosition;
    }

    Transform FindCurrentItem()
    {
        Transform aItem = null;

        for (int i = 0; i < rowItems.Length; i++)
        {
            if (rowItems[i].transform.position.y == yCenterPosition)
            {
                aItem = rowItems[i].transform;
            }
        }
        return aItem;
    }

    int GetCurrentItemIndex()
    {
        int aIndex = 0;

        for (int i = 0; i < rowItems.Length; i++)
        {
            if (rowItems[i].transform.position.y == yCenterPosition)
            {
                aIndex = i;
            }
        }
        return aIndex;
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

    private void AlignRowItems()
    {
        if (rowItems == null)
        {
            Debug.Log("<color=red>There's no Item in Row!!</color>");
            return;
        }

        var yPos = yStartPosition;

        for(int i = (rowItems.Length - 1); i >= 0; i--)
        {
            rowItems[i].transform.position = new Vector2(xOriginPosition, yPos);
            yPos -= ITEM_INTERVAL;
        }
        yEndPosition = yPos;
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
            rowItems[i]           = Instantiate(rowPrefab.gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
            RowItem         aItem = rowItems[i].GetComponent<RowItem>();
            string          aName = iData.itemData[i].itemName;
            int           aWeight = iData.itemData[i].itemWeight;
            Sprite        aSprite = iData.itemData[i].itemSprite;

            rowItems[i].transform.SetParent(gameObject.transform, true);
            aItem.SetItemName(aName);
            aItem.SetItemWeight(aWeight);
            aItem.SetItemSprite(aSprite);
        }
    }

    private void OnDestroy()
    {
        GameControl.HandlePulled -= StartRotating;
    }
}
