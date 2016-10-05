using UnityEngine;
using System.Collections;

public class LauncherCtrl : MonoBehaviour {

    public float speed = 10f;
    public float durationTime = 1f; // 발사 중 유지시간

    private GameObject target;
    private Transform traceTargetTr = null; // 회전을 위한 타겟
    private Vector3 focusVec;

    /*
    public Material matNormal;
    public Material matRed;
    public Material matBlue;
    */

    private AttributeState _curAttibute;

    private int index = 0; // 현재 발사체 배열의 인덱스

    void Awake()
    {
        traceTargetTr = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        //타겟 있을 시
        if (target != null)
        {
            // 직선 공격
            Vector3 relativePos = this.target.transform.position - this.transform.position;
            transform.position = Vector3.Lerp(this.transform.position, target.transform.position, speed * Time.deltaTime);
            //Quaternion lookRot = Quaternion.LookRotation(relativePos);
            //transform.localRotation = Quaternion.Slerp(transform.localRotation, lookRot, 20f);

            // 포물선 공격
            //Vector3 center = (target.transform.position + this.transform.position) * 0.5f;
            //center -= new Vector3(0, 1, 1);
            //Vector3 fromRelCenter = this.transform.position - center;
            //Vector3 toRelCenter = target.transform.position - center;
            //transform.position = Vector3.Slerp(fromRelCenter, toRelCenter, speed * Time.deltaTime);
            //transform.position += center;

            // 타겟 사라졋을 시 탄환체도 사라지도록
            if (!target.activeSelf) {
                target = null;
                this.gameObject.SetActive(false);
                //SkillCtrl.instance.StartReset(index);
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("MONSTER"))
        {
            var monster = coll.GetComponent<FSMBase>();
            monster.GetDamage(15);
            //switch (monster.curAttibute)
            //{
            //    // 속성 상관 없이 데미지
            //    case AttributeState.noraml:
            //        monster.GetDamage(15);
            //        break;

            //    // 몬스터 빨강 / 발사체 파랑일 때 데미지
            //    case AttributeState.red:

            //        if(_curAttibute.Equals(AttributeState.blue))
            //        {
            //            monster.GetDamage(15);
            //        }
            //        break;

            //    // 몬스터 파랑 / 발사체 빨강일 때 데미지
            //    case AttributeState.blue:

            //        if (_curAttibute.Equals(AttributeState.red))
            //        {
            //            monster.GetDamage(15);
            //        }
            //        break;
            //}

            target = null;
            this.gameObject.SetActive(false);
            //SkillCtrl.instance.StartReset(index);
        }
        
    }

    //void OnEnable()
    //{
    //    _curAttibute = SkillCtrl.instance.curAttribute;
    //    switch (_curAttibute)
    //    {
    //        case AttributeState.noraml:
    //            gameObject.GetComponent<MeshRenderer>().material = matNormal;
    //            break;
    //        case AttributeState.red:
    //            gameObject.GetComponent<MeshRenderer>().material = matRed;
    //            break;
    //        case AttributeState.blue:
    //            gameObject.GetComponent<MeshRenderer>().material = matBlue;
    //            break;
    //    }

    //}

    // 날아갈 타겟 위치와 현재 발사체의 배열 인덱스를 받아옴
    public void GetTarget(GameObject _target)
    {
        this.target = _target;
        StartCoroutine(Duration());
    }

    // 탄환이 날아가면서 지속되는 시간
    IEnumerator Duration()
    {
        yield return new WaitForSeconds(durationTime);
        this.gameObject.SetActive(false);
        //SkillCtrl.instance.StartReset(index);
    }
}
