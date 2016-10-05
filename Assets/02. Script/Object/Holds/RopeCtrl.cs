using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    로프
    플레이어는 이 스크립트가 추가된 오브젝트를 이용하여
    오르 내릴 수 있고
    좌우 반동을 이용하여 날아갈 수 있다.

    사용방법 :
    1. 로프의 뿌리가 될 오브젝트에 이 스크립트를 추가
    2. 이 스크립트 prefab 변수에 로프를 구성할 구슬 오브젝트를 삽입
    
    기타 사항은 코드 참조

*************************************************************/

public class RopeCtrl : MonoBehaviour {
    #region 변수 선언부
    [Tooltip("로프 구슬 프리펩")]
    public GameObject prefab;
    [Tooltip("로프 구슬 개수")]
    public int ropeCnt;
    [Tooltip("속도 제한 (50개 때 1.4)")]
    public float speedLimit = 1.4f;
    [System.NonSerialized]
    public bool isCtrlAuthority = false;        // 조작권한 (로프 조종)

    private Transform[] lowRopes;               // 로프 구슬 배열
    private float ropeScale;                    // 로프 구슬 하나의 스케일값

    private float L = 0;                        // 실 길이(m)
    private float stepSize = 0.5f;              // 스텝사이즈
    private float curResist = -0.5f;            // 현재 저항
    private float StopResist = -0.5f;           // 조작 중이지 않을 때
    private float CtrlResist = -0.1f;           // 조작 중일 때
    private float theta = 50 * Mathf.Deg2Rad;   // 각도
    private float theta1 = 0;
    private float theta2 = 0;
    private float pre_theta1 = 0;
    private float pre_theta2 = 0;

    private float addPower = 2.5f;              // 좌우 반동 때 주는 힘
    private bool isLimited = false;             // 힘의 제한
    private bool isLeft;                        // 좌우 방향 체크
    private int playerIdx;                      // 플레이어의 인덱스 위치
    #endregion


    #region 초기화
    void Start()
    {
        lowRopes = new Transform[ropeCnt];
        playerIdx = ropeCnt - 1;

        CreateRope();
        changeRopeRange(playerIdx);
        InitState(90.0f);

    }

    // 초기 설정
    void InitState(float getAngle)
    {
        stepSize = 0.025f;

        theta = getAngle * Mathf.Deg2Rad;
        theta1 = 0;
        theta2 = 0;
        pre_theta1 = 0;
        pre_theta2 = 0;
    }

    // 로프 생성
    void CreateRope()
    {
        for (int i = 0; i < ropeCnt; i++)
        {
            GameObject lowRope = Instantiate(prefab) as GameObject;
            lowRope.name = i.ToString();
            lowRope.tag = "Rope";
            lowRope.transform.parent = gameObject.transform;

            lowRopes[i] = lowRope.transform;

            Vector3 temp = gameObject.transform.position;
            if (i == 0)
            {
                temp.y = gameObject.transform.position.y - gameObject.transform.localScale.y * 0.5f;
                lowRopes[i].position = temp;
                ropeScale = lowRopes[i].localScale.y;
            }
            else
            {
                temp.y = lowRopes[i - 1].position.y - ropeScale * 0.5f;
                lowRopes[i].position = temp;
            }
        }
    }
    #endregion


    // Update is called once per frame
    void Update()
    {
        if (isCtrlAuthority)
        {
            curResist = CtrlResist;
            // 위로 올라감
            if (Input.GetKey(KeyCode.UpArrow))
            {
                changeRopeRange(--playerIdx);
            }
            // 아래로 내려감
            if (Input.GetKey(KeyCode.DownArrow))
            {
                changeRopeRange(++playerIdx);
            }
            // 오른쪽으로 힘을 줌 ( 좌우 반동 )
            if (Input.GetKey(KeyCode.RightArrow) && !isLimited && isLeft)
            {
                theta1 += addPower * Mathf.Deg2Rad;
            }
            // 왼쪽으로 힘을 줌 ( 좌우 반동 )
            if (Input.GetKey(KeyCode.LeftArrow) && !isLimited && !isLeft)
            {
                theta1 -= addPower * Mathf.Deg2Rad;       
            }
            // 날아감
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.X))
            {
                curResist = StopResist;
                isCtrlAuthority = false;
                //PlayerCtrl.instance.GetCtrlAuthorityByRope();  // 플레이어에게 조작 권한을 돌려줌
            }
        }
    }

    void FixedUpdate()
    {
        PendulumMove();
    }

    #region 물리 부분
    // 진자 운동
    void PendulumMove()
    {
        // 진자운동 구현
        theta2 = (Physics.gravity.y / L) * Mathf.Sin(theta);                               // 가속도
        theta1 = theta1 + (pre_theta2 * stepSize)                                          // 속도
             + ((theta2 - pre_theta2) * stepSize * 0.5f)
             - ((theta2 - pre_theta2) * curResist);
        theta = theta + (theta1 * stepSize) + ((theta1 - pre_theta1) * stepSize * 0.5f);   // 변위
        pre_theta2 = theta2;
        pre_theta1 = theta1;


        // 속도제한 ( 제한이 없으면 계속 회전 할 수 있음 / 각도 제한 보다 제어가 쉬움 )
        if (Mathf.Abs(theta1) > speedLimit)
            isLimited = true;
        else
            isLimited = false;

        // 조작 권한이 없을 때
        if (!isCtrlAuthority)
        {
            playerIdx = ropeCnt - 1;
            ChangeRot_2(0 , playerIdx, -(theta * Mathf.Rad2Deg * 0.5f));
        }
        else
        {
            ChangeRot(0, playerIdx, theta * Mathf.Rad2Deg);

            if (playerIdx != ropeCnt - 1)
                ChangeRot_2(playerIdx, ropeCnt - 1, theta * Mathf.Rad2Deg);
        }

       // 로프의 방향 체크
        if (Mathf.Sin(theta) < 0f)
            isLeft = true;
        else
            isLeft = false;
    }

    // 플레이어 위치에 따른 줄의 길이 (진자의 위치)
    void changeRopeRange(int idx)
    {
        if (idx >= ropeCnt)
        {
            playerIdx = ropeCnt - 1;
            return;
        }
        else if (idx <= 0)
        {
            playerIdx = 0;
            // 로프의 끝
            return;
        }

        playerIdx = idx;
        L = 2 * Vector3.Distance(lowRopes[0].transform.position, lowRopes[playerIdx].transform.position);
    }


    // 각도 조절 (시작, 끝, 각도)
    void ChangeRot(int startIdex, int endIdx, float rot)
    {
        Quaternion temp;
        for (int i = startIdex; i <= endIdx; i++)
        {
            temp = lowRopes[i].transform.rotation;
            temp.eulerAngles = new Vector3(0, 0, rot);
            lowRopes[i].rotation = temp;
            if (i == 0)
                continue;
            MovePos(i, rot);
        }
    }

    void ChangeRot_2(int startIdex, int endIdx, float rot)
    {
        Quaternion temp;
        float tempRot = rot;
        for (int i = startIdex; i <= endIdx; i++)
        {
            temp = lowRopes[i].transform.rotation;
            tempRot += tempRot / 40;
            temp.eulerAngles = new Vector3(0, 0, rot - tempRot);
            lowRopes[i].rotation = temp;
            if (i == 0)
                continue;
            MovePos(i, rot - tempRot);
        }
    }

    // 로프 움직이기 ( 각도에 맞춰서 꼬리를 물어야 하므로 )
    void MovePos(int index, float rot)
    {
        Vector3 temp = lowRopes[index - 1].transform.position;
        temp.x += Mathf.Sin(rot * Mathf.Deg2Rad) * (ropeScale * 0.5f);
        temp.y -= Mathf.Cos(rot * Mathf.Deg2Rad) * (ropeScale * 0.5f);

        lowRopes[index].transform.position = temp;
    }
    #endregion


    #region 플레이어와 상호작용

    //플레이어에서 조작권한을 받음
    public void setAuthority(int playerIdx, bool isFocusRight)
    {
        this.playerIdx = playerIdx; // 플레이어의 위치를 받아옴 (구슬 위치)
        playerIdx = ropeCnt - 1;

        isCtrlAuthority = true;     // 조작 권한을줌

        if (isFocusRight)           // 플레이어가 로프에 처음 탔을 때 주는 힘의 방향
            theta1 += 1f;
        else
            theta1 -= 1f;
    }

    // 플레이어와 근접한 로프 구슬의 위치 반환
    public Transform getLowRopeTransform()
    {
        if(playerIdx.Equals(0))
            return lowRopes[playerIdx].transform;
        else
            return lowRopes[playerIdx-1].transform;
    }

    // 각도 반환
    public float getRadian()
    {
        if (playerIdx.Equals(0))
            return lowRopes[playerIdx].transform.eulerAngles.z;
        else
            return lowRopes[playerIdx - 1].transform.eulerAngles.z;
    }

    // 속도 반환
    public float getSpeed()
    {
        return theta1 * L;
    }

    #endregion
}

