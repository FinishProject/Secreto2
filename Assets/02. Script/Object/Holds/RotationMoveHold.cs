using UnityEngine;
using System.Collections;

public class RotationMoveHold : MonoBehaviour {
    public Transform hold;     // 발판
    public float speed = 3;    // 속도
    public float x_MoveRatio;  // x 이동비율
    public float startAngel;
    float angle = 0;           // 각도
    float ragne;               // 중심점으로 부터의 거리

    public Vector3 tempPos;

    // Use this for initialization
    void Start () {
        ragne = Vector3.Distance(transform.position, hold.position);
        angle = startAngel;
    }
	
	// Update is called once per frame
	void Update () {
        angle += speed;
        if (angle > 360)
            angle = 0;

        tempPos.x = Mathf.Cos(angle * Mathf.PI / 180) * ragne * x_MoveRatio;
        tempPos.y = Mathf.Sin(angle * Mathf.PI / 180) * ragne;
        tempPos.z = 0;
        hold.transform.position = tempPos + transform.position;
    }

    public float GetSpeed()
    {
        return speed;
    }

    
}
