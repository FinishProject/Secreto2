using UnityEngine;
using System.Collections;

public class WahleSwitch : WahleCtrl
{

    protected override IEnumerator CurStateUpdate()
    {
        while (true)
        {
            yield return null;
        }
    }
}
