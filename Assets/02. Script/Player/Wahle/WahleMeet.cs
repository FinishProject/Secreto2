using UnityEngine;
using System.Collections;

public class WahleMeet : WahleCtrl
{

    protected override IEnumerator CurStateUpdate()
    {
        while (true)
        {
            distance = (playerTr.position - transform.position).sqrMagnitude;
            if (distance <= 5f)
            {
                
                base.ChangeState(WahleState.MOVE);
            }
            yield return null;
        }
    }
}
