using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalResults : MonoBehaviour
{
    public  int totalSamples = 100;
    public float ratioOfTriple = 0.5f;

    private int[] currentResult = { 0, 0, 0 };
    private int[] rowLength = new int[3];
    private Row[] rows = new Row[3];
    private List<int[]> basicCombinations;
    private List<int[]> combinationSamples;
    private int numberOfSamples;
    private int resultPrizeValue;
    private const int TOTAL_ROW = 3;

    public void ObjectInitialize(Row[] iRows)
    {
        rows = iRows;
        InstanceBasicCombinations();
        InstanceSamples();
        GameControl.CombinationResult += GiveResult;
        GameControl.PrizeResult += GivePrize;
    }

    void InstanceBasicCombinations()//製造基本互斥組合
    {
        basicCombinations = new List<int[]>();

        rowLength[0] = rows[0].rowItems.Length;
        rowLength[1] = rows[1].rowItems.Length;
        rowLength[2] = rows[2].rowItems.Length;

        for (int x = 0; x < rowLength[0]; x++)
        {
            for(int y = 0; y < rowLength[1]; y++)
            {
                for(int z = 0; z < rowLength[2]; z++)
                {
                    int[] aCombine = { x, y, z };
                    basicCombinations.Add(aCombine); 
                }
            }
        }
        
        //Debug
        Debug.Log("基本互斥組合總數 : " + basicCombinations.Count);
    }

    void InstanceSamples()
    {
        combinationSamples = new List<int[]>();

        for (int i = 0; i < basicCombinations.Count; i++)//拉普拉斯平滑處理(Laplace Smoothing)
        {
            combinationSamples.Add(basicCombinations[i]);
        }

        for (int j = 0; j < basicCombinations.Count; j++)//依照機率製造樣本數
        {
            int x = basicCombinations[j][0];
            int y = basicCombinations[j][1];
            int z = basicCombinations[j][2];
            int aSamplesOfTriple = Mathf.RoundToInt(totalSamples * ratioOfTriple);
            int aSamplesOfOthers = totalSamples - aSamplesOfTriple;

            if (x == y && y == z)//三者相同
            {
                int aSamples = Mathf.RoundToInt(aSamplesOfTriple / rowLength[0]);
                for (int t = 1; t < aSamples; t++)//迴圈從1開始，因為前面該組合已經+1了。
                {
                    combinationSamples.Add(basicCombinations[j]);
                }
            }
            else //其他(兩者相同, 三者不同)
            {
                int aSamples = Mathf.RoundToInt(aSamplesOfOthers * (1.0f / basicCombinations.Count));
                for (int o = 1; o < aSamples; o++)
                {
                    combinationSamples.Add(basicCombinations[j]);
                }
            }
        }

        numberOfSamples = combinationSamples.Count;

        //Debug
        CheckCombinationsProbability();
        Debug.Log("樣本總數 : " + numberOfSamples);
    }

    int[] GiveResult()//從樣本數裡面抽一個組合結果
    {
        if(combinationSamples.Count == 0)
        {
            Debug.Log("<color=red>樣本抽完了，重新生成樣本!!</color>");
            InstanceSamples();
        }

        int aRand = Random.Range(0, (combinationSamples.Count - 1));

        if(combinationSamples[aRand] == null)//如果抽到的是已經移除的，重抽，抽到有。
        {
            currentResult = GiveResult();
        }
        else
        {
            currentResult = combinationSamples[aRand];
            combinationSamples.Remove(combinationSamples[aRand]);//抽到後移除此樣本
        }

        Debug.Log("樣本剩餘 : " + combinationSamples.Count);

        return currentResult;
    }

    int GivePrize(int iBet)
    {
        int x = currentResult[0];
        int y = currentResult[1];
        int z = currentResult[2];

        if (x == y && y == z)//三者相同
        {
            resultPrizeValue = iBet * rows[0].rowData.itemData[x].itemOdds;
        }
        else
        {
            resultPrizeValue = 0;
        }

        return resultPrizeValue;
    }

    #region DEBUG用

    void PrintSingleCombine(int[] iCombine)//列出單一組合
    {
        string aCombine = "";

        for(int i = 0; i < iCombine.Length; i++)
        {
            aCombine += "/" + iCombine[i].ToString();
        }

        Debug.Log(aCombine);
    }

    void PrintAllCombine()//列出所有基本互斥組合
    {
        basicCombinations.ForEach(indexCombine => 
        {
            string aCombine = "";

            for(int i = 0; i < indexCombine.Length; i++)
            {
                aCombine += "/" + indexCombine[i].ToString();
            }

            Debug.Log(aCombine);
        });
    }

    void CheckCombinationsProbability()//確認各組合在樣本數的占比
    {
        float aTriple = 0.0f;
        float aDouble = 0.0f;
        float aSingle = 0.0f;

        for (int i = 0; i < combinationSamples.Count; i++)
        {
            int x = combinationSamples[i][0];
            int y = combinationSamples[i][1];
            int z = combinationSamples[i][2];

            if (x == y && y == z)//三者相同
            {
                aTriple++;
            }
            else if (x == y || y == z || x == z)//兩者相同
            {
                aDouble++;
            }
            else if (x != y && y != z && x != z)
            {
                aSingle++;
            }
        }

        Debug.Log("三者相同 : " + (aTriple / numberOfSamples));
        Debug.Log("二者相同 : " + (aDouble / numberOfSamples));
        Debug.Log("都不相同 : " + (aSingle / numberOfSamples));
    }
    #endregion
}
