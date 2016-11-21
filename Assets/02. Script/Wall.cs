using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	void OnCollisionStay(Collision col)
    {
        if (col.collider.CompareTag("OBJECT"))
        {
            if (PushBox.instance.isActive)
            {
                PushBox.instance.isPush = false;
            }
            else
            {
                PushBox.instance.isPush = true;
            }
        }
    }
}
