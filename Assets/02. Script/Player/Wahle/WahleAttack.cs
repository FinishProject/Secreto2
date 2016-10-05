using UnityEngine;
using System.Collections;

public class WahleAttack : WahleCtrl {

    private GameObject targetObj; // 몬스터(타겟) 객체

    protected override IEnumerator CurStateUpdate()
    {
        isSearch = true;
        initSpeed = 3f;

        // 플레이어 정면에 위치
        Vector3 fromPointVec = playerTr.position + ((playerTr.forward * 2f) + (playerTr.up * 0.5f));

        while (true)
        {
            // 화면 밖으로 나가거나 타겟이 사라졌을 시 이동으로 전환
            if (CheckOutCamera() || targetObj.GetComponent<FSMBase>().isDeath)
            {
                isSearch = false;
                ChangeState(WahleState.MOVE);
            }

            Vector3 enemyRelativePos = targetObj.transform.position - transform.position;
            lookRot = Quaternion.LookRotation(enemyRelativePos);

            initSpeed += Time.deltaTime * 5f;
            // 각도 제한
            if (transform.localRotation.x <= 0.24f)
                lookRot.x = 0f;

            
            // 지정 된 위치로 포물선을 그리며 이동
            Vector3 center = (fromPointVec - transform.position) * 0.8f;
            center -= new Vector3(0, 1, 1);
            transform.position = Vector3.Slerp(transform.position - center, fromPointVec  - center, initSpeed * Time.deltaTime);
            transform.position += center;
            // 타겟을 봐라보도록 회전
            transform.localRotation = Quaternion.Slerp(transform.localRotation, lookRot, 5f * Time.deltaTime);


            yield return null;
        }
    }

    // 고래가 추적할 몬스터 객체를 받아옴
    public void GetTarget(GameObject _Target)
    {
        targetObj = _Target;
    }
}
