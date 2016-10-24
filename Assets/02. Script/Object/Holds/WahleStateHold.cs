using UnityEngine;
using System.Collections;

public class WahleStateHold : MonoBehaviour {

	void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            WahleCtrl.curState = WahleCtrl.instance.StepHold();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            WahleCtrl.instance.ChangeState(WahleState.MOVE);
        }
    }
}
