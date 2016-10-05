using UnityEngine;
using System.Collections;

public class CountObj : MonoBehaviour {

    public int destroyCnt = 3;

	void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "BULLET")
        {
            destroyCnt--;

            if(destroyCnt <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
