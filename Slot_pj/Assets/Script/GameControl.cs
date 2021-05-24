using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(CalResults))]
public class GameControl : MonoBehaviour
{
    public static event Func<int[]>      CombinationResult;
    public static event Func<int, int>   PrizeResult;

    public Text myMoneyText;
    public Text prizeText;
    public Row[] rows = new Row[3];
    public Transform handle;

    private CalResults calResults;
    private int myMoney = 1000;
    private int prizeValue;
    private bool handlePulled;

    private void Awake()
    {
        GameInitialize();
    }
    
    private void GameInitialize()
    {
        prizeText.enabled = false;
        prizeText.text = "Prize : " + 0;
        myMoneyText.enabled = true;
        myMoneyText.text = "My Money :" + myMoney;

        calResults = GetComponent<CalResults>();
        handlePulled = false;

        if (rows.Length != 3 || rows == null || calResults == null)
        {
            Debug.Log("<color=red> check GameControl are setup or not! </color>");
            return;
        }

        for(int i = 0; i < rows.Length; i++)
        {
            rows[i].ObjectInitialize();
        }

        calResults.ObjectInitialize(rows);
    }

    private void OnMouseDown()
    {
        if(IsAllRowStopeed() && !handlePulled && myMoney > 0)
        {
            StartCoroutine("PullHandle");
        }
    }

    private IEnumerator PullHandle()
    {
        handlePulled = true;
        myMoney -= 100;

        for (int i = 0; i < 30; i++)
        {
            handle.Rotate(0f, 0f, 1f, Space.Self);
            yield return 0;
        }

        for (int j = 0; j < 30; j++)
        {
            handle.Rotate(0f, 0f, -1f, Space.Self);
            yield return 0;
        }

        int[] aCombine = CombinationResult();

        if (aCombine != null)
        {
            Debug.Log("<color=cyan>開轉!</color>");
            rows[0].StartRotating(aCombine[0]);
            rows[1].StartRotating(aCombine[1]);
            rows[2].StartRotating(aCombine[2]);
            Debug.Log("<color=green>這次的組合 = : </color>" + aCombine[0] + aCombine[1] + aCombine[2]);
        }

        yield return StartCoroutine("CheckIsStopped");
    }

    private IEnumerator CheckIsStopped()
    {
        yield return new WaitForSeconds(0.1f);

        while (!IsAllRowStopeed())
        {
            prizeValue = 0;
            prizeText.enabled = false;
            myMoneyText.text = "My Money : " + myMoney;
            yield return new WaitForSeconds(0.1f);
        }

        if (IsAllRowStopeed())
        {
            prizeValue = PrizeResult(100);
            prizeText.enabled = true;
            prizeText.text = "Prize : " + prizeValue;
            myMoney += prizeValue;
            myMoneyText.text = "My Money : " + myMoney;

            handlePulled = false;
            StopCoroutine("CheckIsStopped");
        }
    }

    private bool IsAllRowStopeed()
    {
        if(!rows[0].IsRowStopped || !rows[1].IsRowStopped || !rows[0].IsRowStopped)
        {
            return false;
        }
        else if(rows[0].IsRowStopped && rows[1].IsRowStopped && rows[2].IsRowStopped)
        {
            return true;
        }
        return false;
    }
}
