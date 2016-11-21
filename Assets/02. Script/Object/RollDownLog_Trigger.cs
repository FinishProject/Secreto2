using UnityEngine;
using System.Collections;

/********************************************** 사용 방법 ***************************************************

    RollDownLog를 사용하기 위해 필요한 스크립트
    구간에 들어가면 통나무를 생성
    
    ※ 사용 방법
    1. 오브젝트 구조
    부모오브젝트
        └ 통나무           <- RollDownLog 스크립트 추가
        └ 구역(Collider)   <- 현 스크립트 추가


************************************************************************************************************/

public class RollDownLog_Trigger : MonoBehaviour {
    public GameObject log;
    public GameObject log2;
    IEnumerator roll;
    IEnumerator roll2;

    public AudioClip clip;
    private AudioSource source;

    public static bool isStarted;
    void Start()
    {
        source = GetComponent<AudioSource>();
        roll = log.GetComponent<RollDownLog>().loopRollDown();
        roll2 = log2.GetComponent<RollDownLog>().loopRollDown();   
    }

	void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            isStarted = true;
            StartCoroutine(RollingLog());
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isStarted = false;
        }
    }

    IEnumerator RollingLog()
    {
        while(true)
        {
            
            if (isStarted)
            {
                if (!source.isPlaying)
                {
                    source.clip = clip;
                    source.Play();
                    source.loop = true;
                }
                StartCoroutine(roll);
                yield return new WaitForSeconds(3f);
                StartCoroutine(roll2);
            }
            else
            {
                source.Stop();
                SoundMgr.instance.StopAudio("Earthquake");
                StopCoroutine(roll);
                StopCoroutine(roll2);
            }
            yield return null;

        }

    }
}
