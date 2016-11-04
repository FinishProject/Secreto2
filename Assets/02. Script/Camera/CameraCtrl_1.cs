using UnityEngine;
using System.Collections;

/************************************ 사용 방법 ****************************************

    가장 기본 적인 카메라
    플레이어와 일정거리 떨어져서 따라가는 카메라
 
    ※ 사용 방법
    1. 카메라에 스크립트를 추가한다

****************************************************************************************/

public class CameraCtrl_1 : MonoBehaviour
{

    Transform tr;                   // 현재 카메라 Transform
    Transform playerTr;             // 현재 플레이어 Transform
    Vector3 camAddPos_ViewRight;    // 플레이어와 카메라의 벡터값
    Vector3 camAddPos_ViewLeft;     // 플레이어와 카메라의 벡터값
    public float speed_Y = 2;
    public float wallRayToCamGap = 6.9f;
    float normalSpeed_Y = 2;
    float curSpeed_X;
    float speed_X_Max = 5;

    bool hasCamTarget;              // 카메라 타겟 위치가 있는지
    bool isFocusRight;              // 진행방향 체크
    bool oldFocusRight;             // 이전 Update에서의 진행방향

    public static CameraCtrl_1 instance;
    void Start()
    {
        instance = this;
        tr = transform;
        playerTr = PlayerCtrl.instance.transform;
        camAddPos_ViewRight = tr.position - playerTr.position;
        camAddPos_ViewLeft = camAddPos_ViewRight;
        camAddPos_ViewLeft.x = playerTr.position.x - tr.position.x;
    }

    

    Vector3 tempPos;
    void Update()
    {
        if (hasCamTarget)
            return;

        FocusChecker();
        Speed_Y_Ctrl();
        Speed_X_Ctrl();

        // 카메라의 x 좌표 움직임 ( 캐릭터가 바라보는 방향에 따라 ) 
        if (isFocusRight)
            tempPos = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, 0, playerTr.position.z) + camAddPos_ViewRight, curSpeed_X * Time.deltaTime);
        else
            tempPos = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, 0, playerTr.position.z) + camAddPos_ViewLeft,  curSpeed_X * Time.deltaTime);

        ChackWall_ByRay();

        // 우측에 벽이 있으면 ( else if는 좌측에 벽이 있으면 )
        if (isNeerWall_Right && wall_R_Pos_X - wallRayToCamGap < tempPos.x)
            tempPos.x = wall_R_Pos_X - wallRayToCamGap;
        else if (isNeerWall_Left && wall_L_Pos_X + wallRayToCamGap > tempPos.x)
            tempPos.x = wall_L_Pos_X + wallRayToCamGap;

        tempPos.y = Mathf.Lerp(tr.position.y, playerTr.position.y + camAddPos_ViewRight.y + shakeVal.y, speed_Y * Time.deltaTime);

        if (!teleportTrigger)
            tr.position = tempPos;
    }

    #region 카메라 이동 관련 함수
    void Speed_Y_Ctrl()
    {
        if (PlayerCtrl.controller.velocity.y > -15f)
            speed_Y = normalSpeed_Y;
        else
            speed_Y += 20f * Time.deltaTime;
    }

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

    // 벽 충돌 체크
    Vector3 wallCheckCenterPos;     // 벽을 체크할 레이를 쏠 위치값
    RaycastHit[] hits;
    bool isNeerWall_Left;           // 왼쪽에 벽이 있으면
    bool isNeerWall_Right;          // 오른쪽에 벽이 있으면
    float wall_L_Pos_X;             // 오른쪽에 위치한 벽의 x 좌표
    float wall_R_Pos_X;             // 왼쪽에 위치한 벽의 x 좌표

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

        //        Debug.Log(hits.Length + " 오른쪽 " + isNeerWall_Right);

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

    #region 카메라 쉐이킹
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

        while (range < startShakeRange)
        {
            range = Vector3.Distance(objTr.position, playerTr.position);
            shakeVal.y *= -1f;
            yield return new WaitForSeconds((range / startShakeRange) * 0.2f);
        }
        shakeVal = new Vector3(0, 0, 0);
    }

    public void StartShake(float shakeTime)
    {
        StartCoroutine(ShakeCamera(shakeTime));
    }

    IEnumerator ShakeCamera(float shakeTime)
    {
        shakeVal = new Vector3(0, 0.3f, 0);

        while (shakeTime >= 0f)
        {
            shakeVal.y *= -1f;
            shakeTime -= Time.deltaTime;
            yield return new WaitForSeconds(0.2f);
        }

        shakeVal = new Vector3(0, 0, 0);
    }
    #endregion

    #region 타켓을 바라보는 카메라
    public void StartViewTargetCam(Transform CamTargetPos)
    {
        StartCoroutine(ViewTargetCam(CamTargetPos));
    }

    IEnumerator ViewTargetCam(Transform CamTargetPos)
    {
        FadeInOut.instance.StartFadeInOut(1, 1, 1);
        hasCamTarget = true;
        yield return new WaitForSeconds(2f);
        tr.position = CamTargetPos.position;
        PlayerCtrl.instance.SetPlayerMove(10f);
        yield return new WaitForSeconds(3f);
        FadeInOut.instance.StartFadeInOut(1, 1, 1);
        yield return new WaitForSeconds(1f);
        hasCamTarget = false;
    }
    #endregion

    #region 포탈 관련 함수
    bool teleportTrigger;
    public void StartTeleport()
    {
        teleportTrigger = true;
    }
    public void EndTeleport(bool isRight)
    {
        if (isRight)
            tr.position = playerTr.position + camAddPos_ViewRight;
        else
        {
            tr.position = playerTr.position + camAddPos_ViewLeft;
        }

        teleportTrigger = false;
    }
    #endregion
}

/*
public class CameraCtrl_1 : MonoBehaviour
{

Transform tr;         // 현재 카메라 Transform
Transform playerTr;   // 현재 플레이어 Transform
Vector3 camAddPos;    // 플레이어와 카메라의 벡터값
public float speedY = 2;
float normalSpeedY = 2;
void Start()
{
    tr = transform;
    playerTr = PlayerCtrl.instance.transform;
    camAddPos = tr.position - playerTr.position;
}

void Yspeed()
{
    if (PlayerCtrl.controller.velocity.y > -15f)
        speedY = normalSpeedY;
    else
        speedY += 20f * Time.deltaTime;
}

Vector3 tempPos;
void Update()
{
    Yspeed();
    float tempSpeed = Vector3.Distance(tr.position, playerTr.position + camAddPos) * 2.5f;
    tempPos = Vector3.Lerp(tr.position, playerTr.position + camAddPos, tempSpeed * Time.deltaTime);
    tempPos.y = Mathf.Lerp(tr.position.y, playerTr.position.y + camAddPos.y, speedY * Time.deltaTime);
    tr.position = tempPos;
}
}
*/
