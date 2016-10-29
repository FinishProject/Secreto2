using UnityEngine;
using System.Collections;

public class Stone : MonoBehaviour {

    void OnCollisionEnter(Collision col)
    {
        if(!col.collider.CompareTag("DeadLine"))
            this.gameObject.tag = "Untagged";
    }
}
