using UnityEngine;
using System.Collections;

public class WahleMeet : WahleCtrl
{
    public Transform targetPoint;

    public float waitTime = 1f;
    private float dTime = 0f;
    public float lookSpeed = 0.1f;

    bool isMove = false;
    bool isActive = false;
    bool isStop = false;

    new void Start()
    {
        //StartCoroutine(TestMove());
    }

    protected override IEnumerator CurStateUpdate()
    {
        anim.SetBool("Move", true);

        while (true)
        {
            float curDir = Mathf.Sign(playerTr.position.x - transform.position.x);
            Debug.Log(curDir);
            if (curDir != -1f)
            {

                base.ChangeState(WahleState.MOVE);
            }

            //if (100f >= (playerTr.position - transform.position).sqrMagnitude)
            //{
            //    isMove = true;
            //    //PlayerCtrl.instance.isMove = false;
            //    //PlayerCtrl.instance.animReset();
            //}

            //if (isMove)
            //{
            //    relativePos = targetPoint.position - transform.position;
            //    distance = relativePos.sqrMagnitude;

            //    if(!isStop)
            //        lookRot = Quaternion.LookRotation(relativePos);

            //    if (distance < 8f && !isActive)
            //        StartCoroutine(CountDown());

            //    transform.localRotation = Quaternion.Slerp(transform.localRotation, lookRot, lookSpeed * Time.deltaTime);
            //    transform.Translate(Vector3.forward * maxSpeed * Time.deltaTime);

            //    if (!isStop)
            //        maxSpeed = DecreaseSpeed(maxSpeed, 2f, accel);
            //}

            yield return null;
        }
    }

    IEnumerator GoPlayer()
    {

        yield return new WaitForSeconds(waitTime);
        isStop = true;
        while (true)
        {
            relativePos = playerTr.position - transform.position;
            lookRot = Quaternion.LookRotation(relativePos);

            yield return null;
        }
    }

    IEnumerator CountDown()
    {
        isActive = true;
        yield return new WaitForSeconds(waitTime);
        isStop = true;
        

        while (true)
        {
            maxSpeed = DecreaseSpeed(maxSpeed, 0f, 1f);

            relativePos = playerTr.position - transform.position;
            lookRot = Quaternion.LookRotation(relativePos);
            yield return null;
        }
    }
}
