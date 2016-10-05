using UnityEngine;
using System.Collections;

public class BouncingMushroom : MonoBehaviour {

    public float jumpHight = 10f;
    Vector3 moveDir = Vector3.zero;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player") && !PlayerCtrl.controller.isGrounded
            && PlayerCtrl.instance.transform.position.y > this.transform.position.y)
        {
            PlayerCtrl.moveDir.y = jumpHight;
            PlayerCtrl.controller.Move(moveDir * jumpHight * Time.deltaTime);
        }
    }

}
