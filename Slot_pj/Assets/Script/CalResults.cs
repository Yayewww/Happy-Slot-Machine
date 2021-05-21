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

    //池 : 每個元素機率加起來 = 1;
    //水位 : 賠率達n%
    //各軸元素權重計算不同，營造好像贏但不會贏的結果。
    //依照水位變化權重
}
