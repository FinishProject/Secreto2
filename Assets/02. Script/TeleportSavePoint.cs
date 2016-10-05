using UnityEngine;
using System.Collections;

public class TeleportSavePoint : MonoBehaviour {

    private GameObject[] savePointTr;
    private int pressKey = 0;

	void Start () {
        savePointTr = GameObject.FindGameObjectsWithTag("SavePoint");
	}
	
	void Update () {
        pressKey = System.Convert.ToInt32(Input.inputString) - 1;
        if(pressKey < savePointTr.Length)
            PlayerCtrl.instance.transform.position = savePointTr[pressKey].transform.position;
    }
}
