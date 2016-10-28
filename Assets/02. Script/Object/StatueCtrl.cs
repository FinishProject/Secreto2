using UnityEngine;
using System.Collections;

public class StatueCtrl : MonoBehaviour {

    public bool isBlow = false;

	void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isBlow = true;
        }
    }
}
