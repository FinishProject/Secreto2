using UnityEngine;
using System.Collections;

public class RotateHold : MonoBehaviour {

    private float rotDir = 1f;
    private Transform playerTr;

    private RotationMoveHold moveScript;

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;

        moveScript = GetComponentInParent<RotationMoveHold>();
    }

    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            float speed = 0f;
            if (moveScript != null)
                speed = moveScript.GetSpeed();

            WahleCtrl.curState = WahleCtrl.instance.StepHold();
            playerTr.Translate(this.transform.position.normalized * (speed * PlayerCtrl.focusRight) 
                * Time.deltaTime);


            //Vector3 stepDir = col.ClosestPointOnBounds(col.transform.position);

            //if (stepDir.x <= transform.position.x)
            //    rotDir = 1f;
            //else if (stepDir.x >= transform.position.x)
            //    rotDir = -1f;

            //if(this.transform.eulerAngles.x < 275f)
            //    transform.Rotate(Vector3.up * rotDir * 50f * Time.deltaTime);
        }
    }
}
