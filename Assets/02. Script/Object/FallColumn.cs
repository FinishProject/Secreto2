using UnityEngine;
using System.Collections;

public class FallColumn : MonoBehaviour {


    private bool isActive = true;
	
	// Update is called once per frame
	void Update () {
        if (isActive)
        {
            transform.Translate(Vector3.right * 0.8f * Time.deltaTime);
            transform.RotateAround(Vector3.right, -2f * Time.deltaTime);
        }
    }


    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Land"))
        {
            isActive = false;
            GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
