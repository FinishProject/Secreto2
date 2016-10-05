using UnityEngine;
using System.Collections;

public class PendularMovement : MonoBehaviour {

    public float speed = 1.5f;
    public float moveLength = 5f;

    private float angle = 0f;
    private float moveSpeed = 0f;
    private float rotSpeed = 0f;
    private float startTime = 0f;
    private float upMoveSpeed = 0f;

    private Quaternion qStart, qEnd;
    private Vector3 vEnd;
    private Vector3 rotDirVec;

    void Start()
    {
        rotDirVec = Vector3.forward;
        vEnd = transform.position - (rotDirVec * moveLength);

        // 회전 각도
        angle = (Mathf.Atan2(vEnd.z, transform.position.z) * Mathf.Rad2Deg);
        // 회전 시작 및 도착 위치
        qStart = Quaternion.AngleAxis(-angle, rotDirVec);
        qEnd = Quaternion.AngleAxis(angle, rotDirVec);
    }
    void Update()
    {
        startTime += Time.deltaTime;
        
        moveSpeed = (Mathf.Sin(startTime * speed) * moveLength);
        //upMoveSpeed = (Mathf.Sin(startTime * (speed * 2f)) * 0.5f * moveLength);
        rotSpeed = (Mathf.Sin(startTime * speed + Mathf.PI * 0.5f) + 1f) * 0.5f;

        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        //transform.Translate(-Vector3.up * upMoveSpeed * Time.deltaTime);


        transform.rotation = Quaternion.Lerp(qStart, qEnd, rotSpeed);
    }
}
