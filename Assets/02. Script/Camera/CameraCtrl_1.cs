using UnityEngine;
using System.Collections;

public class CameraCtrl_1 : MonoBehaviour
{

    Transform tr;   // 현재 Transform
    Transform playerTr;   // 현재 Transform
    Vector3 camAddPos;
  
    void Start()
    {
        tr = transform;
        playerTr = PlayerCtrl.instance.transform;
        camAddPos = tr.position - playerTr.position;
    }

    // Update is called once per frame
    void Update()
    {
        float tempSpeed = Vector3.Distance(tr.position, playerTr.position + camAddPos) * 2.5f;
        tr.position = Vector3.Lerp(tr.position, playerTr.position + camAddPos, tempSpeed * Time.deltaTime);


    }


}
