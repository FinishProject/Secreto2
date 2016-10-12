using UnityEngine;
using System.Collections;

public class RotateHold : MonoBehaviour {


    public Transform point;

    private Vector3 centerPoint;
    private float rotDir = 1f;

    private float startTime = 0f;

    private Quaternion qStart, qEnd;
    private float angle = 0f;

    void Start()
    {
        centerPoint = transform.position;

        angle = (Mathf.Atan2(transform.position.z, transform.position.z) * Mathf.Rad2Deg);

        qStart = Quaternion.AngleAxis(-angle, Vector3.forward);
        qEnd = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Update()
    {

        //startTime += Time.deltaTime;

        //float moveSpeed = (Mathf.Sin(startTime * 2f) * 50f);

        //float rotSpeed = (Mathf.Sin(startTime * 2f + Mathf.PI * 0.5f) + 1f) * 0.5f;

        //transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        //transform.rotation = Quaternion.Lerp(qStart, qEnd, rotSpeed);

        transform.LookAt(Vector3.right);
    }

    //void OnTriggerStay(Collider col)
    //{
    //    if (col.CompareTag("Player"))
    //    {
    //        Rigidbody rb = GetComponent<Rigidbody>();
    //        Vector3 stepDir = col.ClosestPointOnBounds(col.gameObject.transform.position);

    //        if (stepDir.x <= transform.position.x)
    //        {
    //            rotDir = 1f;
    //        }
    //        else if (stepDir.x >= transform.position.x)
    //            rotDir = -1f;

    //        transform.Rotate(Vector3.up * rotDir * 10f * Time.deltaTime);
    //    }
    //}
}
