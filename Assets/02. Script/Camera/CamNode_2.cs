using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CamNode_2 : MonoBehaviour
{
    public bool settingOnEditor;

    public Transform[] PrevNodes;
    public Transform[] NextNodes;

    void Start()
    {
        //        if(settingComplete)
        getData();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            setData();
            Debug.Log("저장됨");
        }
        else if (settingOnEditor)
        {
            setData();
            Debug.Log("저장됨");
        }

        for (int i = 0; i < NextNodes.Length; i++)
        {
            Debug.DrawLine(transform.position, NextNodes[i].position);
        }
    }


    Vector3 tempVec;
    Quaternion tempQuat;
    string tempString;
    void setData()
    {
        tempString = transform.position.x.ToString() + "," +
                     transform.position.y.ToString() + "," +
                     transform.position.z.ToString() + "," +
                     transform.eulerAngles.x.ToString() + "," +
                     transform.eulerAngles.y.ToString() + "," +
                     transform.eulerAngles.z.ToString();

        PlayerPrefs.SetString(name, tempString);
    }

    void getData()
    {
        tempString = PlayerPrefs.GetString(name);
        string[] stringList = tempString.Split(',');

        tempVec.x = float.Parse(stringList[0]);
        tempVec.y = float.Parse(stringList[1]);
        tempVec.z = float.Parse(stringList[2]);
        transform.position = tempVec;

        tempVec.x = float.Parse(stringList[3]);
        tempVec.y = float.Parse(stringList[4]);
        tempVec.z = float.Parse(stringList[5]);

        transform.eulerAngles = tempVec;

    }

}
