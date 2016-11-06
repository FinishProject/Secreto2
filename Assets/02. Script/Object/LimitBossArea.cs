using UnityEngine;
using System.Collections;

public class LimitBossArea : MonoBehaviour {

    PushBox pushBox;

	void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("OBJECT"))
        {
            pushBox = col.GetComponent<PushBox>();
            pushBox.isPush = false;
        }
        else if (col.CompareTag("Player"))
        {
            if(pushBox != null)
                pushBox.isPush = true;
        }
    }
}
