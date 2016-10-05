using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using System;


public class SkillTableParser : CSVReader
{
    public string fileName { set; get; }

    public SkillTableParser() { }
    public SkillTableParser(string fileName) { this.fileName = fileName; }
    
    public override void Load()
    {
        OpenFile(fileName);
    }

    public int[] PhaseSkillCounter()
    {        
        int totalPhase = Convert.ToInt32(stringList[colCnt * (rowCnt-1)]);  //총 페이즈 개수
        int curPhase = 1;
        int[] phases = new int[totalPhase]; // 각 페이즈에 사용되는 스킬들의 개수 저장

        //페이즈에 따른 개수 초기화
        for (int i = 0; i < totalPhase; i++)
            phases[i] = 0;

        int tempData;
        for (int row = 2; row < rowCnt; row++)
        {
            tempData = Convert.ToInt32(stringList[colCnt * row]);
            if (curPhase != tempData)
                curPhase++;

            phases[curPhase-1]++;    // 페이즈의 스킬 개수 저장
        }
        return phases;
    }

    public void ParseByID(ItemStruct data, int id)
    {
        for (int row = 2; row < rowCnt; row++)
        {
            if (!Convert.ToInt32(stringList[colCnt * row]).Equals(id))
                continue;

            data.SetData(
                Convert.ToInt32(stringList[colCnt * row]),
                Convert.ToString(stringList[colCnt * row + 1]),
                Convert.ToInt32(stringList[colCnt * row + 2]),
                Convert.ToInt32(stringList[colCnt * row + 3]));

            break;
        }
    }

    // 이름으로 검색, 반환
    public void ParseByName(ItemStruct data, string name)
    {
        for (int row = 2; row < rowCnt; row++)
        {
            if (!Convert.ToString(stringList[colCnt * row + 1]).Equals(name))
                continue;

            data.SetData(
                Convert.ToInt32(stringList[colCnt * row]),
                Convert.ToString(stringList[colCnt * row + 1]),
                Convert.ToInt32(stringList[colCnt * row + 2]),
                Convert.ToInt32(stringList[colCnt * row + 3]));

            break;
        }
    }

}
