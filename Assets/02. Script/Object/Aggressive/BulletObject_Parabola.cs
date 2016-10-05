using UnityEngine;
using System.Collections;

public class BulletObject_Parabola : MonoBehaviour {


    public void Moving(Transform startPos, float range)
    {
        StartCoroutine(ParabolaMoving(startPos, range));
    }

    IEnumerator ParabolaMoving(Transform startPos, float range)
    {
        Vector3 relativePos;
        Quaternion lookRot;

        transform.position = startPos.position;
        transform.rotation = new Quaternion(0f, 0f, startPos.rotation.z, 0f);
        float angle = 70f;

        var targetPos = PlayerCtrl.instance.transform.position;
        targetPos.x += Random.Range(-range, range);
        targetPos.y += PlayerCtrl.instance.transform.localScale.y;
        var dir = targetPos - transform.position;  // 방향 벡터
        var h = dir.y;                             // 높이 차이
        dir.y = -2;                                 // 수평방향만 유지 하기 위해 (높이 조절 가능)
        var dist = dir.magnitude;                  // 수평 거리
        var a = angle * Mathf.Deg2Rad;             // 각도를 라디안으로 변환
        dir.y = dist * Mathf.Tan(a);               // 방향을 앙각으로 맞춤( 지평면과 포신 방향이 이루는 각)
        dist += h / Mathf.Tan(a);                  // 정확한 높이 차이를 위해             
        var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a)); // 속도를 계산
        var ballistic = vel * dir.normalized;

        while (true)
        {
            relativePos = targetPos - transform.position;
            lookRot = Quaternion.LookRotation(relativePos);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, 
                new Quaternion(0f, 0f, lookRot.z, lookRot.w), 4f * Time.deltaTime);

            ballistic += Physics.gravity * Time.deltaTime;
            transform.position += ballistic * Time.deltaTime;
            yield return null;
        }

    }

    void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("MONSTER"))
        {
            if (col.tag.Equals("Player"))
            {
                PlayerCtrl.instance.getDamage(10);
                gameObject.SetActive(false);
            }
            //else if (col.tag.Equals("BULLET") || col.tag.Equals("MONSTER"))
            //    return;

            
        }
    }
}
