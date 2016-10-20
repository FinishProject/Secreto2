using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WahleState { IDLE, MOVE, ATTACK, SWITCH, MEET }

public class WahleCtrl : MonoBehaviour {
    public float maxSpeed = 10f; // 최대 속도
    public float accel = 5f; // 가속도
    protected float initSpeed = 0f; // 초기 속도
    protected float distance = 0f; // 거리 차
    protected bool isSearch = false; // 탐색 여부

    protected Vector3 npcPos; // NPC 위치를 저장할 변수
    protected Vector3 relativePos; // 상대적 위치값
    protected Quaternion lookRot; // 봐라볼 방향
    protected Transform playerTr; // 플레이어 위치

    private Transform camTr; // 카메라 위치
    protected Animator anim;

    private WahleIdle idle;
    private WahleMove move;
    private WahleMeet meet;

    public static IEnumerator curState;
    public static WahleCtrl instance;

    void Awake()
    {
        instance = this;
        anim = gameObject.GetComponentInChildren<Animator>();
        camTr = GameObject.FindGameObjectWithTag("MainCamera").transform;
        playerTr = GameObject.Find("Luna_Head_Point").transform;

        meet = GetComponent<WahleMeet>();
        idle = GetComponent<WahleIdle>();
        move = GetComponent<WahleMove>();
    }

    private void Start()
    {
        //curState = meet.CurStateUpdate();
        ChangeState(WahleState.MEET);
        StartCoroutine(CoroutineUpdate());
    }

    protected virtual IEnumerator CurStateUpdate() { yield return null; }

    // 현재 상태 변경
    public void ChangeState(WahleState state)
    {
        switch (state)
        {
            case WahleState.MEET:
                curState = meet.CurStateUpdate();
                break;
            case WahleState.IDLE:
                curState = idle.CurStateUpdate();
                break;
            case WahleState.MOVE:
                curState = move.CurStateUpdate();
                break;
        }
    }

    // 현재 상태를 코루틴을 사용하여 Update 함
    public IEnumerator CoroutineUpdate()
    {
        while (true)
        {
            if (!curState.Equals(null) && curState.MoveNext())
                yield return curState.Current;
            else
                yield return null;
        }
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
    protected virtual bool CheckOutCamera(Transform targetTr)
    {
        Vector3 camVec = Camera.main.WorldToScreenPoint(targetTr.position);
        // 화면 밖으로 나갔을 시 true (오른쪽 || 왼쪽) 
        if (camVec.x >= Camera.main.pixelWidth - 130f || camVec.x <= 0f)
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