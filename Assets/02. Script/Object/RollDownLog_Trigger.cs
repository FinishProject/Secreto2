using UnityEngine;
using System.Collections;

public class RollDownLog_Trigger : MonoBehaviour {
    public GameObject log;

	void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            log.GetComponent<RollDownLog>().StartRollDownLog();
        }
    }
}
