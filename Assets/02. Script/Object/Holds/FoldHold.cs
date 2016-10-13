using UnityEngine;
using System.Collections;

public class FoldHold : MonoBehaviour {

    public float length = 10f;
    public float speed = 2f;
    public bool isRight = true;

    private float moveSpeed = 0f;
    private float moveDir = -1f;
    private float startTime = 0f;

    private Transform playerTr;
    private Vector3 originPos, finishPos;

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;

        originPos = this.transform.position;

        finishPos = originPos;
        finishPos.x += length;

        if (!isRight)
            speed *= moveDir;
    }

    void Update()
    {
        startTime += Time.deltaTime;

        moveSpeed = (Mathf.Sin(startTime * speed) * length);

        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }

    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            WahleCtrl.curState = WahleCtrl.instance.StepHold();
            playerTr.Translate(Vector3.forward * (moveSpeed * -PlayerCtrl.focusRight) * Time.deltaTime);
        }
    }
}
