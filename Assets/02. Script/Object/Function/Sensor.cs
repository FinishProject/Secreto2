using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    센서
    (범위 내에 충돌체크)

    사용방법 :
    
    1. 센서를 달고 싶은 오브젝트에 빈 자식오브젝트를 생성
    2. 빈 자식오브젝트에 이 스크립트를 추가
    3. 부모 스크립트에 Sensorable_Player 인터페이스를 이용해 센서입력을 받자!

    ※ 부모 오브젝트에서 ActiveSensor_Player를 실행시켜,
    반환한 값이 true이면 
*************************************************************/

// 센서를 작동, 작동되면 true, 작동이 안되면 false 반환
public interface Sensorable_Player
{
    bool ActiveSensor_Player(int index);
}

public interface Sensorable_Something
{
    bool ActiveSensor_Something(int index);
}

public interface Sensorable_Return
{
    void ActiveSensor_Retuen(int index, GameObject gameObject);
}

public class Sensor : MonoBehaviour
{
    public int index = 0; // 여러개의 센서가 들어가는 부모 오브젝트를 위한 index
    public bool isReturnObject = false;
    public string colliderName = "NULL";
    void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Player"))
        {
            if (transform.parent.GetComponent<Sensorable_Player>() != null &&
                transform.parent.GetComponent<Sensorable_Player>().ActiveSensor_Player(index))
                gameObject.SetActive(false);
        }

        if (col.tag.Equals(colliderName))
        {
            /*
            if (!isReturnObject && transform.parent.GetComponent<Sensorable_Something>() != null &&
                transform.parent.GetComponent<Sensorable_Something>().ActiveSensor_Something(index))
                gameObject.SetActive(false);
                */
            if (isReturnObject && transform.parent.GetComponent<Sensorable_Return>() != null)
                transform.parent.GetComponent<Sensorable_Return>().ActiveSensor_Retuen(index, col.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag.Equals(colliderName))
        {
            /*
            if (!isReturnObject && transform.parent.GetComponent<Sensorable_Something>() != null)
                transform.parent.GetComponent<Sensorable_Something>().ActiveSensor_Something(index + 100);
                */
            if (isReturnObject && transform.parent.GetComponent<Sensorable_Return>() != null)
                transform.parent.GetComponent<Sensorable_Return>().ActiveSensor_Retuen(index, null);
        }
    }

}