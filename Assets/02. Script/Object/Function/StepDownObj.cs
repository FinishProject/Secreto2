using UnityEngine;
using System.Collections;

public class StepDownObj : MonoBehaviour {

    public float maxLength = 10f;
    public float downSpeed = 3f;
    public float upSpeed = 2f;
    private bool isBack = false;

    private Vector3 originPos;

    void Start()
    {
        originPos = this.transform.position;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isBack = false;

            transform.position += Vector3.down * downSpeed * Time.deltaTime;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isBack = true;
            StartCoroutine(ReturnPosition());
        }
    }

    IEnumerator ReturnPosition()
    {
        while (isBack)
        {
            transform.position = Vector3.MoveTowards(transform.position, originPos, upSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
