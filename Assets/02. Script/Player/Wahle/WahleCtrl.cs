using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WahleState { IDLE, MOVE, ATTACK, SWITCH }

public class WahleCtrl : MonoBehaviour {
    public float maxSpeed = 10f; // 최대 속도
    public float accel = 5f; // 가속도
    protected float initSpeed = 0f; // 초기 속도
    protected float distance = 0f; // 거리 차
    protected bool isSearch = false; // 탐색 여부

    bool isStart = true;
    private bool isSwitch = false;

    protected Vector3 npcPos; // NPC 위치를 저장할 변수
    protected Vector3 relativePos; // 상대적 위치값
    protected Quaternion lookRot; // 봐라볼 방향
    protected Transform playerTr; // 플레이어 위치

    private Transform camTr; // 카메라 위치
    protected Animator anim;
    private FSMBase fsmBase; // 몬스터 상태를 받아오기 위한 변수

    private WahleIdle idle;
    private WahleMove move;
    private WahleAttack attack;

    public static IEnumerator curState;
    public static WahleCtrl instance;

    void Awake()
    {
        instance = this;
        anim = gameObject.GetComponentInChildren<Animator>();
        camTr = GameObject.FindGameObjectWithTag("MainCamera").transform;
        playerTr = GameObject.Find("Luna_Head_Point").transform;
        fsmBase = GetComponent<FSMBase>();

        idle = GetComponent<WahleIdle>();
        move = GetComponent<WahleMove>();
        attack = GetComponent<WahleAttack>();
    }

    private void Start()
    {
        ChangeState(WahleState.IDLE);
        StartCoroutine(CoroutineUpdate());
        StartCoroutine(SearchEnemy());
    }

    protected virtual IEnumerator CurStateUpdate() { yield return null; }

    // 현재 상태 변경
    public void ChangeState(WahleState state)
    {
        switch (state)
        {
            case WahleState.IDLE:
                curState = idle.CurStateUpdate();
                break;
            case WahleState.MOVE:
                curState = move.CurStateUpdate();
                break;
            case WahleState.ATTACK:
                curState = attack.CurStateUpdate();
                break;
        }
    }

    // 현재 상태를 코루틴을 사용하여 Update 함
    public IEnumerator CoroutineUpdate()
    {
        while (true)
        {
            if (!curState.Equals(null) && curState.MoveNext())
            {
                yield return curState.Current;
            }
            else
                yield return null;
        }
    }

    void ChangeWahleSwitch()
    {
        if (isSwitch)
            ChangeState(WahleState.IDLE);
        else
            ChangeState(WahleState.SWITCH);

        isSwitch = !isSwitch;
    }

    // 플레이어가 발판에 올라갔을 시
    public IEnumerator StepHold()
    {
        while (true)
        {
            relativePos = (playerTr.position - transform.position); // 두 객체간의 거리 차
            lookRot = Quaternion.LookRotation(relativePos);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, lookRot, 5f * Time.deltaTime);
            // 선형 보간을 사용한 캐릭터 추격
            transform.position = Vector3.Lerp(transform.position, playerTr.position - (playerTr.forward),
                10f * Time.deltaTime);
            yield return null;
        }
    }

    // 주변 몬스터 탐색
    IEnumerator SearchEnemy()
    {
        float monsterDis = 0f;
        while (true)
        {
            Collider[] searchColl = Physics.OverlapSphere(playerTr.position, 20f);
            if (!isSearch)
            {
                for (int i = 0; i < searchColl.Length; i++)
                {
                    // 몬스터 탐색
                    if (searchColl[i].CompareTag("MONSTER"))
                    {
                        // 몬스터가 일정거리 안에 들어올 시 공격으로 전환
                        monsterDis = (searchColl[i].transform.position - playerTr.position).sqrMagnitude;
                        if (monsterDis <= 28f)
                        {
                            if (!searchColl[i].GetComponent<FSMBase>().isDeath)
                            {
                                // 현재 상태를 공격으러 전환, 몬스터 객체를 넘겨줌
                                ChangeState(WahleState.ATTACK);
                                attack.GetTarget(searchColl[i].gameObject);
                            }
                            break;
                        }
                    }
                    //NPC 탐색
                    else if (searchColl[i].CompareTag("NPC"))
                    {
                        npcPos = searchColl[i].transform.position;
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    // 이동 속도 증가
    public float IncrementSpeed(float initSpeed, float maxSpeed, float accel)
    {
        if (initSpeed.Equals(maxSpeed))
            return initSpeed;
        else {
            initSpeed += accel * Time.deltaTime;
            // 기본 속도가 최고 속도를 넘을 시 음수가 되어 maxSpeed만 반환
            return (Mathf.Sign(maxSpeed - initSpeed).Equals(1)) ? initSpeed : maxSpeed;
        }
    }

    // 이동 속도 감소
    public float DecreaseSpeed(float initSpeed, float minSpeed, float accel)
    {
        if (initSpeed.Equals(minSpeed))
            return initSpeed;
        else {
            initSpeed -= accel * Time.deltaTime;
        // 기본 속도가 최고 속도를 넘을 시 음수가 되어 maxSpeed만 반환
            return (Mathf.Sign(minSpeed - initSpeed).Equals(1)) ? minSpeed : initSpeed;
        }
    }

    // 대기 상태 시 랜덤으로 포인트 위치 이동
    protected Vector3 SetRandomPos()
    {
        float rndPointX = Random.Range(camTr.position.x - 4f, camTr.position.x + 2f);
        float rndPointY = Random.Range(playerTr.position.y, camTr.position.y + 1.3f);

        return new Vector3(rndPointX, rndPointY, playerTr.position.z);
    }

    // 레이캐스트 발사하여 지형 있을 시 회피
    protected virtual Vector3 ShotRay(Vector3 relativePos)
    {
        // 양측 레이 발사 위치
        Vector3 rightRayPos = transform.position + (transform.right * 0.3f);
        Vector3 leftRayPos = transform.position - (transform.right * 0.3f);

        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        // 우측 레이캐스트
        if (Physics.Raycast(rightRayPos, forward, out hit, 3f) || Physics.Raycast(leftRayPos, forward, out hit, 3f))
        {
            // 플레이어, 벽, 땅이 있을 시 우회
            if (hit.collider.CompareTag("WALL") || hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Player"))
            {
                return relativePos += hit.normal * 50f;
            }
            else
                return relativePos;
        }
        else
            return relativePos;
    }

    // 카메라 밖으로 나갔는지 확인
    protected virtual bool CheckOutCamera()
    {
        Vector3 camVec = Camera.main.WorldToScreenPoint(transform.position);
        // 화면 밖으로 나갔을 시 true (오른쪽 || 왼쪽) 
        if (camVec.x >= Camera.main.pixelWidth - 130f || camVec.x <= 130f)
            return true;
        else
            return false;
    }

    // 랜덤 값 생성
    public int GetRandomValue(float[] values)
    {
        float total = 0f;
        // 전체 합을 구함
        for (int i = 0; i < values.Length; i++)
        {
            total += values[i];
        }
        // 전체 합의 임이의 0~1의 변수를 곱함
        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < values.Length; i++)
        {
            if (randomPoint < values[i])
            {
                return i;
            }
            else {
                randomPoint -= values[i];
            }
        }
        return values.Length - 1;
    }
}