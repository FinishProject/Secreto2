using UnityEngine;
using System.Collections;

public class WahleIdle : WahleCtrl
{

    public float lookSpeed = 0.3f;
    public Transform targetPoint;

    protected override IEnumerator CurStateUpdate()
    {
        anim.SetBool("Move", true);
        while (true)
        {
            // 플레이어가 이동하고, 고래가 화면 밖으로 나갔을 시 이동으로 전환
            if (PlayerCtrl.inputAxis >= 1f || PlayerCtrl.inputAxis <= -1f)
            {
                distance = (playerTr.position - transform.position).sqrMagnitude;
                if (distance >= 20f)
                {
                    base.ChangeState(WahleState.MOVE);
                }
            }
            else
                Wander();
            
            yield return null;
        }
    }

    // 카메라 안의 공간을 배회
    private void Wander()
    {
        if(distance <= 4f)
        {
            targetPoint.position = base.SetRandomPos();
        }
        relativePos = targetPoint.position - transform.position;
        distance = relativePos.sqrMagnitude;
        lookRot = Quaternion.LookRotation(relativePos);

        relativePos = ShotRay(relativePos);

        //transform.RotateAround(this.transform.forward, 1f * Time.deltaTime);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, lookRot, lookSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * maxSpeed * Time.deltaTime);
    }

    // NPC와 대화시 고래가 플레이어 좌측에 위치
    private void SpeakingNpc()
    {
        relativePos = npcPos - transform.position;
        lookRot = Quaternion.LookRotation(relativePos);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, lookRot, 5f * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, playerTr.position - (playerTr.right),
               3f * Time.deltaTime);
    }

    public void SearchObject()
    {
        targetPoint.position = base.SetRandomPos();
    }
}
