using UnityEngine;
using System.Collections;

[System.Serializable]
public struct ZoomState
{
    [System.NonSerialized]
    public float areaX;                 // 줌 구역 시작 X좌표 위치
    [System.NonSerialized]
    public float areaSize;              // 줌 구역 길이
    public float deep;                  // 줌 깊이
    public float height;                // 줌 높이
    public float startRangePercent;     // 줌인, 아웃 시작 비율
}

public class CameraCtrl_2 : MonoBehaviour, Sensorable_Player, Sensorable_Something, Sensorable_Return
{
    bool isLeftSide = false;            // 플에이어가 왼쪽에 있는가?
    bool isZoom = false;                // 줌 상태인가?

    GameObject inLine_L, inLine_R, Outside_L, Outside_R;    // 라인 오브젝트
    Transform playerTr;                                     // 플레이어 transform
    Transform inLine_L_Tr, inLine_R_Tr;                     // 라인 transform
    Vector3 originPos;                                      // 줌 효과를 주기 전 위치 저장 ( y축, z축 사용 )
    RaycastHit hit;                                         // 레이캐스트 충돌 정보

    // 이동 관련 변수
    float speed;                        // 카메라 이동 속도
    float speed_InLine = 8f;            // 안쪽 라인일때 이동 속도
    float speed_OutLine = 9f;           // 밖쪽 라인일때 이동 속도
    float heightGap;                    // 캐릭터와 카메라 y축 거리 ( 높이 차 )
    float teleportRange = 25f;          // 텔레포트 조건 거리 ( 플레이어와 카메라의 거리 차이 ) 

    // 줌 관련 변수
    float zoomAreaX;                    // 줌 구역 시작 X좌표 위치
    float zoomAreaSize;                 // 줌 구역 길이
    float zoomDeep;                     // 줌 깊이
    float zoomHeight;                   // 줌 높이
    float zoomStartRangePercent;        // 줌인, 아웃 시작 비율



    void Start()
    {
        playerTr = PlayerCtrl.instance.transform;
        inLine_L = GameObject.Find("inLine_L");
        inLine_R = GameObject.Find("inLine_R");
        Outside_L = GameObject.Find("Outside_L");
        Outside_R = GameObject.Find("Outside_R");

        inLine_L_Tr = inLine_L.transform;
        inLine_R_Tr = inLine_R.transform;

        heightGap = transform.position.y - playerTr.position.y;
        originPos = transform.position;
    }

    void Update()
    {
        TeleportCam();
        
    }

    void LateUpdate()
    {
        MoveInsideLine();
        MoveY();
        Zoomed();
    }

    // 라인 센서에 플레이어가 충돌 했을 때 ( 라인의 인덱스 값을 받아옴)
    public bool ActiveSensor_Player(int index)
    {
        switch (index)
        {
            case 1:
                isLeftSide = true;
                inLine_L.SetActive(true);
                speed = speed_InLine;
                break;
            case 2:
                isLeftSide = false;
                inLine_R.SetActive(true);
                speed = speed_InLine;
                break;
            case 3:
                isLeftSide = false;
                ActiveLine();
                inLine_L.SetActive(false);
                speed = speed_OutLine;
                break;
            case 4:
                isLeftSide = true;
                ActiveLine();
                inLine_R.SetActive(false);
                speed = speed_OutLine;
                break;
        }

        return false;
    }

    // 라인 센서에 무언가가 충돌 했을 때 ( 라인 스크립트에 충돌할 것 지정 )
    public bool ActiveSensor_Something(int index)
    {
        switch (index)
        {
            case 3:
                inLine_L.SetActive(false);
                inLine_R.SetActive(false);
                Outside_L.SetActive(false);
                speed = 0;
                break;
            case 4:
                inLine_L.SetActive(false);
                inLine_R.SetActive(false);
                Outside_R.SetActive(false);
                speed = 0;
                break;
        }

        return false;
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
            // 줌 상태에서 벗어 났을때 위치를 위해서 위치 저장
            originPos.y = ShotRay_GroundPos().y;

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

    // 모든 라인을 활성화
    public void ActiveLine()
    {
        inLine_L.SetActive(true);
        inLine_R.SetActive(true);
        Outside_L.SetActive(true);
        Outside_R.SetActive(true);
    }

    // 일정 범위를 벗어나면 플레이어 위치로 텔레포트
    void TeleportCam()
    {
        if (Vector3.Distance(playerTr.position, transform.position) > teleportRange)
        {
            Vector3 tempPos = playerTr.position;
            tempPos.y += heightGap;
            transform.position = tempPos;
        }
    }

    // 라인을 따라 움직임
    void MoveInsideLine()
    {
        Vector3 tempPos = transform.position;

        if (isLeftSide)
        {
            if (playerTr.position.x > inLine_L_Tr.position.x)
            {
                tempPos.x = playerTr.position.x + (tempPos.x - inLine_L_Tr.position.x);
            }
        }
        else
        {
            if (playerTr.position.x < inLine_R_Tr.position.x)
            {
                tempPos.x = playerTr.position.x - (inLine_R_Tr.position.x - tempPos.x);
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, tempPos, speed * Time.deltaTime);
    }


    // Y축 움직임 ( 낙하 속도를 따라 잡지 못해 별도의 함수로 뺐음)
    void MoveY()
    {
        Vector3 tempPos = transform.position;

        // 줌 상태에는 플레이어 점프에 따른 Y축 움직임이 없음
        if (!isZoom)
        {
            tempPos.y = playerTr.position.y + heightGap;
        }

        // 플레이어가 높은 곳에서 떨어지는지 체크 ( 보통 일반 점프시 -7정도 값이 나옴 )
        if (PlayerCtrl.controller.velocity.y < -8f) 
            transform.position = Vector3.MoveTowards(transform.position, tempPos, 50f * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, tempPos, speed * Time.deltaTime);
    }


    // 줌 상태
    void Zoomed()
    {
        Vector3 tempPos = transform.position;
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
                tempPos.y = originPos.y + (zoomHeight * (zoomPercent / zoomStartRangePercent));
                tempPos.z = originPos.z - (zoomDeep * (zoomPercent / zoomStartRangePercent));
            }
            // 중간 위치
            else if (zoomPercent <= (1 - zoomStartRangePercent))
            {
                tempPos.y = originPos.y + zoomHeight;
                tempPos.z = originPos.z - zoomDeep;
            }
            // 끝 위치
            else
            {
                tempPos.y = originPos.y + (zoomHeight * ((1 - zoomPercent) / zoomStartRangePercent));
                tempPos.z = originPos.z - (zoomDeep * ((1 - zoomPercent) / zoomStartRangePercent));
            }
        }
        else
        {
            tempPos.z = originPos.z;
        }
        transform.position = Vector3.MoveTowards(transform.position, tempPos, speed * Time.smoothDeltaTime);
    }


    // 레이를 바닥에 쏴서 ( 플레이어 기준 ) 충돌된 위치 반환
    Vector3 ShotRay_GroundPos()
    {
        Debug.DrawRay(playerTr.position, -playerTr.up * 5, Color.yellow);
        if (Physics.Raycast(playerTr.position, -playerTr.up * 5, out hit))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                return hit.point;
            }
        }
        return Vector3.zero;
    }
    
}