using UnityEngine;
using System.Collections;

public class ScriptArea : MonoBehaviour {

    public string setId;

	void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            ScriptMgr.instance.GetScript(setId);
        }
    }
}
