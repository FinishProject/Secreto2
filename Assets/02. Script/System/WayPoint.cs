using UnityEngine;
using System.Collections;

public class WayPoint : MonoBehaviour {

    public delegate void SaveSystem();
    public static event SaveSystem OnSave;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && PlayerCtrl.dying)
        {
            Debug.Log("부활");
            InGameUI_2.instance.AvtiveLoad();
        }
        else if(col.CompareTag("Player") && !PlayerCtrl.dying)
        {
            OnSave();
            InGameUI_2.instance.AvtiveSave();
        }
           
    }
}
