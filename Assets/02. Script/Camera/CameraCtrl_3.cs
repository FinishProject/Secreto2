using UnityEngine;
using System.Collections;

public class CameraCtrl_3 : MonoBehaviour
{

    Transform tr;         // 현재 카메라 Transform
    Transform playerTr;   // 현재 플레이어 Transform
    Vector3 camAddPos;    // 플레이어와 카메라의 벡터값

    float camAddPosX;
    float camAddPosY;
    float camAddPosUpY;
    float camAddPosDownY;

    float curYspeed;
    float val = 20f;
    float orignYspeed = 5f;
    float upYspeed = 1f;
    float downYspeed = 20f;

    void Start()
    {
        tr = transform;
        playerTr = PlayerCtrl.instance.transform;
        camAddPos = tr.position - playerTr.position;
        camAddPosY = camAddPos.y;
        camAddPosUpY = camAddPos.y + 0.8f;
        camAddPosDownY = camAddPos.y - 1f;
    }

    void Yspeed()
    {
        if (PlayerCtrl.controller.isGrounded)
        {
            camAddPos.y = camAddPosY;

            if (curYspeed - 0.1f > orignYspeed)
            {
                curYspeed -= orignYspeed * Time.deltaTime;

            }
            else if (curYspeed + 0.1f < orignYspeed)
            {
                curYspeed += orignYspeed * Time.deltaTime;
            }
        }
        else if (PlayerCtrl.controller.velocity.y > 0)
        {
            camAddPos.y = camAddPosUpY;

            if (curYspeed > upYspeed)
                curYspeed -= 30 * Time.deltaTime;
        }
        else
        {
            camAddPos.y = camAddPosDownY;

            if (curYspeed < downYspeed)
                curYspeed += downYspeed * Time.deltaTime;
        }

    }
    Vector3 tempPos;
    void Update()
    {
        Yspeed();
        float tempSpeed = Vector3.Distance(tr.position, playerTr.position + camAddPos) * 2.5f;
        tempPos = Vector3.Lerp(tr.position, playerTr.position + camAddPos, tempSpeed * Time.deltaTime);
        tempPos.y = Mathf.Lerp(tr.position.y, playerTr.position.y + camAddPos.y, curYspeed * Time.deltaTime);
        tr.position = tempPos;
    }
}