using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    특정 오브젝트가 있어야 열리는 문

    사용방법 :
    
    1. 필요한 아이템 Id를 꼭! 넣어주자~★

*************************************************************/

public class OpenByCollectionItem : MonoBehaviour , Sensorable_Player
{
    public int needItemId;  // 필요한 아이템 ID

    // 센서를 작동, 작동되면 true, 작동이 안되면 false 반환
    public bool ActiveSensor_Player(int index)
    {
        if(Inventory.instance.Find(needItemId) != null)
        {
            StartCoroutine(OpenTheDoor());
            return true;
        }
        return false;
    }

    // 문이 열린다.
    IEnumerator OpenTheDoor()
    {
        Transform thisTr = gameObject.transform;
        Vector3 TargetPos = thisTr.position;
        TargetPos.y += thisTr.localScale.y;
        while (true)
        {
            thisTr.position = Vector3.Lerp(thisTr.position, TargetPos, Time.deltaTime);

            if (Vector3.Distance(thisTr.position, TargetPos) < 0.5f)
                break;

            yield return null;
        }
    }
}
