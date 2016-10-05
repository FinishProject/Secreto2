using UnityEngine;
using System.Collections;

public class RotationPlayer : MonoBehaviour {

    private bool isShow = false;

    public Transform lookPoint;
    public Transform rightPoint;

    private Vector3 relativePos;
    private Transform playerTr;

	void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            WahleCtrl.curState = WahleCtrl.instance.StepHold();
            playerTr = PlayerCtrl.instance.transform;

            relativePos = lookPoint.position - playerTr.position;
            Quaternion lookRot = Quaternion.LookRotation(relativePos);

            playerTr.rotation = Quaternion.Slerp(playerTr.rotation, new Quaternion(0f, lookRot.y, 0f, lookRot.w), 5f * Time.deltaTime);
            isShow = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        playerTr = PlayerCtrl.instance.transform;
        rightPoint.position = new Vector3(transform.position.x + 5f, transform.position.y, playerTr.position.z);
        if (isShow)
        {
            WahleCtrl.instance.ChangeState(WahleState.MOVE);
            float fRight = Mathf.Sign(PlayerCtrl.inputAxis);

            if (fRight == 1)
            {
                relativePos = rightPoint.position - playerTr.position;
                Quaternion lookRot = Quaternion.LookRotation(relativePos);
                //PlayerCtrl.instance.transform.Rotate(new Vector3(0, 1, 0), 45f);
                playerTr.rotation = Quaternion.Slerp(playerTr.rotation, new Quaternion(0f, lookRot.y, 0f, lookRot.w), 10f);
            }
            else
                PlayerCtrl.instance.transform.LookAt(PlayerCtrl.instance.transform.right);

            isShow = false;
        }
    }
}
