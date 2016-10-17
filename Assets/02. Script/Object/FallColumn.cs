using UnityEngine;
using System.Collections;

public class FallColumn : MonoBehaviour {


    private bool isActive = true;
    private float getTime = 0f;

    public float move = 1.7f;
    public float rot = 2f;
    float rotSpeed = 0f;
    float startTime = 0f;

    private float angle = 0f;
    public float moveLength = 1f;


	void Update () {
        if (isActive) {
            angle += rot;
            transform.RotateAround(Vector3.left, rot * Time.deltaTime);
            transform.Translate(Vector3.right * move * Time.deltaTime);
        }
    }


    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Land"))
        {
            isActive = false;
            //GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
