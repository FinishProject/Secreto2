using UnityEngine;
using System.Collections;

public class RollDownLog_Trigger : MonoBehaviour {
    public GameObject log;
    public GameObject log2;
    IEnumerator roll;
    IEnumerator roll2;

    bool isStarted;
    void Start()
    {
        roll = log.GetComponent<RollDownLog>().loopRollDown();
        roll2 = log2.GetComponent<RollDownLog>().loopRollDown();

        
    }

	void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            isStarted = true;
            StartCoroutine(a());
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isStarted = false;
        }
    }

    IEnumerator a()
    {
        while(true)
        {
            if (isStarted)
            {
                SoundMgr.instance.PlayAudio("Earthquake", true, 1f);
                StartCoroutine(roll);
                yield return new WaitForSeconds(3f);
                StartCoroutine(roll2);
            }
            else
            {
                SoundMgr.instance.StopAudio("Earthquake");
                StopCoroutine(roll);
                StopCoroutine(roll2);
            }
            yield return null;

        }

    }
}
