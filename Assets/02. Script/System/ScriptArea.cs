using UnityEngine;
using System.Collections;

public class ScriptArea : MonoBehaviour {

    public string setId;
    public string context;

    public bool isLoad = true;
    private bool isActive = false;

	void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isActive)
        {
            isActive = true;
            if (isLoad)
            {
                ScriptMgr.instance.GetScript(setId);
            }
            else if (!isLoad)
            {
                ScriptMgr.instance.SetActiveUI(true, context);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (!isLoad)
            {
                ScriptMgr.instance.SetActiveUI(false, null);
            }
        }
    }
}
