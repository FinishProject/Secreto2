using UnityEngine;
using System.Collections;

/************************************ 사용 방법 ****************************************

    CameraCtrl_7을 사용할때 필요한 카메라 정보
    다음 노드와 이전 노드 정보(Transform)를 가지고 있다.

    ※ 사용 방법
    1. 카메라가 지나갈 동선에 오브젝트를 배치하고 스크립트를 추가한다
    2. 배치를 하고 Q를 눌러주어야 한다 ( "저장됨" 이라는 로그가 뜨는지 항상 확인 )
    3. 이전 노드와 다음노드를 추가해준다.

    ★ 오브젝트의 이름을 모두 다르게 주어야한다.
       (오브젝트 이름으로 Trasnsform 정보를 저장하기 때문 )

****************************************************************************************/

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
