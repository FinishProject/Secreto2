using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScriptArea : MonoBehaviour {

    public string setId;
    public string context;

    public bool isLoad = true;
    public bool isReroad = true;
    private bool isActive = false;
    private bool isActive2 = false;

	void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isActive)
        {
            WahleCtrl.instance.PlayRandomSound();
            if(!isReroad)
                isActive = true;

            if (isLoad)
                ScriptMgr.instance.GetScript(setId);
            else
            {
                //WahleCtrl.instance.PlayRandomSound();
                ScriptMgr.instance.SetActiveUI(true, context);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (!isLoad && !isActive2)
            {
                if (!isReroad)
                    isActive2 = true;
                ScriptMgr.instance.SetActiveUI(false, null);
            }
        }
    }
}
