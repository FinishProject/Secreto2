using UnityEngine;
using System.Collections;

public class Statue : MonoBehaviour {

	void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            bool isBlow = transform.GetComponentInParent<StatueCtrl>().isBlow;

            if (isBlow)
            {
                GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }
}
