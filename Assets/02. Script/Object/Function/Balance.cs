using UnityEngine;
using System.Collections;

public class Balance : MonoBehaviour {

    private Rigidbody rb;
    private Vector3 dir;

    public float power = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerStay(Collider coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            dir = coll.ClosestPointOnBounds(coll.gameObject.transform.position);
            rb.AddForceAtPosition(Vector3.down * power * Time.deltaTime, dir);
        }
    }
}
