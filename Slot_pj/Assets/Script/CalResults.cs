using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalResults : MonoBehaviour
{
    private void Start()
    {
        GameControl.CheckResults += CalScore;
    }

    private void Update()
    {
        
    }

    private int CalScore()
    {
        return 0;
    }
}
