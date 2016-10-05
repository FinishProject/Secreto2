using UnityEngine;
using System.Collections;

public class CameraCtrl_4 : MonoBehaviour, Sensorable_Return, Sensorable_Something
{
    public Vector3 playerDistance;
    public float camSpeed = 5f;

    bool isNearByWall_L = false;
    bool isNearByWall_R = false;
    bool isCinematicView = false;

    private float originCamSpeed;

    public Vector3 NearWallDistance;

    Transform playerTr;
    Transform tr;

    Transform sensorArea, sensorWall_L, sensorWall_R;
    CameraArea cameraArea;
    Camera cam;

    Vector3 cinemaPos, cinemaFocusPos;


    public static CameraCtrl_4 instance;

    void Start()
    {
        instance = this;
        tr = GetComponent<Transform>();
        cam = GetComponent<Camera>();

        sensorArea = GameObject.Find("Sensor_Area").transform;
        sensorWall_L = GameObject.Find("Sensor_Wall_L").transform;
        sensorWall_R = GameObject.Find("Sensor_Wall_R").transform;

        playerTr = PlayerCtrl.instance.transform;

        originCamSpeed = camSpeed;
    }

    public float range;
    public int EndCnt;
    public float delay;
    public float mi;
    public Vector3 shakePos;
    public IEnumerator Shake(float range, int EndCnt, float delay)
    {
        float rangeY = range;
        float curCnt = 0;
        while (true)
        {
            shakePos.Set(0, rangeY, 0);

            rangeY = rangeY * mi;
            rangeY *= -1;
            curCnt++;

            if (curCnt >= EndCnt)
                break;

            yield return new WaitForSeconds(delay);
        }
        shakePos = Vector3.zero;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Debug.Log(1111);
            StartCoroutine(Shake(range, EndCnt, delay));
        }
           

        // 거리가 급격히 멀어 졌을 때 (죽었을 때)
        if (Vector3.Distance(tr.position, playerTr.position) > 30f)
            tr.position = playerTr.position;

        // 시네마틱 뷰 일때
        if (isCinematicView)
        {
            CinematicView();
        }
        else
        {
            // 벽과 충돌이 있을때
            if (isNearByWall_L || isNearByWall_R)
            {
                NearByWall();
            }
            // 벽과 충돌이 없을 때
            else
            {
                tr.position = Vector3.Lerp(tr.position, playerTr.position + playerDistance + shakePos, camSpeed * Time.deltaTime);
            }

            // 카메라 영역에 포커스가 있을때
            if (cameraArea.hasFocus)
            {
                Vector3 pos = cameraArea.focusTr.position - tr.position;
                Quaternion newRot = Quaternion.LookRotation(pos);
                tr.rotation = Quaternion.Lerp(tr.rotation, newRot, camSpeed * Time.deltaTime);
            }
            else
            {
                tr.rotation = Quaternion.Lerp(tr.rotation, Quaternion.identity, camSpeed * Time.deltaTime);
            }

            CensorRotZero();
        }
   
    }

    // 시네마틱 뷰

    public void SetCinematicView(bool isCinematicView, Vector3 cinemaPos, Vector3 cinemaFocusPos)
    {
        this.isCinematicView = isCinematicView;
        this.cinemaPos = cinemaPos;
        this.cinemaFocusPos = cinemaFocusPos;
    }

    void CinematicView()
    {

        // 위치로 이동
        tr.position = Vector3.Lerp(tr.position, cinemaPos, camSpeed * Time.deltaTime);

        // 포커싱
        Vector3 pos = cinemaFocusPos - tr.position;
        Quaternion newRot = Quaternion.LookRotation(pos);

        tr.rotation = Quaternion.Slerp(tr.rotation, newRot, camSpeed * Time.deltaTime);
    }

    void NearByWall()
    {
        // 양쪽 두 벽과 충돌이 있을 때
        if (isNearByWall_L && isNearByWall_R)
        {
            Vector3 tempPos = tr.position;
            tempPos.y = playerTr.position.y + playerDistance.y;
            tr.position = Vector3.Lerp(tr.position, tempPos, camSpeed * Time.deltaTime);
        }
        // 한쪽 벽과 충돌이 있을 때
        else if (isNearByWall_L || isNearByWall_R)
        {
            if (isNearByWall_L && playerTr.position.x >= player_L_EndPos + playerDistance.x)
            {
                tr.position = Vector3.Lerp(tr.position, playerTr.position + NearWallDistance, camSpeed * Time.deltaTime);
            }
            else if (isNearByWall_R && playerTr.position.x <= player_R_EndPos - playerDistance.x)
            {
                tr.position = Vector3.Lerp(tr.position, playerTr.position + NearWallDistance, camSpeed * Time.deltaTime);
            }
            else
            {
                Vector3 tempPos = tr.position;
                tempPos.y = playerTr.position.y + playerDistance.y;
                tr.position = Vector3.Lerp(tr.position, tempPos, camSpeed * Time.deltaTime);
            }
        }

    }

    void CensorRotZero()
    {
        sensorWall_L.rotation = Quaternion.Lerp(sensorWall_L.rotation, Quaternion.identity, camSpeed * Time.deltaTime);
        sensorWall_R.rotation = Quaternion.Lerp(sensorWall_R.rotation, Quaternion.identity, camSpeed * Time.deltaTime);
        sensorArea.rotation = Quaternion.Lerp(sensorArea.rotation, Quaternion.identity, camSpeed * Time.deltaTime);
    }

    float player_L_EndPos;
    float player_R_EndPos;
    public void ActiveSensor_Retuen(int index, GameObject returnObjet)
    {
        switch (index)
        {
            case 1:
                if (returnObjet != null)
                {
                    cameraArea = returnObjet.GetComponent<CameraArea>();
                    playerDistance = cameraArea.playerDistance;
                    camSpeed = cameraArea.camSpeed;
                }
                break;

            case 2:
                if (returnObjet == null)
                {
                    isNearByWall_L = false;
                }
                else
                {
                    isNearByWall_L = true;
                    NearWallDistance = playerDistance;

                    float sensor_L_EndPos = tr.position.x - (tr.position.x - sensorWall_L.transform.position.x) - (sensorWall_L.transform.localScale.x * 0.5f);
                    float wall_L_EndPos = returnObjet.transform.position.x + (returnObjet.transform.localScale.x * 0.5f);
                    NearWallDistance.x = wall_L_EndPos - sensor_L_EndPos;
                    player_L_EndPos = playerTr.position.x;

                    /*
                    NearWallDistance.x +=
                        (returnObjet.transform.position.x - returnObjet.transform.localScale.x * 0.5f)
                        - (sensorWall_R.position.x - sensorWall_R.localScale.x * 0.5f);
                    */
                }

                break;

            case 3:
                if (returnObjet == null)
                {
                    isNearByWall_R = false;
                }
                else
                {
                    isNearByWall_R = true;
                    NearWallDistance = playerDistance;

                    float sensor_R_EndPos = tr.position.x + (sensorWall_R.transform.position.x - tr.position.x) + (sensorWall_R.transform.localScale.x * 0.5f);
                    float wall_R_EndPos = returnObjet.transform.position.x - (returnObjet.transform.localScale.x * 0.5f);
                    NearWallDistance.x = -(sensor_R_EndPos - wall_R_EndPos);
                    player_R_EndPos = playerTr.position.x;
                    /*
                    NearWallDistance.x += 
                        (returnObjet.transform.position.x - returnObjet.transform.localScale.x * 0.5f)
                        -(sensorWall_R.position.x + sensorWall_R.localScale.x * 0.5f);
                    */
                }
                break;
        }



    }
    public bool ActiveSensor_Something(int index)
    {
        switch (index)
        {
            case 2:
                isNearByWall_L = true;
                NearWallDistance = playerDistance;
                break;

            case 3:
                isNearByWall_R = true;
                break;

            case 102:
                isNearByWall_L = false;
                break;

            case 103:
                isNearByWall_R = false;
                break;
        }

        return false;
    }

    // 외부에서 카메라 속도 변경
    public void ChangeCamSpeed(float speed)
    {
        camSpeed = speed;
    }
    // 카메라 속도를 초기 설정한 속도로 되돌림
    public void ResetCameSpeed()
    {
        camSpeed = originCamSpeed;
    }

}
