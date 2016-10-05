using UnityEngine;
using System.Collections;

public class SlantMushroom : MonoBehaviour {

    private Rigidbody rb;
    public float power = 10f;
    public float retrunPower = 3f;
    public float slope = 0.09f;
    private Quaternion origin;
    private Vector3 dir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        origin = this.transform.rotation;
    }

    void OnTriggerStay(Collider coll)
    {
        if (transform.rotation.y <= slope)
        {
            rb.constraints = RigidbodyConstraints.None;
            //플레이어와 접촉 되어 있는 부분에서 아래로 힘을 가함
            dir = coll.ClosestPointOnBounds(coll.gameObject.transform.position);
            rb.AddForceAtPosition(-Vector3.up * power * Time.deltaTime, dir);
        }
        else { rb.constraints = RigidbodyConstraints.FreezeRotation; }
    }
    void OnTriggerExit(Collider coll)
    {
        //rb.constraints = RigidbodyConstraints.FreezeRotation;
        StartCoroutine("RollBack");
    }
    //버섯이 원위치로 돌아감
    IEnumerator RollBack()
    {
        rb.constraints = RigidbodyConstraints.None;
        while (transform.rotation.y >= origin.y)
        {
            rb.AddForceAtPosition(Vector3.up * (power * retrunPower) * Time.deltaTime, dir);
            yield return new WaitForSeconds(0.1f);
        }
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
