using UnityEngine;
using System.Collections;

public class SmashLauncher : MonoBehaviour {

    public float speed = 10f;
    private float damage = 10f;

    private GameObject target;
    
    IEnumerator MoveUpdate()
    {
        while (true)
        {
            // 직선 공격
            Vector3 relativePos = this.target.transform.position - this.transform.position;
            transform.position = Vector3.Lerp(this.transform.position, target.transform.position, speed * Time.deltaTime);

            // 타겟 사라졋을 시 탄환체도 사라지도록
            if (!target.activeSelf)
            {
                target = null;
                this.gameObject.SetActive(false);
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        // 몬스터와 충돌 시 주변 몬스터까지 데미지를 입음
        if (col.CompareTag("MONSTER"))
        {
            Collider[] hitCollider = Physics.OverlapSphere(this.transform.position, 5f);
            for(int i=0; i<hitCollider.Length; i++)
            {
                if (hitCollider[i].CompareTag("MONSTER"))
                {
                    FSMBase monster = hitCollider[i].GetComponent<FSMBase>();
                    monster.GetDamage(damage);
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
    // 타겟과 데미지를 얻어옴
    public void GetTarget(GameObject _target, float _damage)
    {
        target = _target;
        damage = _damage;
        StartCoroutine(MoveUpdate());
        StartCoroutine(Duration());
    }

    // 탄환이 날아가면서 지속되는 시간
    IEnumerator Duration()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
        //SkillCtrl.instance.StartReset(index);
    }
}
