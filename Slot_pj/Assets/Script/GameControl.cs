using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameControl : MonoBehaviour
{

    public static event Action HandlePulled = delegate { };
    public static event Func<int[]> CombinationResult;

    [SerializeField]
    private Text prizeText;

    [SerializeField]
    private Row[] rows;

    [SerializeField]
    private CalResults calResults;

    [SerializeField]
    private Transform handle;
    private int prizeValue;
    private bool IsResultsChecked = false;

    private void Awake()
    {
        GameInitialize();
    }
    
    private void GameInitialize()
    {
        if(rows.Length != 3 || rows == null || calResults == null)
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

    private void Update()//優化：用Coroutine去測好了沒
    {
        if(!IsAllRowStopeed())
        {
            prizeValue = 0;
            prizeText.enabled = false;
            IsResultsChecked = false;
            Debug.Log("轉ing");
        }

        if (IsAllRowStopeed() && !IsResultsChecked)
        {
            IsResultsChecked = true;
            prizeValue = 0;
            prizeText.enabled = true;
            prizeText.text = "Prize : " + prizeValue;
            Debug.Log("停了，開獎!");
        }
    }

    private void OnMouseDown()
    {
        if(IsAllRowStopeed())
        {
            StartCoroutine("PullHandle");
            Debug.Log("開轉!");
        }
        else
        {
            Debug.Log("還沒停不給轉!");
        }
    }

    private IEnumerator PullHandle()
    {
        for(int i = 0; i < 15; i += 5)
        {
            handle.Rotate(0f, 0f, i);
            yield return new WaitForSeconds(0.1f);
        }

        //HandlePulled();
        int[] aCombine = CombinationResult();

        if(aCombine != null)
        {
            rows[0].StartRotating(aCombine[0]);
            rows[1].StartRotating(aCombine[1]);
            rows[2].StartRotating(aCombine[2]);
        }

        for (int i = 0; i < 15; i += 5)
        {
            handle.Rotate(0f, 0f, -i);
            yield return new WaitForSeconds(0.1f);
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
