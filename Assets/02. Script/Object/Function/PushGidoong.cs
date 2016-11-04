using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    캐릭터가 일정 구역에 들어가면 기둥이 넘어진다.

    사용방법 :
    1. 구조
    FallGidoong           <- 스크립트 추가 (빈 오브젝트)
        └ Area           <- Sensor 스크립트 추가
        └ PushGidoong    <- 기둥을 밀어줄 투명 오브젝트
        └ Gidoong        <- 기둥

    ※ 리지드 바디가 붙어있나 확인하세요~!
*************************************************************/

public class PushGidoong : MonoBehaviour, Sensorable_Player
{ 
    public GameObject pusher;
    public GameObject childObj;
    Rigidbody pushersRigidbody;
    int passCount;
    public bool pushFirstTime;

    bool isPlay = false;
	void Start () {
        pushersRigidbody = pusher.GetComponent<Rigidbody>();
        passCount = 0;
    }

    public bool ActiveSensor_Player(int index)
    {
        passCount++;
        if(pushFirstTime || passCount >= 2)
        {
            pushersRigidbody.AddForce(-Vector3.forward * 3000);

            if (!isPlay)
            {
                isPlay = true;
                StartCoroutine(PlaySound());
                StartCoroutine(RemoveTag());
            }
        }
        return true;
    }
    IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(2f);
        SoundMgr.instance.PlayAudio("Fall_Statue", false, 0.5f);
        yield return new WaitForSeconds(1f);
        SoundMgr.instance.StopAudio("Fall_Statue");
    }

    IEnumerator RemoveTag()
    {
        yield return new WaitForSeconds(2f);
        pusher.SetActive(false);
        childObj.tag = "Untagged";
    }
}
