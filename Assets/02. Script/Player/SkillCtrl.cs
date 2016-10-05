using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AttributeState
{
    noraml = 0,
    red = 1,
    blue = 2
}

public class SkillCtrl : MonoBehaviour {

    private GameObject[] bullet;
    public GameObject smashBullet;

    private GameObject enhanceBullet;
    private GameObject targetObj;
    private Transform playerTr;
    public Transform shotPoint;
    public GameObject normalBullet;

    public float initSmashDamage = 10f;
    public float maxSmashDamgae = 20f;
    private float increDamage = 20f;
    
    private int count = 0;
    public int bulletNum = 3;
    public float curEnhance = 0;
    public float maxEnhance = 10;

    [System.NonSerialized]
    public AttributeState curAttribute;         // 속성   
    public float attributeDuration = 5.0f;      // 속성 지속시간 (시간이 끝나면 noraml)
    private float _countDownForAttribute;
    public float ProportionAttribute
    {
        get { return _countDownForAttribute / attributeDuration; }
    }

    public float ProportionEnhance
    {
        get { return curEnhance / maxEnhance; }
    }

    public static SkillCtrl instance;

    void Awake()
    {
//        instance = this;
        curAttribute = AttributeState.noraml;
        _countDownForAttribute = attributeDuration;
    }

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        // 발사체 생성
        bullet = new GameObject[bulletNum];
        for (int i = 0; i < bullet.Length; i++)
        {
            bullet[i] = (GameObject)Instantiate(normalBullet, transform.position, Quaternion.identity);
            bullet[i].SetActive(false);
        }
        enhanceBullet = (GameObject)Instantiate(smashBullet, transform.position, Quaternion.identity);
        enhanceBullet.SetActive(false);
    }

    void Update()
    {
        // F키 입력 시 공격체 생성
        if (Input.GetKeyDown(KeyCode.F)) {
            if (count >= bullet.Length) { count = 0; } // 총알 갯수 넘을 시 Index 초기화
            else {

                targetObj = DistanceCompare(FindTarget());
                bullet[count].SetActive(true);
                bullet[count].transform.position = shotPoint.position;
                // 현재 발사체에게 타겟 포지션을 알려줌
                LauncherCtrl launcher = bullet[count].GetComponent<LauncherCtrl>();
                launcher.GetTarget(targetObj);
                count++;
                //InGameUI.instance.ChangeEnhance();
            }
        }
        if(curEnhance >= maxEnhance)
        {
            if (Input.GetKey(KeyCode.G))
            {
                if (initSmashDamage >= maxSmashDamgae)
                {
                    initSmashDamage = maxSmashDamgae;
                }
                else
                    initSmashDamage += increDamage * Time.deltaTime;
            }
            else if (Input.GetKeyUp(KeyCode.G))
            {
                List<Transform> targetList = FindTarget();
                if (targetList != null)
                {
                    enhanceBullet.SetActive(true);
                    enhanceBullet.transform.position = this.transform.position;

                    StartCoroutine(HideBullet());

                    for (int i = 0; i < targetList.Count; i++)
                    {
                        FSMBase monster = targetList[i].GetComponent<FSMBase>();
                        monster.GetDamage(initSmashDamage);
                    }
                    curEnhance = 0f;
                    initSmashDamage = 0f;
                }
            }
        }
    }

    //타겟 탐색
    List<Transform> FindTarget()
    {
        Collider[] hitCollider = Physics.OverlapSphere(playerTr.position, 8f);
        List<Transform> targetList = new List<Transform>(); // 주변 몬스터 위치를 저장할 리스트
        for (int i = 0; i < hitCollider.Length; i++) { 
            if (hitCollider[i].CompareTag("MONSTER")) {
                targetList.Add(hitCollider[i].transform);
            }
        }
        // 타겟이 1개 이상일 시 거리 비교
        if (targetList.Count > 0)
        {
            return targetList;
        }
        else
            return null;
    }
    // 가장 가까운 타겟의 위치를 찾아 타겟의 위치값을 넘겨줌
    GameObject DistanceCompare(List<Transform> targetTrList)
    {
        float nearDistance = (playerTr.position - targetTrList[0].position).sqrMagnitude;
        int targetIndex = 0;
        // 각 타겟의 거리를 비교하여 가장 가까운 타겟을 찾음
        for (int i = 1; i < targetTrList.Count; i++)
        {
            float curDistance = (playerTr.position - targetTrList[i].position).sqrMagnitude;
            if (nearDistance > curDistance) {
                nearDistance = curDistance;
                targetIndex = i;
            }
        }
        return targetTrList[targetIndex].gameObject;
    }

    public void ChangeAttribute(AttributeState attribute)
    {
        StopAllCoroutines();
        InGameUI.instance.ChangeCountDownForAttributeBar();
        StartCoroutine(StartChangeAttribute(attribute));
    }

    // 속성 지속
    IEnumerator StartChangeAttribute(AttributeState attribute)
    {
        curAttribute = attribute;
        InGameUI.instance.ChangeAttribute();
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            _countDownForAttribute -= 0.05f;

            if(_countDownForAttribute < 0.05f)
            {
                _countDownForAttribute = attributeDuration;
                curAttribute = AttributeState.noraml;
                InGameUI.instance.ChangeAttribute();
                break;
            }
        }
    }

    // 인핸스 증가 ( 마나 )
    public void AddEnhance()
    {
        curEnhance++;
       
        InGameUI.instance.ChangeEnhance();
        if (curEnhance > maxEnhance)
        {
            curEnhance = maxEnhance;
        }

    }

    IEnumerator HideBullet()
    {
        yield return new WaitForSeconds(1f);
        enhanceBullet.SetActive(false);
    }

    /*
    void OnGUI()
    {
        string tempText;
        tempText = "현재 인핸스 : " + curEnhance.ToString();
        tempText += "\n적용 속성  : " + curAttribute.ToString();


        if(!curAttribute.Equals(AttributeState.noraml))
        {
            tempText += "\n남은 시간 : " + Mathf.Round(_countDownForAttribute).ToString();
        }

        GUI.TextField(new Rect(0, 0, 300.0f, 60.0f), tempText);
    }
    */

    //public void StartReset(int curIndex)
    //{
    //    bulletInfo[curIndex].Bullet.SetActive(false);
    //    //StartCoroutine(ResetBullet(curIndex));
    //}
    //// 탄환 발사 후 사라질 수 재생성
    //IEnumerator ResetBullet(int index)
    //{
    //    yield return new WaitForSeconds(initTime);
    //    bullet[count].SetActive(true);
    //}

}
