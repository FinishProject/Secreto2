using UnityEngine;
using System.Collections;

public class SideSmashObj : MonoBehaviour {
   
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            StartCoroutine(PlayerCtrl.instance.PlayerDie());
        }
    }
}
