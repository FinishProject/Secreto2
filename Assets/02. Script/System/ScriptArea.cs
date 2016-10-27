using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScriptArea : MonoBehaviour {

    public string setId;
    public string context;

    public bool isLoad = true;
    private bool isActive = false;
    private bool isActive2 = false;

	void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isActive)
        {
            isActive = true;

            if(isLoad)
                ScriptMgr.instance.GetScript(setId);
            else
            ScriptMgr.instance.SetActiveUI(true, context);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (!isLoad && !isActive2)
            {
                isActive2 = true;
                ScriptMgr.instance.SetActiveUI(false, null);
                //StartCoroutine(Off());
            }
        }
    }

    IEnumerator Off()
    {
        yield return new WaitForSeconds(1.5f);
        this.gameObject.SetActive(false);
    }

    IEnumerator WaitScript()
    {
        yield return new WaitForSeconds(0.5f);
        ScriptMgr.instance.GetScript(setId);
    }
}
