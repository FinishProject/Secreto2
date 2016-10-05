using UnityEngine;
using System.Collections;

public class FullFushObject : MonoBehaviour {

    private float speed = 20f;
    private bool isStep = false;

    void FullObject()
    {
        // 플레이어가 밟고 있을 시 인력 불가
        if (!isStep)
        {
            Transform wahle = GameObject.FindWithTag("WAHLE").transform;

            Vector3 relativePos = wahle.position - transform.position;
            transform.position = Vector3.Lerp(transform.position, wahle.position, speed * Time.deltaTime);
            //transform.Translate(relativePos.normalized * 20f * Time.deltaTime);
        }
    }

    void FushObject()
    {
        Transform wahle = GameObject.FindWithTag("WAHLE").transform;

        Vector3 relativePos = wahle.position - transform.position;
        transform.position = Vector3.Lerp(new Vector3(transform.position.x, 2f, 0f), 
            new Vector3(relativePos.x, 0f, 0f), 1f * Time.deltaTime);
        //transform.Translate(new Vector3(-relativePos.normalized.x * speed * Time.deltaTime,
        //    -relativePos.normalized.x * speed * Time.deltaTime, 0f));
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
            isStep = true;
    }

    void OnTriggerExit(Collider col)
    {
        isStep = false;
    }
}
