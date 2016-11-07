using UnityEngine;
using System.Collections;

public class FoldHold : MonoBehaviour {


    public float speed = 1f;
    public float length = 3f;
    public float moveDir = 1;

    private bool isPlayerOn = false;

    private Vector3 originPos;
    float startTime = 0f;

    void Update()
    {
        float moveSpeed = Mathf.Sin(speed * Time.time) * length;

        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (isPlayerOn)
            PlayerCtrl.instance.transform.Translate(Vector3.forward * (moveSpeed * PlayerCtrl.focusRight)* Time.deltaTime);

        //    transform.position = new Vector3(moveDir * Mathf.PingPong(speed * Time.time, length) + originPos.x, transform.position.y, transform.position.z);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isPlayerOn = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isPlayerOn = false;
        }
    }

}
