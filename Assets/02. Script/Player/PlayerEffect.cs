using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*************************   정보   **************************

    플레이어의 이펙트를 관리하는 스크립트

    사용방법 :
    
    1. 플레이어에 스크립트를 추가
    2. 각 스킬 프리펩을 연결해주자

*************************************************************/

public class PlayerEffect : MonoBehaviour {

    public GameObject[] effects;
    private Transform playerTr;

    Queue<GameObject> effectQueue = new Queue<GameObject>();

    void Start()
    {
        playerTr = GetComponent<PlayerCtrl>().transform;
    }

    public void StartEffect(PlayerEffectList effectState)
    {
        StartCoroutine(ShowEffected(effectState));
    }

    IEnumerator ShowEffected(PlayerEffectList effectState)
    {
        Vector3 playerVec = this.transform.position;
        switch (effectState)
        {
            // 플레이어 죽음 이펙트
            case PlayerEffectList.DIE:
                playerVec.y += 1f;
                effects[(int)effectState].transform.position = playerVec;

                effects[(int)effectState].SetActive(true);
                yield return new WaitForSeconds(1f);
                effects[(int)effectState].SetActive(false);
                break;
            // 플레이어 기본 점프 이펙트
            case PlayerEffectList.BASIC_JUMP:
                GameObject jumpEffect = (GameObject)Instantiate(effects[1], 
                    new Vector3(playerVec.x, playerVec.y + 1f, playerVec.z), Quaternion.identity);
                effectQueue.Enqueue(jumpEffect);
                StartCoroutine(SetOffEffect());
                break;
        }
    }

    IEnumerator SetOffEffect()
    {
        yield return new WaitForSeconds(5f);
        Destroy(effectQueue.Dequeue());
    }
}
