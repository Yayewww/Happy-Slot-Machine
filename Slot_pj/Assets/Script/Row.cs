using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    public bool IsRowStopped = true;
    public string stoppedSlot;
    public float yStartPosition;
    public float yEndPosition;
    public int minSpinRange = 60;
    public int maxSpinRange = 100;

    private int randomValue;
    private float timeInterval;

    private void Start()
    {
        IsRowStopped = true;
        GameControl.HandlePulled += StartRotating;
    }

    private void StartRotating()
    {
        stoppedSlot = "";
        StartCoroutine("Rotate");
    }

    private IEnumerator Rotate()
    {
        IsRowStopped = false;
        timeInterval = 0.025f;

        for(int i = 0; i < 30; i++)//Step 1 Rotating Animation
        {
            if(IsRowToEnd())
            {
                SetToStartPosition();
            }
            transform.position = new Vector2(transform.position.x, transform.position.y - 0.25f);

            yield return new WaitForSeconds(timeInterval);
        }

        randomValue = Random.Range(minSpinRange, maxSpinRange);

        switch(randomValue % 3)
        {
            case 1:
                randomValue += 2;
                break;
            case 2:
                randomValue += 1;
                break;
        }

        for(int i = 0; i < randomValue; i++)//Step 2 Rotating To Position
        {
            if(IsRowToEnd())
            {
                SetToStartPosition();
            }
            transform.position = new Vector2(transform.position.x, transform.position.y - 0.25f);

            //if (i > Mathf.RoundToInt(randomValue * 0.25f)) { timeInterval = 0.05f; }
            //if (i > Mathf.RoundToInt(randomValue * 0.5f)) { timeInterval = 0.1f; }
            //if (i > Mathf.RoundToInt(randomValue * 0.75f)) { timeInterval = 0.15f; }
            //if (i > Mathf.RoundToInt(randomValue * 0.95f)) { timeInterval = 0.2f; }

            yield return new WaitForSeconds(timeInterval);
        }

        IsRowStopped = true;
    }

    private bool IsRowToEnd()
    {
        bool    IsEnd = transform.position.y <= yEndPosition ? true : false;
        return  IsEnd;
    }

    private void SetToStartPosition()
    {
        transform.position = new Vector2(transform.position.x, yStartPosition);
    }

    private void OnDestroy()
    {
        GameControl.HandlePulled -= StartRotating;
    }
}
