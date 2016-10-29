using UnityEngine;
using System.Collections;

public class Change : MonoBehaviour {

	// Use this for initialization
	void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            WahleCtrl.instance.ChangeState(WahleState.MOVE);
        }
    }
}
