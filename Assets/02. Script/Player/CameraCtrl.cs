using UnityEngine;
using System.Collections;

public class CameraCtrl : MonoBehaviour {

    public Transform playerTr; // 플레이어의 위치값
    public static float speed = 10f;  // 카메라의 속도

    private Vector3 relCameraPos;
    private Vector3 standardPos;

    private int index = 0;
    Vector3 curVec;
    bool focusRight = true;

    float focusDir;
    float moveRange = 0;

    void Awake()
    {
        relCameraPos = transform.position - playerTr.position;
        curVec = playerTr.position;
    }

    void Update()
    {
        focusDir = PlayerCtrl.inputAxis;
        standardPos = FocusPlayerVec() + relCameraPos;
        transform.position = Vector3.Lerp(transform.position, standardPos, speed * Time.deltaTime);
    }

    Vector3 FocusPlayerVec()
    {
        if (focusDir >= 0.9f || moveRange >= 10)
        {
            curVec = new Vector3(playerTr.position.x + 3f, playerTr.position.y, playerTr.position.z);
            moveRange = 0;
        }
        else if (focusDir <= -0.9f || moveRange <= -10)
        {
            curVec = new Vector3(playerTr.position.x - 5f, playerTr.position.y, playerTr.position.z);
            moveRange = 0;
        }
        else if (focusDir != 0)
        {
            moveRange += focusDir;
        }
        curVec.y = playerTr.position.y;

        return curVec;
    }
}
