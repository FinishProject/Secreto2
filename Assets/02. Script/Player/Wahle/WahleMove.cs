using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WahleMove : WahleCtrl
{

    public float stateChangeTime = 8f;
    public float slerpSpeed = 0.5f;
    public float lookSpeed = 5f;
    private float changeTime = 0f;
    private float focusDir = 1f; // 봐라보고 있는 방향 오른쪽 : 1, 왼쪽 : -1

//    private float startTime = Time.time;

    protected override IEnumerator CurStateUpdate()
    {
        initSpeed = 0f;
        while (true)
        {
            focusDir = PlayerCtrl.focusRight;
            relativePos = playerTr.position - transform.position;
            lookRot = Quaternion.LookRotation(relativePos);
            distance = relativePos.sqrMagnitude;

            // 플레이어가 멈췄을 시
            if (PlayerCtrl.inputAxis == 0f && distance <= 4f)
            {
                TurningPlayer();
            }
            // 플레이어가 이동중일 시
            else
            {
                Movement();
            }
            yield return null;
        }
    }

    // 플레이어를 따라 이동
    private void Movement()
    {
        if (!PlayerCtrl.controller.isGrounded)
        {
            initSpeed = 2f;
        }
        else
        {
            initSpeed = IncrementSpeed(initSpeed, maxSpeed, accel); // 이동속도 가속도                              
        }
        // 플레이어를 봐라봄
        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            lookRot, 2f * Time.deltaTime);

        // 플레이어 추격
        transform.position = Vector3.Lerp(transform.position, playerTr.position - (playerTr.forward),
                     initSpeed * Time.deltaTime);

        //initSpeed = IncrementSpeed(initSpeed, maxSpeed, accel); // 이동속도 가속도     
        //// 플레이어를 봐라봄
        //transform.localRotation = Quaternion.Slerp(transform.localRotation,
        //    lookRot, lookSpeed * Time.deltaTime);

        //if (!PlayerCtrl.controller.isGrounded)
        //{
        //    Vector3 center = (transform.position + playerTr.position) * 0.1F;
        //    center -= new Vector3(0, 1, 0);
        //    Vector3 riseRelCenter = transform.position - center;
        //    Vector3 setRelCenter = playerTr.position - center;
        //    transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, slerpSpeed * Time.deltaTime);
        //    transform.position += center;
        //}
        //// 플레이어 추격
        //transform.position = Vector3.Lerp(transform.position, playerTr.position - (playerTr.forward),
        //           initSpeed * Time.deltaTime);


    }

    // 플레이어 주위를 회전
    private void TurningPlayer()
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, lookRot,
            0.5f * Time.deltaTime);
        transform.Translate(Vector3.forward * 1f * Time.deltaTime);

     
        changeTime += Time.deltaTime;

        // 일정 시간후 대기상태로 전환
        if (changeTime >= stateChangeTime)
        {
            changeTime = 0f;
            base.ChangeState(WahleState.IDLE);
        }
    }

    public void ResetSpeed()
    {
        initSpeed *= 0.2f;
    }
}