using UnityEngine;
using System.Collections;

/**************************************** 사용 방법 ********************************************

    캐릭터 좌우측 떨어진 곳에 레이 박스를 배치하여 앞에 있는 지형의 높이를 받아와
    미리 카메라의 높이를 높여준다
    
    ※ 사용 방법
    1. 오브젝트 구조
    MainCamera              <- 스크립트 추가
        └ ReyCube_R        <- 우측 방향으로
        └ ReyCube_L        <- 상단 판정 영역, (좌우 판정)
        └ ReyCube_Wall     <- 스크립트 추가
    
    2. 바닥에 "Land" 태그를 붙여야 카메라 위치를 미리 옮겨 줄 수 있다

    3. 공중에 떠 있는 발판의 경우( 아래 떨어지는 부분 )에는 보이지 않는 콜리더를 따로 박은 후
       위와 같이 "Land" 태그를 붙여주자, 자연스럽게 움직인다
       ( 발판에 "Land" 태그를 붙일 경우 다음 발판과의 갑작스러운 높이차이 때문에 끊기는 느낌이 있을것임 )

    4. 위 3번 콜리더 약간 밑에 "ChaseLine"태그를 가진 콜리더를 추가 해주자
       (3번 콜리더를 복사를 해서 1.3정도 아래에 배치)
       이는 떨어질때 카메라가 빨리 쫓아 가도록 해준다.

************************************************************************************************/

// 카메라 16_10_09
public class CameraCtrl_6 : MonoBehaviour
{
    public float traceYpos = 5f;    // 추격 시작할 Y차이 (땅과 캐릭터)
    public float speed_X_Max = 5f;  // X축 최대 추적 속도
    public float speed_Y_1 = 0.5f;  // Y축 범위 밖 추적 속도
    public float speed_Y_2 = 3f;    // Y축 범위 안 추적 속도
    public float rayRange = 20f;    // 좌우 레이의 길이
    float curSpeed_X;               // 현재 카메라의 x축 속도
    float wall_L_Pos_X;             // 오른쪽에 위치한 벽의 x 좌표
    float wall_R_Pos_X;             // 왼쪽에 위치한 벽의 x 좌표

    Transform tr, playerTr;         // 카메라, 플레이어의  transfrom
    public Transform rayBox_R;      // 오른쪽 레이 박스 Transform
    public Transform rayBox_L;      //   왼쪽 레이 박스 Transform
    public Transform rayBox_Wall;   // 벽체크 레이 박스 Transform


    Vector3 rayBox_R_addpos;        // 캐릭터 위치에 비례한 레이 박스의 위치 값 ( 오른쪽 )
    Vector3 rayBox_L_addpos;        // 캐릭터 위치에 비례한 레이 박스의 위치 값 ( 왼쪽 )

    Vector3 groundPos_Box_R;        // 오른쪽 레이 박스 위에 있는 땅 pos
    Vector3 groundPos_Box_L;        //   왼쪽 레이 박스 위에 있는 땅 pos
    Vector3 groundPos_Player;       // 플레이어가 위에 있는 땅 pos
    Vector3 wallCheckCenterPos;     // 벽을 체크할 레이를 쏠 위치값

    Vector3 baseCamPos;             // 카메라의 기본 위치 (x,z값은 유지, y 값은 지형 높이에 따라 변화 줌)
    Vector3 baseCamPos_Left;        // 왼쪽 방향 카메라 위치

    RaycastHit hit;                 // 충돌된 레이

    float correctionValue;          // 보정 수치 ( 레이 박스와 카메라간의 보정 수치 값)
    float wallRayToCamGap;          // 벽과 카메라의 거리차이
    float groundToCamYgap;          // 땅과 카메라의 거리차이

    bool isFalling_Before;          // 떨어지기 직전
    bool isFalling;                 // 떨어 지고 있는 중
    bool isFocusRight;              // 진행 방향
    bool oldFocusRight;             // 이전 진행 방향
    bool isNeerWall_Left;           // 왼쪽에 벽이 있으면
    bool isNeerWall_Right;          // 오른쪽에 벽이 있으면
    bool teleportTrigger;           // 텔레포트중인지

    public static CameraCtrl_6 instance;
    void Start()
    {
        instance = this;

        // 위치 초기화
        tr = transform;
        playerTr = PlayerCtrl.instance.transform;
        rayBox_R_addpos = rayBox_R.position - playerTr.position;
        rayBox_R_addpos.z = 0;
        rayBox_L_addpos = rayBox_L.position - playerTr.position;
        rayBox_L_addpos.z = 0;

        wallRayToCamGap = Mathf.Abs(tr.position.x - rayBox_Wall.position.x);

        // 카메라 기본위치 저장
        baseCamPos = tr.position - playerTr.position;
        baseCamPos_Left = baseCamPos;
        baseCamPos_Left.x = playerTr.position.x - tr.position.x;

        groundToCamYgap = baseCamPos.y - groundPos_Player.y;
        ChackGround_ByRay(playerTr, ref groundPos_Player.y);

        teleportTrigger = false;
    }



    #region Update 함수

    void FixedUpdate()
    {
        rayBox_R.position = playerTr.position + rayBox_R_addpos;
        rayBox_L.position = playerTr.position + rayBox_L_addpos;
    }

    void Update()
    {
        FocusChecker();             // 진행 방향 체크
        Speed_X_Ctrl();             // x축 속도조절

        // 레이 체크
        ChackGround_ByRay(playerTr, ref groundPos_Player.y);
        if (isFocusRight)
            ChackGround_ByRay(rayBox_R, ref groundPos_Box_R.y);
        else
            ChackGround_ByRay(rayBox_L, ref groundPos_Box_L.y);

        if(!teleportTrigger)
            ChackWall_ByRay();          // 주변에 벽이 있는지 체크

        CamCorrectionValue();       // 지형 보정값

        ChackChaseLine_ByRay();     // 하단 추락시 추적할 레이

        if (PlayerCtrl.controller.isGrounded)
            isFalling = false;
    }

    void LateUpdate()
    {
        Vector3 temp = tr.position;
        // 카메라의 x 좌표 움직임 ( 캐릭터가 바라보는 방향에 따라 ) 
        if (isFocusRight)
            temp = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, 0, playerTr.position.z) + baseCamPos, curSpeed_X * Time.deltaTime);
        else
            temp = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, 0, playerTr.position.z) + baseCamPos_Left, curSpeed_X * Time.deltaTime);

        // 우측에 벽이 있으면 ( else if는 좌측에 벽이 있으면 )
        if (isNeerWall_Right && wall_R_Pos_X - wallRayToCamGap < temp.x)
            temp.x = wall_R_Pos_X - wallRayToCamGap;
        else if (isNeerWall_Left && wall_L_Pos_X + wallRayToCamGap > temp.x)
            temp.x = wall_L_Pos_X + wallRayToCamGap;


        // 카메라의 y 좌표 움직임
        // 플레이어 위치 > 추격할 거리 + 땅과의 거리
        if (playerTr.position.y > traceYpos + groundPos_Player.y)
            temp.y = Mathf.Lerp(tr.position.y, traceYpos + groundPos_Player.y + baseCamPos.y + correctionValue + shakeVal.y, speed_Y_1 * Time.deltaTime);

        //추락할 때
        else if (isFalling)
            temp.y = Mathf.Lerp(tr.position.y, baseCamPos.y + playerTr.position.y + shakeVal.y , Vector3.Distance(playerTr.position, tr.position) * Time.deltaTime);

        //그 외
        else
            temp.y = Mathf.Lerp(tr.position.y, baseCamPos.y + groundPos_Player.y + correctionValue + shakeVal.y, speed_Y_2 * Time.deltaTime);

        // 텔레포트를 이용 할 때
        if (!teleportTrigger)
            tr.position = temp;
    }
    #endregion




    #region 카메라 이동 관련 함수
    // x축 속도 가속 (방향이 바뀔 시 바로 바뀌면 이상하니)
    void Speed_X_Ctrl()
    {
        if (curSpeed_X < speed_X_Max)
        {
            curSpeed_X += 5 * Time.deltaTime;
        }
    }

    // 진행 방향 체크
    void FocusChecker()
    {
        isFocusRight = PlayerCtrl.isFocusRight;
        // 방향이 바뀌었으면
        if (oldFocusRight != isFocusRight)
        {
            curSpeed_X = 0.3f;
        }
        oldFocusRight = isFocusRight;
    }


    // 지형 높이 보정을 해준다
    void CamCorrectionValue()
    {
        if (isFocusRight)
            correctionValue = (groundPos_Box_R.y - groundPos_Player.y) * 0.5f;
        else
            correctionValue = (groundPos_Box_L.y - groundPos_Player.y) * 0.5f;
    }
    #endregion




    #region 레이 캐스트
    RaycastHit[] hits;
    void ChackGround_ByRay(Transform objTr, ref float posY)
    {
        Debug.DrawRay(objTr.position, -Vector3.up * 30, Color.yellow);
        hits = Physics.RaycastAll(objTr.position, -Vector3.up, 30);

        // Ignore를 가진 오브젝트가 있으면 예외 처리
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.transform.CompareTag("Ignore"))
            {
                return;
            }
        }

        // 레이를 쏴서 충돌한 지형들의 Y좌표를 저장 ( 지형이 겹쳐 있으면 여러개 잡히기 때문)
        float[] tempPosY = new float[hits.Length];
        int idx = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("Land"))
            {
                tempPosY[idx++] = hit.point.y;
                posY = hit.point.y;
            }
        }

        float tempVal;
        float approx = 999f; // 근사치 값 ( 낮을 수록 근사 값)
        int approxIdx = 0;

        // 플레이어의 위치와 가까운 Y축 좌표를 찾음
        for (int i = idx; i > 0; i--)
        {
            tempVal = Mathf.Abs(playerTr.position.y - tempPosY[i - 1]);
            if (approx > tempVal)
            {
                approx = tempVal;
                approxIdx = i - 1;
            }
        }
        if (approx != 999f)
            posY = tempPosY[approxIdx];
    }
        // 기준 위치에서 레이를 쏴서 충동한 바닥의 위치를 알기위한 함수
        /*
        void ChackGround_ByRay(Transform objTr, ref float posY)
        {
            Debug.DrawRay(objTr.position, -Vector3.up * rayRange, Color.yellow);
            hits = Physics.RaycastAll(objTr.position, -Vector3.up, rayRange);

            // Ignore를 가진 오브젝트가 있으면 예외 처리
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.transform.CompareTag("Ignore"))
                {
                    return;
                }
            }

            // 레이를 쏴서 충돌한 지형들의 Y좌표를 저장 ( 지형이 겹쳐 있으면 여러개 잡히기 때문)
            float[] tempPosY = new float[hits.Length];
            int idx = 0;
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                if (hit.transform.CompareTag("Land"))
                {
                    tempPosY[idx++] = hit.point.y;
                    posY = hit.point.y;
                }
            }

            // 레이를 쏘는 위치 기준 제일 높은 위치에 있는 지형(Land)을 기준으로 Y축 위치를 저장하기 위함
            float maxY = 0;
            for (int i = idx; i > 0; i--)
            {
                if (maxY < tempPosY[i - 1])
                    maxY = tempPosY[i - 1];
            }

            if (maxY != 0)
                posY = maxY;

        }
        */

        // 추락시 카메라 추적할 때 필요한 콜리더 체크 ( 기본적으로 지형의 높이에 따른 카메라라서 빠르게 추적하기 위해 이런 콜리더가 필요함 )
    void ChackChaseLine_ByRay()
    {
        Debug.DrawRay(playerTr.position, -Vector3.up * 0.5f, Color.red);
        hits = Physics.RaycastAll(playerTr.position, -Vector3.up, 0.5f);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("ChaseLine"))
            {
                isFalling = true;
                break;
            }
        }
    }


    // 벽 충돌 체크
    void ChackWall_ByRay()
    {
        wallCheckCenterPos = tr.position;
        wallCheckCenterPos.z = playerTr.position.z;
        // 오른쪽에 벽이 있는지 체크
        Debug.DrawRay(wallCheckCenterPos, Vector3.right * wallRayToCamGap, Color.magenta);
        hits = Physics.RaycastAll(wallCheckCenterPos, Vector3.right, wallRayToCamGap);

        if (hits.Length == 0)
            isNeerWall_Right = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("WALL"))
            {
                wall_R_Pos_X = hit.point.x;
                isNeerWall_Right = true;
                break;
            }
            else
                isNeerWall_Right = false;
        }

        // 왼쪽에 벽이 있는지 체크
        Debug.DrawRay(wallCheckCenterPos, -Vector3.right * wallRayToCamGap, Color.cyan);
        hits = Physics.RaycastAll(wallCheckCenterPos, -Vector3.right, wallRayToCamGap);

        if (hits.Length == 0)
            isNeerWall_Left = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("WALL"))
            {
                wall_L_Pos_X = hit.point.x;
                isNeerWall_Left = true;
                break;
            }
            else
                isNeerWall_Left = false;
        }

    }

    #endregion

    public GameObject shakeObject;
    Vector3 shakeVal;
    bool isShaking;
    
    public void ShakeStart(Transform objTr, float startShakeRange)
    {
        StartCoroutine(Shake(objTr, startShakeRange));
    }

    IEnumerator Shake(Transform objTr, float startShakeRange)
    {
        shakeVal = new Vector3(0, 0.3f, 0);
        float range = Vector3.Distance(objTr.position, playerTr.position);
        Debug.Log(2);
        while (range < startShakeRange)
        {
            Debug.Log(1);
            range = Vector3.Distance(objTr.position, playerTr.position);
            shakeVal.y *= -1f;
            yield return new WaitForSeconds((range/startShakeRange)*0.2f);
        }
        shakeVal = new Vector3(0, 0, 0);
    }


    #region 포탈 관련 함수
    public void StartTeleport()
    {
        teleportTrigger = true;
    }
    public void EndTeleport(bool isRight)
    {
        if(isRight)
            tr.position = playerTr.position + baseCamPos;
        else
            tr.position = playerTr.position + baseCamPos_Left;

        teleportTrigger = false;
    }
    #endregion

}

// 카메라 16_10_07
/*
public class CameraCtrl_6 : MonoBehaviour
{
    public float traceYpos = 5f;    // 추격 시작할 Y차이 (땅과 캐릭터)
    public float speed_X_Max = 5f;  // X축 최대 추적 속도
    public float speed_Y_1 = 0.5f;  // Y축 범위 밖 추적 속도
    public float speed_Y_2 = 3f;    // Y축 범위 안 추적 속도
    public float speed_Y_3 = 3f;    // Y축 범위 안 추적 속도
    public float fallStandardRange = 7f;
    public float rayRange = 20f;
    float curSpeed_X;               // 현재 카메라의 x축 속도
    float wall_L_Pos_X;             // 오른쪽에 위치한 벽의 x 좌표
    float wall_R_Pos_X;             // 왼쪽에 위치한 벽의 x 좌표

    Transform tr, playerTr;         // 카메라, 플레이어의  transfrom
    public Transform rayBox_R;      // 레이 박스 Transform
    public Transform rayBox_L;
    public Transform rayBox_Rn;
    public Transform rayBox_Ln;
    public Transform rayBox_Wall;

    // 캐릭터 위치에 비례한 레이 박스의 위치 값
    Vector3 rayBox_R_addpos;
    Vector3 rayBox_L_addpos;
    Vector3 rayBox_Rn_addpos;
    Vector3 rayBox_Ln_addpos;

    Vector3 wallCheckCenterPos;     // 레이를 쏠 위치값

    // 레이 박스 위에 있는 땅 pos
    Vector3 groundPos_Box_R;
    Vector3 groundPos_Box_L;
    Vector3 groundPos_Box_Rn;
    Vector3 groundPos_Box_Ln;
    Vector3 groundPos_Player;       // 플레이어가 위에 있는 땅 pos

    Vector3 baseCamPos;             // 카메라의 기본 위치 (x,z값은 유지, y 값은 지형 높이에 따라 변화 줌)
    Vector3 baseCamPos_Left;        // 왼쪽 방향 카메라 위치

    RaycastHit hit;                 // 충돌된 레이

    float correctionValue;          // 보정 수치 ( 레이 박스와 카메라간의 보정 수치 값)
    float wallRayToCamgap;          // 벽과 카메라의 거리차이
    float groundToCamYgap;          // 땅과 카메라의 거리차이

    bool isFalling_Before;          // 떨어지기 직전
    bool isFalling;                 // 떨어 지고 있는 중
    bool isFocusRight;              // 진행 방향
    bool oldFocusRight;             // 이전 진행 방향
    bool isNeerWall_Left;           // 왼쪽에 벽이 있으면
    bool isNeerWall_Right;          // 오른쪽에 벽이 있으면
    bool teleportTrigger;           // 텔레포트중인지

    public static CameraCtrl_6 instance;
    void Start()
    {
        instance = this;

        // 위치 초기화
        tr = transform;
        playerTr = PlayerCtrl.instance.transform;
        rayBox_R_addpos = rayBox_R.position - playerTr.position;
        rayBox_R_addpos.z = 0;
        rayBox_L_addpos = rayBox_L.position - playerTr.position;
        rayBox_L_addpos.z = 0;
        rayBox_Rn_addpos = rayBox_Rn.position - playerTr.position;
        rayBox_Rn_addpos.z = 0;
        rayBox_Ln_addpos = rayBox_Ln.position - playerTr.position;
        rayBox_Ln_addpos.z = 0;

        wallRayToCamgap = Mathf.Abs(tr.position.x - rayBox_Wall.position.x);

        // 카메라 기본위치 저장
        baseCamPos = tr.position - playerTr.position;
        baseCamPos_Left = baseCamPos;
        baseCamPos_Left.x = playerTr.position.x - tr.position.x;

        groundToCamYgap = baseCamPos.y - groundPos_Player.y;
        ChackGround_ByRay(playerTr, ref groundPos_Player.y);

        teleportTrigger = false;
    }



    #region Update 함수

    void FixedUpdate()
    {
        rayBox_R.position = playerTr.position + rayBox_R_addpos;
        rayBox_L.position = playerTr.position + rayBox_L_addpos;
        rayBox_Rn.position = playerTr.position + rayBox_Rn_addpos;
        rayBox_Ln.position = playerTr.position + rayBox_Ln_addpos;
    }

    void Update()
    {
        FocusChecker();             // 진행 방향 체크
        Speed_X_Ctrl();             // x축 속도조절

        // 레이 체크
        ChackGround_ByRay(playerTr, ref groundPos_Player.y);
        if (isFocusRight)
        {
            ChackGround_ByRay(rayBox_R, ref groundPos_Box_R.y);
            ChackGround_ByRay(rayBox_Rn, ref groundPos_Box_Rn.y);
        }
        else
        {
            ChackGround_ByRay(rayBox_L, ref groundPos_Box_L.y);
            ChackGround_ByRay(rayBox_Ln, ref groundPos_Box_Ln.y);
        }
        ChackWall_ByRay();

        Debug.Log(isNeerWall_Left + " : " + isNeerWall_Right);
        CamCorrectionValue();
    }

    void LateUpdate()
    {
        Vector3 temp = tr.position;
        // 카메라의 x 좌표 움직임 ( 캐릭터가 바라보는 방향에 따라 ) 

        if (isFocusRight)
            temp = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, 0, playerTr.position.z) + baseCamPos, curSpeed_X * Time.deltaTime);
        else
            temp = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, 0, playerTr.position.z) + baseCamPos_Left, curSpeed_X * Time.deltaTime);

        if (isNeerWall_Right && isFocusRight)
        {
            temp.x = wall_R_Pos_X - wallRayToCamgap;
        }

        // 카메라의 y 좌표 움직임
        // 플레이어 위치 > 추격할 거리 + 땅과의 거리
        if (playerTr.position.y > traceYpos + groundPos_Player.y)
            temp.y = Mathf.Lerp(tr.position.y, traceYpos + groundPos_Player.y + baseCamPos.y + correctionValue, speed_Y_1 * Time.deltaTime);

        // 추락할 때
        else if (isFalling)
            temp.y = Mathf.Lerp(tr.position.y, baseCamPos.y + playerTr.position.y, speed_Y_3 - Vector3.Distance(playerTr.position, tr.position) * Time.deltaTime);

        // 그 외
        else
            temp.y = Mathf.Lerp(tr.position.y, baseCamPos.y + groundPos_Player.y + correctionValue, (isFalling_Before ? speed_Y_3 : speed_Y_2) * Time.deltaTime);

        // 텔레포트를 이용 할 때
        if (!teleportTrigger)
            tr.position = temp;
    }
    #endregion




    #region 카메라 이동 관련 함수
    // x축 속도 가속 (방향이 바뀔 시 바로 바뀌면 이상하니)
    void Speed_X_Ctrl()
    {
        if (curSpeed_X < speed_X_Max)
        {
            curSpeed_X += 5 * Time.deltaTime;
        }
    }

    // 진행 방향 체크
    void FocusChecker()
    {
        isFocusRight = PlayerCtrl.isFocusRight;
        // 방향이 바뀌었으면
        if (oldFocusRight != isFocusRight)
        {
            curSpeed_X = 0.3f;
        }
        oldFocusRight = isFocusRight;
    }


    float tempRangeR;
    float tempRangeRn;
    float tempRangeL;
    float tempRangeLn;
    void CamCorrectionValue()
    {
        tempRangeR = Mathf.Abs(groundPos_Box_R.y - groundPos_Player.y);
        tempRangeRn = Mathf.Abs(groundPos_Box_Rn.y - groundPos_Player.y);
        tempRangeL = Mathf.Abs(groundPos_Box_L.y - groundPos_Player.y);
        tempRangeLn = Mathf.Abs(groundPos_Box_Ln.y - groundPos_Player.y);

        if (isFocusRight)
        {
            if (tempRangeR < fallStandardRange)  // 추락 기준값과 비교, 일반 추적
            {
                isFalling_Before = false;
                correctionValue = (groundPos_Box_R.y - groundPos_Player.y) * 0.5f;
            }
            else if (tempRangeRn > fallStandardRange)
            {
                isFalling_Before = true;
                correctionValue = (groundPos_Box_Rn.y - groundPos_Player.y) * 0.5f;
            }
            else
                isFalling_Before = false;
        }
        else
        {
            if (tempRangeL < fallStandardRange)  // 추락 기준값과 비교, 일반 추적
            {
                isFalling_Before = false;
                correctionValue = (groundPos_Box_L.y - groundPos_Player.y) * 0.3f;
            }
            else if (tempRangeLn > fallStandardRange)
            {
                isFalling_Before = true;
                correctionValue = (groundPos_Box_Ln.y - groundPos_Player.y) * 0.2f;
            }
            else
                isFalling_Before = false;
        }


        // 플레이어와 땅의 거리를 비교, 추락할 위치 인지 체크
        if (Mathf.Abs(playerTr.position.y - groundPos_Player.y) > fallStandardRange)
            isFalling = true;
        else if (PlayerCtrl.controller.isGrounded)
            isFalling = false;

    }
    #endregion





    #region 레이 캐스트
    RaycastHit[] hits;
    void ChackGround_ByRay(Transform objTr, ref float posY)
    {
        Debug.DrawRay(objTr.position, -Vector3.up * rayRange, Color.yellow);
        hits = Physics.RaycastAll(objTr.position, -Vector3.up, rayRange);

        // Ignore를 가진 오브젝트가 있으면 예외 처리
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.transform.CompareTag("Ignore"))
            {
                return;
            }
        }

        // 레이를 쏴서 충돌한 지형들의 Y좌표를 저장 ( 지형이 겹쳐 있으면 여러개 잡히기 때문)
        float[] tempPosY = new float[hits.Length];
        int idx = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("Land"))
            {
                tempPosY[idx++] = hit.point.y;
                posY = hit.point.y;
            }
        }

        // 레이를 쏘는 위치 기준 제일 높은 위치에 있는 지형(Land)을 기준으로 Y축 위치를 저장하기 위함
        float maxY = 0;
        for (int i = idx; i > 0; i--)
        {
            if (maxY < tempPosY[i - 1])
                maxY = tempPosY[i - 1];
        }

        if (maxY != 0)
            posY = maxY;

    }

    // 벽 충돌 체크
    void ChackWall_ByRay()
    {
        wallCheckCenterPos = tr.position;
        wallCheckCenterPos.z = playerTr.position.z;
        // 오른쪽에 벽이 있는지 체크
        Debug.DrawRay(wallCheckCenterPos, Vector3.right * wallRayToCamgap, Color.yellow);
        hits = Physics.RaycastAll(wallCheckCenterPos, Vector3.right, wallRayToCamgap);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("WALL"))
            {
                wall_R_Pos_X = hit.point.x;
                isNeerWall_Right = true;
                break;
            }
            else
                isNeerWall_Right = false;
        }

        // 왼쪽에 벽이 있는지 체크
        Debug.DrawRay(wallCheckCenterPos, Vector3.left * wallRayToCamgap, Color.yellow);
        hits = Physics.RaycastAll(wallCheckCenterPos, Vector3.left, wallRayToCamgap);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("WALL"))
            {
                wall_L_Pos_X = hit.point.x;
                isNeerWall_Left = true;
                break;
            }
            else
                isNeerWall_Left = false;
        }

    }
    #endregion





    #region 포탈 관련 함수
    public void StartTeleport()
    {
        teleportTrigger = true;
    }
    public void EndTeleport()
    {
        tr.position = playerTr.position + baseCamPos;
        teleportTrigger = false;
    }
    #endregion




}
*/

//카메라 16_10_05 이후
/*
public class CameraCtrl_6 : MonoBehaviour
{
    public float traceYpos = 3f;    // 추격 시작할 Y차이 (땅과 캐릭터)
    public float speed_X_Max = 5f;  // X축 추적 속도
    public float speed_Y = 2.4f;    // Y축 범위 안 추적 속도
    float curSpeed_X;


    // 레이 박스 Transform
    public Transform rayBox_R;
    public Transform rayBox_L;
    public Transform rayBox_Rn;
    public Transform rayBox_Ln;

    // 캐릭터 위치에 비례한 레이 박스의 위치 값
    Vector3 rayBox_R_addpos;
    Vector3 rayBox_L_addpos;
    Vector3 rayBox_Rn_addpos;
    Vector3 rayBox_Ln_addpos;


    float correctionValue;          // 보정 수치 ( 레이 박스와 카메라간의 보정 수치 값)

    Transform tr, playerTr;
    Vector3 groundPos_Player;       // 플레이어가 위에 있는 땅 pos
    Vector3 groundPos_Box_R;        // 레이 박스 위에 있는 땅 pos
    Vector3 groundPos_Box_L;
    Vector3 groundPos_Box_Rn;
    Vector3 groundPos_Box_Ln;

    Vector3 baseCamPos;             // 카메라의 기본 위치 (x,z값은 유지, y 값은 지형 높이에 따라 변화 줌)
    Vector3 baseCamPos_Left;

    float groundToCamYgap;          // 땅과 카메라의 거리차이
    bool isFocusRight;              // 진행 방향
    bool oldFocusRight;

    RaycastHit[] hits;
    RaycastHit hit;

    bool teleportTrigger;

    public static CameraCtrl_6 instance;
    void Start()
    {
        instance = this;
        tr = transform;
        playerTr = PlayerCtrl.instance.transform;
        rayBox_R_addpos = rayBox_R.position - playerTr.position;
        rayBox_R_addpos.z = 0;
        rayBox_L_addpos = rayBox_L.position - playerTr.position;
        rayBox_L_addpos.z = 0;
        rayBox_Rn_addpos = rayBox_Rn.position - playerTr.position;
        rayBox_Rn_addpos.z = 0;
        rayBox_Ln_addpos = rayBox_Ln.position - playerTr.position;
        rayBox_Ln_addpos.z = 0;

        baseCamPos = tr.position - playerTr.position;
        baseCamPos_Left = baseCamPos;
        baseCamPos_Left.x = playerTr.position.x - tr.position.x;


        groundToCamYgap = baseCamPos.y - groundPos_Player.y;
        ChackGround(playerTr, ref groundPos_Player.y);

        teleportTrigger = false;
    }

    float tempRangeR;
    float tempRangeRn;
    float tempRangeL;
    float tempRangeLn;

    void CamCorrectionValue()
    {
        tempRangeR = Mathf.Abs(groundPos_Box_R.y - groundPos_Player.y);
        tempRangeRn = Mathf.Abs(groundPos_Box_Rn.y - groundPos_Player.y);
        tempRangeL = Mathf.Abs(groundPos_Box_L.y - groundPos_Player.y);
        tempRangeLn = Mathf.Abs(groundPos_Box_Ln.y - groundPos_Player.y);

        if (isFocusRight)
        {
            correctionValue = (groundPos_Box_R.y - groundPos_Player.y) * 0.5f;
        }
        else
        {
            correctionValue = (groundPos_Box_Ln.y - groundPos_Player.y) * 0.5f;
        }

    }

    void Speed_X_Ctrl()
    {
        if (curSpeed_X < speed_X_Max)
        {
            curSpeed_X += 5 * Time.deltaTime;
        }
    }

    void FocusChecker()
    {
        isFocusRight = PlayerCtrl.isFocusRight;
        // 방향이 바뀌었으면
        if (oldFocusRight != isFocusRight)
        {
            curSpeed_X = 0.3f;
        }
        oldFocusRight = isFocusRight;
    }

    void FixedUpdate()
    {
        rayBox_R.position = playerTr.position + rayBox_R_addpos;
        rayBox_L.position = playerTr.position + rayBox_L_addpos;
        rayBox_Rn.position = playerTr.position + rayBox_Rn_addpos;
        rayBox_Ln.position = playerTr.position + rayBox_Ln_addpos;
    }


    void Update()
    {
        FocusChecker();             // 진행 방향 체크
        Speed_X_Ctrl();

        ChackGround(playerTr, ref groundPos_Player.y);      // 플레이어 <-> 땅 거리 체크

        // 진행 방향 앞쪽에서 레이를 쏴줌
        if (isFocusRight)
        {
            ChackGround(rayBox_R, ref groundPos_Box_R.y);
            ChackGround(rayBox_Rn, ref groundPos_Box_Rn.y);
        }
        else
        {
            ChackGround(rayBox_L, ref groundPos_Box_L.y);
            ChackGround(rayBox_Ln, ref groundPos_Box_Ln.y);
        }

        CamCorrectionValue();
    }

    // 카메라 움직임 적용
    void LateUpdate()
    {
        Vector3 temp = tr.position;
        if (isFocusRight)
            temp = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, 0, playerTr.position.z) + baseCamPos, curSpeed_X * Time.deltaTime);
        else
            temp = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, 0, playerTr.position.z) + baseCamPos_Left, curSpeed_X * Time.deltaTime);

        temp.y = Mathf.Lerp(tr.position.y, baseCamPos.y + groundPos_Player.y + correctionValue, speed_Y * Time.deltaTime);

        if (!teleportTrigger)
            tr.position = temp;
    }


    // 레이를 아래로 쏴서 땅의 위치 찾음
    void ChackGround(Transform objTr, ref float posY)
    {
        Debug.DrawRay(objTr.position, -Vector3.up * 15, Color.yellow);
        hits = Physics.RaycastAll(objTr.position, -Vector3.up, 15);

        // Ignore를 가진 오브젝트가 있으면 예외 처리
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.transform.CompareTag("Ignore"))
                return;
        }


        // 레이를 쏴서 충돌한 지형들의 Y좌표를 저장 ( 지형이 겹쳐 있으면 여러개 잡히기 때문)
        float[] tempPosY = new float[hits.Length];
        int idx = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("Land"))
            {
                tempPosY[idx++] = hit.point.y;
                posY = hit.point.y;
            }
        }


        // 레이를 쏘는 위치 기준 제일 높은 위치에 있는 지형(Land)을 기준으로 Y축 위치를 저장하기 위함
        float maxY = 0;
        for (int i = idx; i > 0; i--)
        {
            if (maxY < tempPosY[i - 1])
                maxY = tempPosY[i - 1];
        }

        if (maxY != 0)
            posY = maxY;


    }

    public void StartTeleport()
    {
        teleportTrigger = true;
    }
    public void EndTeleport()
    {
        Vector3 temp;
        if (isFocusRight)
        {
            temp = playerTr.position + baseCamPos;
            temp.y = baseCamPos.y + groundPos_Player.y + correctionValue;
            tr.position = temp;
        }
        else
        {
            temp = playerTr.position + baseCamPos_Left;
            temp.y = baseCamPos_Left.y + groundPos_Player.y + correctionValue;
            tr.position = temp;
        }

        teleportTrigger = false;
    }

    // 근사값에 따라 우선순위 체크
    
    //void ChackGround(Transform objTr, ref float posY)
    //{
    //    Debug.DrawRay(objTr.position, -Vector3.up * 15, Color.yellow);
    //    hits = Physics.RaycastAll(objTr.position, -Vector3.up, 15);

    //    // Ignore를 가진 오브젝트가 있으면 예외 처리
    //    for (int i = 0; i < hits.Length; i++)
    //    {
    //        RaycastHit hit = hits[i];
    //        if (hit.transform.CompareTag("Ignore"))
    //        {
    //            return;
    //        }
    //    }

    //    // 레이를 쏴서 충돌한 지형들의 Y좌표를 저장 ( 지형이 겹쳐 있으면 여러개 잡히기 때문)
    //    float[] tempPosY = new float[hits.Length];
    //    int idx = 0;
    //    for (int i = 0; i < hits.Length; i++)
    //    {
    //        RaycastHit hit = hits[i];

    //        if (hit.transform.CompareTag("Land"))
    //        {
    //            tempPosY[idx++] = hit.point.y;
    //            posY = hit.point.y;
    //        }
    //    }


       
    //    float tempVal;
    //    float approx = 999f; // 근사치 값 ( 낮을 수록 근사 값)
    //    int approxIdx = 0;

    //    // 플레이어의 위치와 가까운 Y축 좌표를 찾음
    //    for (int i = idx; i > 0; i--)
    //    {
    //        tempVal = Mathf.Abs(playerTr.position.y - tempPosY[i - 1]);
    //        if (approx > tempVal)
    //        {
    //            approx = tempVal;
    //            approxIdx = i - 1;
    //        }
    //    }
    //    if (approx != 999f)
    //        posY = tempPosY[approxIdx];
    //}
    
}
*/

// 카메라 16_10_05 이전
/*
public class CameraCtrl_6 : MonoBehaviour
{
    public float traceYpos = 5f;    // 추격 시작할 Y차이 (땅과 캐릭터)
    public float speed_X = 5f;      // X축 추적 속도
    public float speed_Y_1 = 0.5f;  // Y축 범위 밖 추적 속도
    public float speed_Y_2 = 3f;    // Y축 범위 안 추적 속도
    public float speed_Y_3 = 3f;    // Y축 범위 안 추적 속도
    public float fallStandardRange = 7f;


    public Transform rayBox_R;      // 레이 박스 Transform
    public Transform rayBox_L;
    public Transform rayBox_Rn;
    public Transform rayBox_Ln;
    //    public Transform rayBox;


    Vector3 rayBox_R_addpos;        // 캐릭터 위치에 비례한 레이 박스의 위치 값
    Vector3 rayBox_L_addpos;
    Vector3 rayBox_Rn_addpos;
    Vector3 rayBox_Ln_addpos;


    float correctionValue;          // 보정 수치 ( 레이 박스와 카메라간의 보정 수치 값)

    Transform tr, playerTr;
    Vector3 groundPos_Player;       // 플레이어가 위에 있는 땅 pos
    Vector3 groundPos_Box_R;        // 레이 박스 위에 있는 땅 pos
    Vector3 groundPos_Box_L;
    Vector3 groundPos_Box_Rn;
    Vector3 groundPos_Box_Ln;

    Vector3 baseCamPos;             // 카메라의 기본 위치 (x,z값은 유지, y 값은 지형 높이에 따라 변화 줌)

    float groundToCamYgap;          // 땅과 카메라의 거리차이
    bool isFalling_Before;
    bool isFalling;
    bool isFocusRight;              // 진행 방향

    RaycastHit hit;

    bool teleportTrigger;

    public static CameraCtrl_6 instance;
    void Start()
    {
        instance = this;
        tr = transform;
        playerTr = PlayerCtrl.instance.transform;
        rayBox_R_addpos = rayBox_R.position - playerTr.position;
        rayBox_R_addpos.z = 0;
        rayBox_L_addpos = rayBox_L.position - playerTr.position;
        rayBox_L_addpos.z = 0;
        rayBox_Rn_addpos = rayBox_Rn.position - playerTr.position;
        rayBox_Rn_addpos.z = 0;
        rayBox_Ln_addpos = rayBox_Ln.position - playerTr.position;
        rayBox_Ln_addpos.z = 0;


        baseCamPos = tr.position - playerTr.position;

        groundToCamYgap = baseCamPos.y - groundPos_Player.y;
        ChackGround(playerTr, ref groundPos_Player.y);

        teleportTrigger = false;
    }

    float tempRangeR;
    float tempRangeRn;
    float tempRangeL;
    float tempRangeLn;

    void CamCorrectionValue()
    {
        tempRangeR = Mathf.Abs(groundPos_Box_R.y - groundPos_Player.y);
        tempRangeRn = Mathf.Abs(groundPos_Box_Rn.y - groundPos_Player.y);
        tempRangeL = Mathf.Abs(groundPos_Box_L.y - groundPos_Player.y);
        tempRangeLn = Mathf.Abs(groundPos_Box_Ln.y - groundPos_Player.y);

        if (isFocusRight)
        {
            if (tempRangeR < fallStandardRange)  // 추락 기준값과 비교, 일반 추적
            {
                isFalling_Before = false;
                correctionValue = (groundPos_Box_R.y - groundPos_Player.y) * 0.5f;
            }
            else if (tempRangeRn > fallStandardRange)
            {
                isFalling_Before = true;
                correctionValue = (groundPos_Box_Rn.y - groundPos_Player.y) * 0.5f;
            }
            else
                isFalling_Before = false;
        }
        else
        {
            if (tempRangeL < fallStandardRange)  // 추락 기준값과 비교, 일반 추적
            {
                isFalling_Before = false;
                correctionValue = (groundPos_Box_L.y - groundPos_Player.y) * 0.3f;
            }
            else if (tempRangeLn > fallStandardRange)
            {
                isFalling_Before = true;
                correctionValue = (groundPos_Box_Ln.y - groundPos_Player.y) * 0.2f;
            }
            else
                isFalling_Before = false;
        }


        // 플레이어와 땅의 거리를 비교, 추락할 위치 인지 체크
        if (Mathf.Abs(playerTr.position.y - groundPos_Player.y) > fallStandardRange)
        {
            isFalling = true;
        }
        else if (PlayerCtrl.controller.isGrounded)
            isFalling = false;

    }

    void FixedUpdate()
    {
        rayBox_R.position = playerTr.position + rayBox_R_addpos;
        rayBox_L.position = playerTr.position + rayBox_L_addpos;
        rayBox_Rn.position = playerTr.position + rayBox_Rn_addpos;
        rayBox_Ln.position = playerTr.position + rayBox_Ln_addpos;
    }

    void Update()
    {
        isFocusRight = PlayerCtrl.isFocusRight;

        ChackGround(playerTr, ref groundPos_Player.y);
        if (isFocusRight)
        {
            ChackGround(rayBox_R, ref groundPos_Box_R.y);
            ChackGround(rayBox_Rn, ref groundPos_Box_Rn.y);
        }
        else
        {
            ChackGround(rayBox_L, ref groundPos_Box_L.y);
            ChackGround(rayBox_Ln, ref groundPos_Box_Ln.y);
        }
        CamCorrectionValue();
    }

    void LateUpdate()
    {
        Vector3 temp = tr.position;
        temp = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, 0, playerTr.position.z) + baseCamPos, speed_X * Time.deltaTime);

        if (playerTr.position.y > traceYpos + groundPos_Player.y)  // 플레이어 위치 > 추격할 거리 + 땅과의 거리
            temp.y = Mathf.Lerp(tr.position.y, traceYpos + groundPos_Player.y + baseCamPos.y + correctionValue, speed_Y_1 * Time.deltaTime);
        else if (isFalling)
            temp.y = Mathf.Lerp(tr.position.y, baseCamPos.y + playerTr.position.y, Vector3.Distance(playerTr.position,tr.position) * 2 * Time.deltaTime);
        else
            temp.y = Mathf.Lerp(tr.position.y, baseCamPos.y + groundPos_Player.y + correctionValue, (isFalling_Before ? speed_Y_3 : speed_Y_2) * Time.deltaTime);

        if (!teleportTrigger)
            tr.position = temp;
    }


    GameObject oldGround = null;
    RaycastHit[] hits;
    void ChackGround(Transform objTr, ref float posY)
    {
        Debug.DrawRay(objTr.position, -Vector3.up * 10, Color.yellow);
        hits = Physics.RaycastAll(objTr.position, -Vector3.up, 10);

        // Ignore를 가진 오브젝트가 있으면 예외 처리
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.transform.CompareTag("Ignore"))
            {
                return;
            }
        }

        // 레이를 쏴서 충돌한 지형들의 Y좌표를 저장 ( 지형이 겹쳐 있으면 여러개 잡히기 때문)
        float[] tempPosY = new float[hits.Length];
        int idx = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("Land"))
            {
                tempPosY[idx++] = hit.point.y;
                posY = hit.point.y;
            }
        }



        // 레이를 쏘는 위치 기준 제일 높은 위치에 있는 지형(Land)을 기준으로 Y축 위치를 저장하기 위함
        float maxY = 0;
        for (int i = idx; i > 0; i--)
        {
            if (maxY < tempPosY[i - 1])
                maxY = tempPosY[i - 1];
        }

        if (maxY != 0)
            posY = maxY;


    }

    public void StartTeleport()
    {
        teleportTrigger = true;
    }
    public void EndTeleport()
    {
        tr.position = playerTr.position + baseCamPos;
        teleportTrigger = false;
    }
}

#endregion

#region 카메라 3
/*
public class CameraCtrl_6 : MonoBehaviour
{
    public float traceYpos = 3f;    // 추격 시작할 Y차이 (땅과 캐릭터)
    public float speed_X = 5f;      // X축 추적 속도
    public float speed_Y_1 = 1.5f;  // Y축 범위 밖 추적 속도
    public float speed_Y_2 = 2.4f;  // Y축 범위 안 추적 속도
    public float speed_Y_3 = 3f;    // Y축 범위 안 추적 속도
    public float speed_Y_4 = 5f;    // Y축 범위 안 추적 속도
    public float fallStandardRange = 5f;

   
    public Transform rayBox_R;      // 레이 박스 Transform
    public Transform rayBox_L;
    public Transform rayBox_Rn;
    public Transform rayBox_Ln;
    public Transform rayBox_All;
    public Transform rayBox_All1;

    Vector3 rayBox_R_addpos;        // 캐릭터 위치에 비례한 레이 박스의 위치 값
    Vector3 rayBox_L_addpos;
    Vector3 rayBox_Rn_addpos;
    Vector3 rayBox_Ln_addpos;
    Vector3 rayBox_All_addpos;
    Vector3 rayBox_All1_addpos;

    float correctionValue;          // 보정 수치 ( 레이 박스와 카메라간의 보정 수치 값)

    Transform tr, playerTr;
    Vector3 groundPos_Player;       // 플레이어가 위에 있는 땅 pos
    Vector3 groundPos_Box_R;        // 레이 박스 위에 있는 땅 pos
    Vector3 groundPos_Box_L;
    Vector3 groundPos_Box_Rn;
    Vector3 groundPos_Box_Ln;
    Vector3 groundPos_Box_All;
    Vector3 groundPos_Box_All1;

    Vector3 baseCamPos;             // 카메라의 기본 위치 (x,z값은 유지, y 값은 지형 높이에 따라 변화 줌)

    float groundToCamYgap;          // 땅과 카메라의 거리차이
    bool isFalling_Before;
    bool isFalling;
    bool isFocusRight;              // 진행 방향

    RaycastHit hit;

    bool teleportTrigger;

    public static CameraCtrl_6 instance;
    void Start()
    {
        instance = this;
        tr = transform;
        playerTr = PlayerCtrl.instance.transform;
        rayBox_R_addpos = rayBox_R.position - playerTr.position;
        rayBox_R_addpos.z = 0;
        rayBox_L_addpos = rayBox_L.position - playerTr.position;
        rayBox_L_addpos.z = 0;
        rayBox_Rn_addpos = rayBox_Rn.position - playerTr.position;
        rayBox_Rn_addpos.z = 0;
        rayBox_Ln_addpos = rayBox_Ln.position - playerTr.position;
        rayBox_Ln_addpos.z = 0;
        rayBox_All_addpos = rayBox_All.position - playerTr.position;
        rayBox_All_addpos.z = 0;
        rayBox_All1_addpos = rayBox_All1.position - playerTr.position;
        rayBox_All1_addpos.z = 0;

        baseCamPos = tr.position - playerTr.position;

        groundToCamYgap = baseCamPos.y - groundPos_Player.y;
        ChackGround(playerTr, ref groundPos_Player.y);

        teleportTrigger = false;
    }

    float tempRangeR;
    float tempRangeRn;
    float tempRangeL;
    float tempRangeLn;
    float tempRangeAll;
    float tempRangeAll1;
    void CamCorrectionValue()
    {
        tempRangeR = Mathf.Abs(groundPos_Box_R.y - groundPos_Player.y);
        tempRangeRn = Mathf.Abs(groundPos_Box_Rn.y - groundPos_Player.y);
        tempRangeL = Mathf.Abs(groundPos_Box_L.y - groundPos_Player.y);
        tempRangeLn = Mathf.Abs(groundPos_Box_Ln.y - groundPos_Player.y);
        tempRangeAll = Mathf.Abs(groundPos_Box_All.y - groundPos_Player.y);
        tempRangeAll1 = Mathf.Abs(groundPos_Box_All1.y - groundPos_Player.y);

        if (isFocusRight)
        {
            if (tempRangeR < fallStandardRange)  // 추락 기준값과 비교, 일반 추적
            {
                isFalling_Before = false;
                correctionValue = (groundPos_Box_R.y - groundPos_Player.y) * 0.5f;
                Debug.Log("오른쪽");
            }
            else if(tempRangeAll < fallStandardRange)
            {
                isFalling_Before = false;
                correctionValue = (groundPos_Box_All.y - groundPos_Player.y) * 0.5f;
            }
            else if (tempRangeAll1 < fallStandardRange)
            {
                isFalling_Before = false;
                correctionValue = (groundPos_Box_All.y - groundPos_Player.y) * 0.5f;
            }
            else if (tempRangeRn > fallStandardRange)
            {
                isFalling_Before = true;
                correctionValue = (groundPos_Box_Rn.y - groundPos_Player.y) * 0.5f;
            }
            else
                isFalling_Before = false;
        }
        else
        {
            if (tempRangeL < fallStandardRange)  // 추락 기준값과 비교, 일반 추적
            {
                isFalling_Before = false;
                correctionValue = (groundPos_Box_L.y - groundPos_Player.y) * 0.5f;
                Debug.Log("왼쪽");
            }
            else if (tempRangeLn > fallStandardRange)
            {
                isFalling_Before = true;
                correctionValue = (groundPos_Box_Ln.y - groundPos_Player.y) * 0.5f;
            }
            else
                isFalling_Before = false;
        }


        // 플레이어와 땅의 거리를 비교, 추락할 위치 인지 체크
        if (Mathf.Abs(playerTr.position.y - groundPos_Player.y) > fallStandardRange)
        {
            isFalling = true;
        }
        else if (PlayerCtrl.controller.isGrounded)
            isFalling = false;

    }

    void FixedUpdate()
    {
        rayBox_R.position = playerTr.position + rayBox_R_addpos;
        rayBox_L.position = playerTr.position + rayBox_L_addpos;
        rayBox_Rn.position = playerTr.position + rayBox_Rn_addpos;
        rayBox_Ln.position = playerTr.position + rayBox_Ln_addpos;
        rayBox_All.position = playerTr.position + rayBox_All_addpos;
        rayBox_All1.position = playerTr.position + rayBox_All1_addpos;
    }

    void Update()
    {
        
        isFocusRight = PlayerCtrl.isFocusRight;

        ChackGround(playerTr, ref groundPos_Player.y);
        ChackGround(rayBox_R, ref groundPos_Box_R.y);
        ChackGround(rayBox_L, ref groundPos_Box_L.y);
        ChackGround(rayBox_Rn, ref groundPos_Box_Rn.y);
        ChackGround(rayBox_Ln, ref groundPos_Box_Ln.y);
        ChackGround(rayBox_All, ref groundPos_Box_Ln.y);
        ChackGround(rayBox_All1, ref groundPos_Box_Ln.y);
        CamCorrectionValue();
    }

    void LateUpdate()
    {
        Vector3 temp = tr.position;
        temp = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, 0, playerTr.position.z) + baseCamPos, speed_X * Time.deltaTime);

        if (playerTr.position.y > traceYpos + groundPos_Player.y)  // 플레이어 위치 > 추격할 거리 + 땅과의 거리
            temp.y = Mathf.Lerp(tr.position.y, traceYpos + groundPos_Player.y + baseCamPos.y + correctionValue, speed_Y_1 * Time.deltaTime);
        else if(isFalling)
        {
            temp.y = Mathf.Lerp(tr.position.y, baseCamPos.y + playerTr.position.y, speed_Y_4 * Time.deltaTime);
            Debug.Log("추락");
        }
        else
        {
            temp.y = Mathf.Lerp(tr.position.y, baseCamPos.y + groundPos_Player.y + correctionValue, (isFalling_Before ? speed_Y_3 : speed_Y_2) * Time.deltaTime);

        }

        if(!teleportTrigger)
            tr.position = temp;
    }


    GameObject oldGround = null;
    RaycastHit[] hits;
    void ChackGround(Transform objTr, ref float posY)
    {
        Debug.DrawRay(objTr.position, -Vector3.up * 30, Color.yellow);
        hits = Physics.RaycastAll(objTr.position, -Vector3.up, 30);

        // Ignore를 가진 오브젝트가 있으면 예외 처리
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.transform.CompareTag("Ignore"))
            {
                return;
            }
        }

        // 레이를 쏴서 충돌한 지형들의 Y좌표를 저장 ( 지형이 겹쳐 있으면 여러개 잡히기 때문)
        float[] tempPosY = new float[hits.Length];
        int idx = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("Land"))
            {
                tempPosY[idx++] = hit.point.y;
                posY = hit.point.y;
            }
        }

        float tempVal;
        float approx = 999f; // 근사치 값 ( 낮을 수록 근사 값)
        int approxIdx = 0;
        
        // 플레이어의 위치와 가까운 Y축 좌표를 찾음
        for (int i = idx; i > 0; i--)
        {
            tempVal = Mathf.Abs(playerTr.position.y - tempPosY[i - 1]);
            if (approx > tempVal)
            {
                approx = tempVal;
                approxIdx = i - 1;
            }
        }
        Debug.Log(idx);
        if (approx != 999f)
            posY = tempPosY[approxIdx];

        
        //// 레이를 쏘는 위치 기준 제일 높은 위치에 있는 지형(Land)을 기준으로 Y축 위치를 저장하기 위함
        //float maxY = 0;
        //for(int i = idx; i > 0; i--)
        //{
        //    if (maxY < tempPosY[i-1])
        //        maxY = tempPosY[i-1];
        //}

        //if(maxY != 0)
        //    posY = maxY;
        

    }

    public void StartTeleport()
    {
        teleportTrigger = true;
    }
    public void EndTeleport()
    {
        tr.position = playerTr.position + baseCamPos;
        teleportTrigger = false;
    }
}
*/

// 카메라 2
/*
public class CameraCtrl_6 : MonoBehaviour
{
    public float traceYpos = 3f;    // 추격 시작할 Y차이 (땅과 캐릭터)
    public float speed_X = 5f;      // X축 추적 속도
    public float speed_Y_1 = 1.5f;  // Y축 범위 밖 추적 속도
    public float speed_Y_2 = 2.4f;  // Y축 범위 안 추적 속도


    Transform tr, playerTr;
    Vector3 groundPos;              // 플레이어가 위에 있는 땅 pos
    Vector3 baseCamPos;             // 카메라의 기본 위치 (x,z값은 유지, y 값은 지형 높이에 따라 변화 줌)
    float groundToCamYgap;          // 땅과 카메라의 거리차이
    RaycastHit hit;

    void Start()
    {
        tr = transform;
        playerTr = PlayerCtrl.instance.transform;
        baseCamPos = tr.position - playerTr.position;
        groundToCamYgap = baseCamPos.y - groundPos.y;
        ChackGround();

    }


    void Update()
    {
        ChackGround();
    }


    void LateUpdate()
    {
        Vector3 temp = tr.position;
        temp = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, 0, playerTr.position.z) + baseCamPos, speed_X * Time.deltaTime);
        if (playerTr.position.y > traceYpos + groundPos.y)  // 플레이어 위치 > 추격할 거리 + 땅과의 거리
            temp.y = Mathf.Lerp(tr.position.y, traceYpos + groundPos.y + baseCamPos.y, speed_Y_1 * Time.deltaTime);
        else
            temp.y = Mathf.Lerp(tr.position.y, baseCamPos.y + groundPos.y, speed_Y_2 * Time.deltaTime);

        tr.position = temp;
    }


    GameObject oldGround = null;
    RaycastHit[] hits;
    void ChackGround()
    {

        Debug.DrawRay(playerTr.position, -Vector3.up * 20, Color.yellow);
        hits = Physics.RaycastAll(playerTr.position, -Vector3.up, 20);

        Debug.Log(hits.Length);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.transform.CompareTag("Ignore"))
            {
                return;
            }
        }

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            Debug.Log(hit.transform.name);
            if (hit.transform.CompareTag("Land"))
            {
                groundPos.y = hit.point.y;
            }
        }
    }
}
*/

// 카메라 1
/*
public class CameraCtrl_6 : MonoBehaviour
{
public float traceYpos = 3f;    // 추격 시작할 Y차이 (땅과 캐릭터)
public float speed_X = 5f;      // X축 추적 속도
public float speed_Y_1 = 1.5f;  // Y축 범위 밖 추적 속도
public float speed_Y_2 = 2.4f;  // Y축 범위 안 추적 속도


Transform tr, playerTr;
Vector3 groundPos;              // 플레이어가 위에 있는 땅 pos
Vector3 baseCamPos;             // 카메라의 기본 위치 (x,z값은 유지, y 값은 지형 높이에 따라 변화 줌)
float groundToCamYgap;          // 땅과 카메라의 거리차이
RaycastHit hit;

void Start()
{
    tr = transform;
    playerTr = PlayerCtrl.instance.transform;
    baseCamPos = tr.position - playerTr.position;
    groundToCamYgap = baseCamPos.y - groundPos.y;
    ChackGround();

}


void Update()
{
    ChackGround();
}


void LateUpdate()
{
    Vector3 temp = tr.position;
    temp = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, 0, playerTr.position.z) + baseCamPos, speed_X * Time.deltaTime);
    if (playerTr.position.y > traceYpos + groundPos.y)  // 플레이어 위치 > 추격할 거리 + 땅과의 거리
        temp.y = Mathf.Lerp(tr.position.y, traceYpos + groundPos.y + baseCamPos.y, speed_Y_1 * Time.deltaTime);
    else
        temp.y = Mathf.Lerp(tr.position.y, baseCamPos.y + groundPos.y, speed_Y_2 * Time.deltaTime);

    tr.position = temp;
}


GameObject oldGround = null;
RaycastHit[] hits;
void ChackGround()
{

    Debug.DrawRay(playerTr.position, -Vector3.up * 10, Color.yellow);
    hits = Physics.RaycastAll(playerTr.position, -Vector3.up, 10);

    Debug.Log(hits.Length);
    for (int i = 0; i < hits.Length; i++)
    {
        RaycastHit hit = hits[i];

        Debug.Log(hit.transform.name);
        if (hit.transform.CompareTag("Land"))
        {
            groundPos.y = hit.point.y;
        }
    }
}
}
*/
