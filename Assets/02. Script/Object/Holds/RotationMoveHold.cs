using UnityEngine;
using System.Collections;

public class RotationMoveHold : MonoBehaviour {
    public Transform hold;     // 발판
    Transform playerTr;
    public float speed = 3;    // 속도
    public float x_MoveRatio;  // x 이동비율
    public float startAngel;
    float angle = 0;           // 각도
    float ragne;               // 중심점으로 부터의 거리

    float oldX;
    bool isOn;
    
    public Vector3 tempPos;
    float playerRange;
    // Use this for initialization
    void Start () {
        ragne = Vector3.Distance(transform.position, hold.position);
        angle = startAngel;
        playerTr = PlayerCtrl.instance.transform;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        angle += speed;
        if (angle > 360)
            angle = 0;

        tempPos.x = Mathf.Cos(angle * Mathf.PI / 180) * ragne * x_MoveRatio;
        tempPos.y = Mathf.Sin(angle * Mathf.PI / 180) * ragne;
        tempPos.z = 0;
        

        hold.transform.position = tempPos + transform.position;
        if (isOn)
        {
            playerTr.Translate(Vector3.forward * (tempPos.x - oldX));
        }
        oldX = tempPos.x;
    }

    public float GetSpeed()
    {
        return speed;
    }

    void OnTriggerEnter(Collider col)
    {
        // 플레이어가 발판 위에 있을 시 발판과 같이 이동
        if (col.CompareTag("Player"))
        {
            isOn = true;
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            isOn = false;
        }
    }

}
