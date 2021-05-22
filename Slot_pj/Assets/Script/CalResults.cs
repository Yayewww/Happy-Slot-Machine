using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalResults : MonoBehaviour
{
    private int[] rowLength = new int[3];
    private Row[] rows = new Row[3];
    private List<int[]> basicCombinations;
    private List<int[]> combinationSamples;
    private int numberOfSamples;
    private const int TOTAL_ROW = 3;

    /// <summary>
    /// 做三種演算結果的方式
    /// 1. 分開看各軸權重，如 Rows[0]{(A)0.5, (B)0.3, (C)0.2}, Rows[1] {(A)0.1, (B)0.2, (C)0.7}......
    /// 2. 看單軸權重，但各軸權重會隨上一個結果改變。如Rows[0]{(A)0.5, (B)0.3, (C)0.2}，抽到(A)的話，降低權重增加另外兩者權重->Rows[1] {(A)0.3, (B)0.4, (C)0.3}
    /// 3. 排列組合上權重，如(A)(A)(A)、(B)(B)(B)，三者相同的組合依照Item的權重製造樣本。兩者相同或三者皆不同的樣本更多。
    /// </summary>
    /// <param name="iRows"></param>

    public void ObjectInitialize(Row[] iRows)
    {
        rows = iRows;
        rowLength[0] = rows[0].rowItems.Length;
        rowLength[1] = rows[1].rowItems.Length;
        rowLength[2] = rows[2].rowItems.Length;

        basicCombinations = new List<int[]>();
        combinationSamples = new List<int[]>();
        InstanceBasicCombinations();
        InstanceSamples();
        GameControl.CombinationResult += GiveResult;
    }

    void InstanceBasicCombinations()//製造基本互斥組合
    {
        for(int x = 0; x < rowLength[0]; x++)
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

        Debug.Log("基本互斥組合總數 : " + basicCombinations.Count);
    }

    void InstanceSamples()
    {
        //決定三者相同、兩者相同、三者不同的樣本數。機率先隨便給{0.1, 0.3, 0.6}
        int SamplesOfTriple = Mathf.RoundToInt(basicCombinations.Count * 0.5f);
        int SamplesOfDouble = Mathf.RoundToInt(basicCombinations.Count * 0.2f);
        int SampleOfSingle  = Mathf.RoundToInt(basicCombinations.Count * 0.3f);

        for (int i = 0; i < basicCombinations.Count; i++)//拉普拉斯平滑處理，製造基本互斥組合在樣本數各一，使其不等於0。
        {
            combinationSamples.Add(basicCombinations[i]);
        }

        //for (int j = 0; j < basicCombinations.Count; j++)//依照機率製造樣本數
        //{
        //    int x = basicCombinations[j][0];
        //    int y = basicCombinations[j][1];
        //    int z = basicCombinations[j][2];

        //    if (x == y && y == z)//三者相同
        //    {
        //        for (int t = 0; t < SamplesOfTriple; t++)
        //        {
        //            combinationSamples.Add(basicCombinations[j]);
        //        }
        //    }
        //    else if (x == y || y == z || x == z)//兩者相同
        //    {
        //        for (int d = 0; d < SamplesOfDouble; d++)
        //        {
        //            combinationSamples.Add(basicCombinations[j]);
        //        }
        //    }
        //    else
        //    {
        //        for (int s = 0; s < SampleOfSingle; s++)//三者不同
        //        {
        //            combinationSamples.Add(basicCombinations[j]);
        //        }
        //    }
        //}

        numberOfSamples = combinationSamples.Count;
        CheckCombinationsProbability();//Debug
        Debug.Log("樣本總數 : " + numberOfSamples);
    }

    void CheckCombinationsProbability()//Debug用，確認各組合在樣本數的占比
    {
        float aTriple = 0.0f;
        float aDouble = 0.0f;
        float aSingle = 0.0f;

        for(int i = 0; i < combinationSamples.Count; i++)
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
            else
            {
                aSingle++;
            }
        }

        Debug.Log("三者相同 : " + (aTriple / numberOfSamples));
        Debug.Log("二者相同 : " + (aDouble / numberOfSamples));
        Debug.Log("都不相同 : " + (aSingle / numberOfSamples));

    }

    int[] GiveResult()
    {
        if(combinationSamples.Count == 0)
        {
            Debug.Log("<color=red>樣本抽完了!!</color>");
            return null;
        }

        int[] aResult = { 0, 0, 0 };
        int aRand = Random.Range(0, (combinationSamples.Count - 1));

        if(combinationSamples[aRand] == null)//如果抽到的是已經移除的，重抽，抽到有。
        {
            aResult = GiveResult();
        }
        else
        {
            aResult = combinationSamples[aRand];
            combinationSamples.Remove(combinationSamples[aRand]);//抽到後移除此樣本
        }

        Debug.Log("樣本剩餘 : " + combinationSamples.Count);
        return aResult;
    }

    bool IsAlreadyInCombinations(int[] iRowCombine)//偵測是否已有相同組合
    {
        if(basicCombinations.Count == 0) { return false; }

        bool InCombine = false;

        basicCombinations.ForEach(indexCombine =>
        {
            if(IsAllSame(indexCombine, iRowCombine))
            {
                InCombine = true;
            }
        });

        return InCombine;
    }

    bool IsAllSame(int[] xCombine, int[] yCombine)
    {
        bool IsSame = true;

        for(int i = 0; i < TOTAL_ROW; i ++)
        {
            if(xCombine[i] != yCombine[i]) { return false; }
        }

        return IsSame;
    }

    void PrintSingleCombine(int[] iCombine)
    {
        string aCombine = "";

        for(int i = 0; i < iCombine.Length; i++)
        {
            aCombine += "/" + iCombine[i].ToString();
        }

        Debug.Log(aCombine);
    }

    void PrintAllCombine()
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
}
