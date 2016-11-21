using UnityEngine;
using System.Collections;

public class CameraCtrl_1 : MonoBehaviour, Sensorable_Return
{

    public Transform areaCubeTr;
    Transform tr;                   // 현재 카메라 Transform
    Transform playerTr;             // 현재 플레이어 Transform
    Vector3 camAddPos_ViewRight;    // 플레이어와 카메라의 벡터값
    Vector3 camAddPos_ViewLeft;     // 플레이어와 카메라의 벡터값

    public float speed_X_Max = 5;
    public float speed_Y = 1;
    public float speed_Z = 5;
    public float wallRayToCamGap = 6.9f;
    float normalSpeed_Y = 1;
    float curSpeed_X;
    

    bool hasCamTarget;              // 카메라 타겟 위치가 있는지
    bool isFocusRight;              // 진행방향 체크
    bool oldFocusRight;             // 이전 Update에서의 진행방향
    bool isZoom = false;            // 줌 상태인가?

    // 줌 관련 변수
    float zoomAreaX;                    // 줌 구역 시작 X좌표 위치
    float zoomAreaSize;                 // 줌 구역 길이
    float zoomDeep;                     // 줌 깊이
    float zoomHeight;                   // 줌 높이
    float zoomStartRangePercent;        // 줌인, 아웃 시작 비율


    float normalSpeedY = 2;

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
        areaCubeTr.position = playerTr.position;
        if (hasCamTarget)
            return;

        FocusChecker();
        Speed_Y_Ctrl();
        Speed_X_Ctrl();
        Zoomed();

        // 카메라의 x 좌표 움직임 ( 캐릭터가 바라보는 방향에 따라 ) 
        if (isFocusRight)
            tempPos.x = Mathf.Lerp(tr.position.x, playerTr.position.x + camAddPos_ViewRight.x, curSpeed_X * Time.deltaTime);
        else
            tempPos.x = Mathf.Lerp(tr.position.x, playerTr.position.x + camAddPos_ViewLeft.x , curSpeed_X * Time.deltaTime);
        ChackWall_ByRay();

        // 우측에 벽이 있으면 ( else if는 좌측에 벽이 있으면 )
        if (isNeerWall_Right && wall_R_Pos_X - wallRayToCamGap < tempPos.x)
            tempPos.x = wall_R_Pos_X - wallRayToCamGap + 0.1f;
        else if (isNeerWall_Left && wall_L_Pos_X + wallRayToCamGap > tempPos.x)
            tempPos.x = wall_L_Pos_X + wallRayToCamGap - 0.1f;

        // 카메라의 y 좌표 움직임 ( 카메라 쉐이킹 추가 )
        tempPos.y = Mathf.Lerp(tr.position.y, playerTr.position.y + camAddPos_ViewRight.y + shakeVal.y + zoomPos.y, speed_Y * Time.deltaTime);

        // 카메라의 z 좌표 움직임
        tempPos.z = Mathf.Lerp(tr.position.z, playerTr.position.z + camAddPos_ViewRight.z + zoomPos.z, speed_Z * Time.deltaTime);

//        tempPos = Vector3.MoveTowards(tempPos, tempPos + zoomPos, speed_Z * Time.deltaTime);
        if (!teleportTrigger)
            tr.position = tempPos;   
    }


    Vector3 zoomPos;
    void Zoomed()
    {
        zoomPos = Vector3.zero;
        if (isZoom)
        {
            float zoomRange = transform.position.x - zoomAreaX;

            // 0이하의 값이 뜨면 안됨 ( 충돌 박스로 isZoom 체크 하기 때문에 초반에 -값이 뜬다 [ 중심 값이 필요하다 ])
            if (zoomRange < 0) return;

            float zoomPercent = Mathf.Ceil(zoomRange / zoomAreaSize * 100) * 0.01f;

            // 1이상의 값이 뜨면 안됨 ( 위와 마찬가지 )
            if (zoomPercent > 1) return;

            // 시작 위치
            if (zoomPercent < zoomStartRangePercent)
            {
                zoomPos.y =   (zoomHeight * (zoomPercent / zoomStartRangePercent));
                zoomPos.z = - (zoomDeep   * (zoomPercent / zoomStartRangePercent));
            }
            // 중간 위치
            else if (zoomPercent <= (1 - zoomStartRangePercent))
            {
                zoomPos.y =   zoomHeight;
                zoomPos.z = - zoomDeep;
            }
            // 끝 위치
            else
            {
                zoomPos.y =   (zoomHeight * ((1 - zoomPercent) / zoomStartRangePercent));
                zoomPos.z = - (zoomDeep   * ((1 - zoomPercent) / zoomStartRangePercent));
            }
        }
    }

    // 라인 센서에 무언가가 충돌 했을 때, 충돌한 오브젝트를 받아옴 ( 라인 스크립트에 충돌할 것 지정 )
    public void ActiveSensor_Retuen(int index, GameObject gameObjet)
    {
        if (gameObjet == null)
        {
            isZoom = false;
            return;
        }

        if (!isZoom)
        {
            // 오브젝트에서 줌 정보를 가져옴 ( 구조체 그대로 썼더니 가독성이 안좋아서 각 변수에 저장함 )
            ZoomState tempZoomState = gameObjet.transform.GetComponent<ZoomArea>().zoomState;
            zoomAreaSize = tempZoomState.areaSize;
            zoomAreaX = tempZoomState.areaX;
            zoomDeep = tempZoomState.deep;
            zoomHeight = tempZoomState.height;
            zoomStartRangePercent = tempZoomState.startRangePercent;
            isZoom = true;
        }
    }

    /*
    CameraArea_3 cameraArea;
    public void ActiveSensor_Retuen(int index, GameObject returnObjet)
    {

        if (returnObjet != null)
        {
            cameraArea.correctionValue = returnObjet.GetComponent<CameraArea_3>().correctionValue;
        }
        else
        {
            cameraArea.correctionValue = CameraArea_3.ZeroCorrectionValue();
        }

    }
    */

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
    public void FocusChecker()
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

    bool isShake = false;
    public void StartShaking()
    {
        if(!isShake)
            StartCoroutine(Shaking());
    }

    IEnumerator Shaking()
    {
        Vector3 originPos = shakeVal;
        isShake = true;
        float shakeTime = 0f;
        while (shakeTime <= 2f)
        {
            shakeTime += Time.deltaTime;
            shakeVal = originPos + Random.insideUnitSphere * 1f;
            yield return null;
        }
        isShake = false;
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
        StartCoroutine(PlayerCtrl.instance.SetStopMoveDuration(10f));
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
