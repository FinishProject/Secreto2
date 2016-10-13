using UnityEngine;
using System.Collections;

public class MoveLaser : MonoBehaviour {


    public float length = 10f;
    public float speed = 2f;

    float moveDir = -1f;
    float originPosX, finishPosX;
    private float moveSpeed = 0f;
    private float startTime = 0f;

    void Start()
    {
        originPosX = this.transform.position.x;
        finishPosX = originPosX + length;
    }

    void Update()
    {

        if (transform.position.x >= finishPosX)
        {
            moveDir = 1f;
        }
        else if (transform.position.x <= originPosX)
            moveDir = -1f;


        transform.Translate(Vector3.up * (speed * moveDir) * Time.deltaTime);
    }
}
