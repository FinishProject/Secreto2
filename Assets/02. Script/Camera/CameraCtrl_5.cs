using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;



// 카메라 4

public class CameraCtrl_5 : MonoBehaviour, Sensorable_Return
{
    public float camSpeed = 5f;

    bool isNearByWall_L = false;
    bool isNearByWall_R = false;

    public Vector3 NearWallDistance;

    private float originCamSpeed;

    float player_L_EndPos;
    float player_R_EndPos;
    float camY;
    float camY_old;
    float camY_gap;
    bool inCamArea;
    bool teleportTrigger;
    Transform playerTr;
    Transform tr;
    Transform sensorArea, sensorWall_L, sensorWall_R;
    Vector3 baseCamPos;

    public static CameraCtrl_5 instance;

    public void StartTeleport()
    {
        teleportTrigger = true;
    }
    public void EndTeleport()
    {
        tr.position = playerTr.position + baseCamPos;
        teleportTrigger = false;
    }

    void Start()
    {
        instance = this;
        playerTr = PlayerCtrl.instance.transform;

        // GetComponent<VignetteAndChromaticAberration>().intensity = 0.8f;    // 비네팅
        // GetComponent<VignetteAndChromaticAberration>().chromaticAberration = 0.8f;    // 희미

        sensorArea = GameObject.Find("Sensor_Area").transform;
        sensorWall_L = GameObject.Find("Sensor_Wall_L").transform;
        sensorWall_R = GameObject.Find("Sensor_Wall_R").transform;

        tr = gameObject.transform;
        baseCamPos = tr.position - playerTr.position;

        Quaternion tempQuate = Quaternion.Inverse(tr.rotation);
        sensorWall_L.localRotation = tempQuate;
        sensorWall_R.localRotation = tempQuate;
        sensorArea.localRotation = tempQuate;

        originCamSpeed = camSpeed;

        camY = 0;
        inCamArea = false;
        teleportTrigger = false;
    }

    void LateUpdate()
    {
        sensorArea.transform.position = PlayerCtrl.instance.transform.position + new Vector3(0,1);
        Vector3 temp = tr.position;

        // 거리가 급격히 멀어 졌을 때 (죽었을 때 or 텔레포트 됬을 때)
        if (Vector3.Distance(tr.position, playerTr.position) > 30f)
        {
            tr.position = playerTr.position + baseCamPos;
        }
            
        // 벽과 충돌이 있을때
        if (isNearByWall_L || isNearByWall_R)
        {
            NearByWall();
        }
        // 벽과 충돌이 없을 때
        else if(!teleportTrigger)
        {
            if (PlayerCtrl.controller.velocity.y > -30 || inCamArea)     // 빠른 속도로 낙하 중이지 않으면
            {

                temp = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, 0, playerTr.position.z) + baseCamPos, camSpeed * Time.deltaTime);
                temp.y = Mathf.Lerp(tr.position.y, camY + baseCamPos.y, camY_gap * 0.4f * Time.deltaTime);
            }
            else
                //                temp = Vector3.Lerp(tr.position, playerTr.position + baseCamPos, Mathf.Abs(PlayerCtrl.controller.velocity.y) * Time.deltaTime);
            temp = playerTr.position + baseCamPos;

        }

        tr.position = temp;
    }


    void NearByWall()
    {
        // 양쪽 두 벽과 충돌이 있을 때
        if (isNearByWall_L && isNearByWall_R)
        {
            Vector3 tempPos = tr.position;
            tempPos.y = playerTr.position.y + baseCamPos.y;
            tr.position = Vector3.Lerp(tr.position, tempPos, camSpeed * Time.deltaTime);
        }

        // 한쪽 벽과 충돌이 있을 때
        else if (isNearByWall_L || isNearByWall_R)
        {
            if (isNearByWall_L && playerTr.position.x >= player_L_EndPos + baseCamPos.x)
            {
//                tr.position = Vector3.Lerp(tr.position, playerTr.position + NearWallDistance, camSpeed * Time.deltaTime);
                isNearByWall_L = false;
            }
            else if (isNearByWall_R && playerTr.position.x <= player_R_EndPos - baseCamPos.x)
            {
                isNearByWall_R = false;
//                tr.position = Vector3.Lerp(tr.position, playerTr.position + NearWallDistance, camSpeed * Time.deltaTime);
            }
            else
            {
                Vector3 tempPos = tr.position;
                tempPos.y = playerTr.position.y + baseCamPos.y;
                tr.position = Vector3.Lerp(tr.position, tempPos, camSpeed * Time.deltaTime);
            }
        }

    }

    GameObject CamArea;
    bool isCamAreaMove = false;

    IEnumerator GetCamAreaVal()
    {
        while(isCamAreaMove)
        {
            camY = CamArea.GetComponent<CameraArea_2>().val;
            yield return null;
        }
    }

    public void ActiveSensor_Retuen(int index, GameObject returnObjet)
    {
        switch (index)
        {
            case 1:
                {
                    if (returnObjet != null)
                    {
                        if(!returnObjet.GetComponent<CameraArea_2>().moving)
                        {
                            isCamAreaMove = false;
                            camY_old = camY;
                            camY = returnObjet.GetComponent<CameraArea_2>().val;
                            camY_gap = Mathf.Abs(camY - camY_old);
                            Debug.Log(returnObjet.name + " : " + camY);
                        }
                        else
                        {
                            CamArea = returnObjet;
                            isCamAreaMove = true;
                            StartCoroutine(GetCamAreaVal());
                        }

                        inCamArea = true;
                    }
                    else
                    {
                        isCamAreaMove = false;
                        inCamArea = false;
                    }
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
                    NearWallDistance = baseCamPos;

                    float sensor_L_EndPos = tr.position.x - (tr.position.x - sensorWall_L.transform.position.x) - (sensorWall_L.transform.localScale.x * 0.5f);
                    float wall_L_EndPos = returnObjet.transform.position.x + (returnObjet.transform.localScale.x * 0.5f);
                    NearWallDistance.x = wall_L_EndPos - sensor_L_EndPos;
                    player_L_EndPos = playerTr.position.x;
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
                    NearWallDistance = baseCamPos;

                    float sensor_R_EndPos = tr.position.x + (sensorWall_R.transform.position.x - tr.position.x) + (sensorWall_R.transform.localScale.x * 0.5f);
                    float wall_R_EndPos = returnObjet.transform.position.x - (returnObjet.transform.localScale.x * 0.5f);
                    NearWallDistance.x = -(sensor_R_EndPos - wall_R_EndPos);
                    player_R_EndPos = playerTr.position.x;

                }
                break;
        }
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

#region 안쓰는 카메라
// 카메라 1
/*
public class CameraCtrl_5 : MonoBehaviour, Sensorable_Return
{
    public float camSpeed = 5f;

    bool isNearByWall_L = false;
    bool isNearByWall_R = false;

    public Vector3 NearWallDistance;

    private float originCamSpeed;

    float player_L_EndPos;
    float player_R_EndPos;
    float camY;

    Transform playerTr;
    Transform tr;
    Transform sensorArea, sensorWall_L, sensorWall_R;
    Vector3 baseCamPos;

    public static CameraCtrl_5 instance;

    void Start()
    {
        instance = this;
        playerTr = PlayerCtrl.instance.transform;

        sensorArea = GameObject.Find("Sensor_Area_New").transform;
        sensorWall_L = GameObject.Find("Sensor_Wall_L").transform;
        sensorWall_R = GameObject.Find("Sensor_Wall_R").transform;

        tr = gameObject.transform;
        baseCamPos = tr.position - playerTr.position;

        originCamSpeed = camSpeed;

        camY = 0;
    }

    void Update()
    {
        Vector3 temp = tr.position;
        // 거리가 급격히 멀어 졌을 때 (죽었을 때)
        if (Vector3.Distance(tr.position, playerTr.position) > 30f)
            tr.position = playerTr.position;

        // 벽과 충돌이 있을때
        if (isNearByWall_L || isNearByWall_R)
        {
            NearByWall();
        }
        // 벽과 충돌이 없을 때
        else
        {
            //            tr.position = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, camY, playerTr.position.z) + baseCamPos, camSpeed * Time.deltaTime);
            if (PlayerCtrl.controller.velocity.y > -25)
                tr.position = Vector3.Lerp(tr.position, new Vector3(playerTr.position.x, camY, playerTr.position.z) + baseCamPos, camSpeed * Time.deltaTime);
            else
                tr.position = Vector3.Lerp(tr.position, playerTr.position + baseCamPos, 0.9f);
        }
    }


    void NearByWall()
    {
        // 양쪽 두 벽과 충돌이 있을 때

        if (isNearByWall_L && isNearByWall_R)
        {
            Vector3 tempPos = tr.position;
            tempPos.y = playerTr.position.y + baseCamPos.y;
            tr.position = Vector3.Lerp(tr.position, tempPos, camSpeed * Time.deltaTime);
        }

        // 한쪽 벽과 충돌이 있을 때
        else if (isNearByWall_L || isNearByWall_R)
        {
            if (isNearByWall_L && playerTr.position.x >= player_L_EndPos + baseCamPos.x)
            {
                tr.position = Vector3.Lerp(tr.position, playerTr.position + NearWallDistance, camSpeed * Time.deltaTime);
            }
            else if (isNearByWall_R && playerTr.position.x <= player_R_EndPos - baseCamPos.x)
            {
                tr.position = Vector3.Lerp(tr.position, playerTr.position + NearWallDistance, camSpeed * Time.deltaTime);
            }
            else
            {
                Vector3 tempPos = tr.position;
                tempPos.y = playerTr.position.y + baseCamPos.y;
                tr.position = Vector3.Lerp(tr.position, tempPos, camSpeed * Time.deltaTime);
            }
        }

    }


    public void ActiveSensor_Retuen(int index, GameObject returnObjet)
    {
        switch (index)
        {
            case 1:
                {
                    Debug.Log(returnObjet);
                    if (returnObjet != null)
                    {
                        
                        camY = returnObjet.GetComponent<Test_Cam>().val;
                    }
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
                    NearWallDistance = baseCamPos;

                    float sensor_L_EndPos = tr.position.x - (tr.position.x - sensorWall_L.transform.position.x) - (sensorWall_L.transform.localScale.x * 0.5f);
                    float wall_L_EndPos = returnObjet.transform.position.x + (returnObjet.transform.localScale.x * 0.5f);
                    NearWallDistance.x = wall_L_EndPos - sensor_L_EndPos;
                    player_L_EndPos = playerTr.position.x;
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
                    NearWallDistance = baseCamPos;

                    float sensor_R_EndPos = tr.position.x + (sensorWall_R.transform.position.x - tr.position.x) + (sensorWall_R.transform.localScale.x * 0.5f);
                    float wall_R_EndPos = returnObjet.transform.position.x - (returnObjet.transform.localScale.x * 0.5f);
                    NearWallDistance.x = -(sensor_R_EndPos - wall_R_EndPos);
                    player_R_EndPos = playerTr.position.x;
                }
                break;
        }
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
*/


// 카메라 2
/*
public class CameraCtrl_5 : MonoBehaviour, Sensorable_Return
{
    public float camSpeed = 5f;

    bool isNearByWall_L = false;
    bool isNearByWall_R = false;

    public Vector3 NearWallDistance;

    private float originCamSpeed;

    float player_L_EndPos;
    float player_R_EndPos;

    Transform playerTr;
    Transform tr;
    Transform sensorArea, sensorWall_L, sensorWall_R;
    Vector3 baseCamPos;

    public static CameraCtrl_5 instance;

    void Start()
    {
        instance = this;
        playerTr = PlayerCtrl.instance.transform;

        sensorArea = GameObject.Find("Sensor_Area_New").transform;
        sensorWall_L = GameObject.Find("Sensor_Wall_L").transform;
        sensorWall_R = GameObject.Find("Sensor_Wall_R").transform;

        tr = gameObject.transform;
        baseCamPos = tr.position - playerTr.position;

        originCamSpeed = camSpeed;

    }

    void Update()
    {
        // 거리가 급격히 멀어 졌을 때 (죽었을 때)
        if (Vector3.Distance(tr.position, playerTr.position) > 30f)
            tr.position = playerTr.position;

        // 벽과 충돌이 있을때
        if (isNearByWall_L || isNearByWall_R)
        {
            NearByWall();
        }
        // 벽과 충돌이 없을 때
        else
        {
            tr.position = Vector3.Lerp(tr.position, playerTr.position + baseCamPos, camSpeed * Time.deltaTime);
        }
    }


    void NearByWall()
    {
        // 양쪽 두 벽과 충돌이 있을 때

        if (isNearByWall_L && isNearByWall_R)
        {
            Vector3 tempPos = tr.position;
            tempPos.y = playerTr.position.y + baseCamPos.y;
            tr.position = Vector3.Lerp(tr.position, tempPos, camSpeed * Time.deltaTime);
        }

        // 한쪽 벽과 충돌이 있을 때
        else if (isNearByWall_L || isNearByWall_R)
        {
            if (isNearByWall_L && playerTr.position.x >= player_L_EndPos + baseCamPos.x)
            {
                tr.position = Vector3.Lerp(tr.position, playerTr.position + NearWallDistance, camSpeed * Time.deltaTime);
            }
            else if (isNearByWall_R && playerTr.position.x <= player_R_EndPos - baseCamPos.x)
            {
                tr.position = Vector3.Lerp(tr.position, playerTr.position + NearWallDistance, camSpeed * Time.deltaTime);
            }
            else
            {
                Vector3 tempPos = tr.position;
                tempPos.y = playerTr.position.y + baseCamPos.y;
                tr.position = Vector3.Lerp(tr.position, tempPos, camSpeed * Time.deltaTime);
            }
        }

    }


    public void ActiveSensor_Retuen(int index, GameObject returnObjet)
    {
        switch (index)
        {

            case 2:
                if (returnObjet == null)
                {
                    isNearByWall_L = false;
                }
                else
                {
                    isNearByWall_L = true;
                    NearWallDistance = baseCamPos;

                    float sensor_L_EndPos = tr.position.x - (tr.position.x - sensorWall_L.transform.position.x) - (sensorWall_L.transform.localScale.x * 0.5f);
                    float wall_L_EndPos = returnObjet.transform.position.x + (returnObjet.transform.localScale.x * 0.5f);
                    NearWallDistance.x = wall_L_EndPos - sensor_L_EndPos;
                    player_L_EndPos = playerTr.position.x;
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
                    NearWallDistance = baseCamPos;

                    float sensor_R_EndPos = tr.position.x + (sensorWall_R.transform.position.x - tr.position.x) + (sensorWall_R.transform.localScale.x * 0.5f);
                    float wall_R_EndPos = returnObjet.transform.position.x - (returnObjet.transform.localScale.x * 0.5f);
                    NearWallDistance.x = -(sensor_R_EndPos - wall_R_EndPos);
                    player_R_EndPos = playerTr.position.x;
                }
                break;
        }
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
*/

// 카메라 3
/*
 public class CameraCtrl_5 : MonoBehaviour, Sensorable_Return
{
    public float camSpeed = 5f;

    bool isNearByWall_L = false;
    bool isNearByWall_R = false;

    public Vector3 NearWallDistance;

    private float originCamSpeed;

    float player_L_EndPos;
    float player_R_EndPos;

    Transform playerTr;
    Transform tr;
    Transform sensorArea, sensorWall_L, sensorWall_R;
    Vector3 baseCamPos;

    public static CameraCtrl_5 instance;

    void Start()
    {
        instance = this;
        playerTr = PlayerCtrl.instance.transform;

        sensorArea = GameObject.Find("Sensor_Area_New").transform;
        sensorWall_L = GameObject.Find("Sensor_Wall_L").transform;
        sensorWall_R = GameObject.Find("Sensor_Wall_R").transform;

        tr = gameObject.transform;
        baseCamPos = tr.position - playerTr.position;

        originCamSpeed = camSpeed;

    }

    void Update()
    {
        Vector3 temp = tr.position;

        // 거리가 급격히 멀어 졌을 때 (죽었을 때)
        if (Vector3.Distance(tr.position, playerTr.position) > 30f)
            tr.position = playerTr.position;

        // 벽과 충돌이 있을때
        if (isNearByWall_L || isNearByWall_R)
        {
            NearByWall();
        }
        // 벽과 충돌이 없을 때
        else
        {
            temp = Vector3.Lerp(tr.position, new Vector3 (playerTr.position.x, 0, playerTr.position.z) + baseCamPos, camSpeed * Time.deltaTime);
        }

        temp.y = Mathf.Lerp(tr.position.y, playerTr.position.y + baseCamPos.y, camSpeed * 0.5f * Time.deltaTime);
        tr.position = temp;
    }


    void NearByWall()
    {
        // 양쪽 두 벽과 충돌이 있을 때

        if (isNearByWall_L && isNearByWall_R)
        {
            Vector3 tempPos = tr.position;
            tempPos.y = playerTr.position.y + baseCamPos.y;
            tr.position = Vector3.Lerp(tr.position, tempPos, camSpeed * Time.deltaTime);
        }

        // 한쪽 벽과 충돌이 있을 때
        else if (isNearByWall_L || isNearByWall_R)
        {
            if (isNearByWall_L && playerTr.position.x >= player_L_EndPos + baseCamPos.x)
            {
                tr.position = Vector3.Lerp(tr.position, playerTr.position + NearWallDistance, camSpeed * Time.deltaTime);
            }
            else if (isNearByWall_R && playerTr.position.x <= player_R_EndPos - baseCamPos.x)
            {
                tr.position = Vector3.Lerp(tr.position, playerTr.position + NearWallDistance, camSpeed * Time.deltaTime);
            }
            else
            {
                Vector3 tempPos = tr.position;
                tempPos.y = playerTr.position.y + baseCamPos.y;
                tr.position = Vector3.Lerp(tr.position, tempPos, camSpeed * Time.deltaTime);
            }
        }

    }


    public void ActiveSensor_Retuen(int index, GameObject returnObjet)
    {
        switch (index)
        {

            case 2:
                if (returnObjet == null)
                {
                    isNearByWall_L = false;
                }
                else
                {
                    isNearByWall_L = true;
                    NearWallDistance = baseCamPos;

                    float sensor_L_EndPos = tr.position.x - (tr.position.x - sensorWall_L.transform.position.x) - (sensorWall_L.transform.localScale.x * 0.5f);
                    float wall_L_EndPos = returnObjet.transform.position.x + (returnObjet.transform.localScale.x * 0.5f);
                    NearWallDistance.x = wall_L_EndPos - sensor_L_EndPos;
                    player_L_EndPos = playerTr.position.x;
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
                    NearWallDistance = baseCamPos;

                    float sensor_R_EndPos = tr.position.x + (sensorWall_R.transform.position.x - tr.position.x) + (sensorWall_R.transform.localScale.x * 0.5f);
                    float wall_R_EndPos = returnObjet.transform.position.x - (returnObjet.transform.localScale.x * 0.5f);
                    NearWallDistance.x = -(sensor_R_EndPos - wall_R_EndPos);
                    player_R_EndPos = playerTr.position.x;
                }
                break;
        }
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
*/
#endregion