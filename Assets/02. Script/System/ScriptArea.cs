using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

            if (setId.Equals("narration"))
                StartCoroutine(WaitScript());
            
            else if (isLoad)
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

    IEnumerator WaitScript()
    {
        yield return new WaitForSeconds(0.5f);
        ScriptMgr.instance.GetScript(setId);
    }
}
