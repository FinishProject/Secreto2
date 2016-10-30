using UnityEngine;
using System.Collections;

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