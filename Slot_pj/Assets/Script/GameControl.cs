using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameControl : MonoBehaviour
{

    public static event Action HandlePulled = delegate { };
    public static event Func<int> CheckResults;

    [SerializeField]
    private Text prizeText;

    [SerializeField]
    private Row[] rows;

    [SerializeField]
    private Transform handle;
    private int prizeValue;
    private bool IsResultsChecked = false;

    private void Start()
    {
        
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
            prizeValue = CheckResults();
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

        HandlePulled();

        for(int i = 0; i < 15; i += 5)
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
