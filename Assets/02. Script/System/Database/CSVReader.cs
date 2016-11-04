using System;
using System.Text;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/*************************   정보   **************************

    CSV파일을 읽도록 하는 부모 클래스
    제어는 CSVParser 에서 한다

*************************************************************/

public abstract class CSVReader
{
    public CSVReader() { }
    ~CSVReader() { stringList = null; }

    protected string[] stringList = null;
    protected int colCnt;
    protected int rowCnt;
    protected int startIdx;

    public abstract void Load();

    // 파일 오픈
    public void OpenFile(string fileName)
    {
        string filePath = Application.streamingAssetsPath + "/"+ fileName + ".csv";
        StreamReader txtFile = new StreamReader(filePath);
        string fileFullPath = txtFile.ReadToEnd();
        
        txtFile.Close();
        fileFullPath = fileFullPath.Replace("\r\n", ",");
        stringList = fileFullPath.Split(',');
        

        colCnt = int.Parse(stringList[0]);
        rowCnt = int.Parse(stringList[1]);
    }
}