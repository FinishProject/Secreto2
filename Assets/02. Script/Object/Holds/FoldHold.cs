using UnityEngine;
using System.Collections;

public class FoldHold : MonoBehaviour {

    public float length = 10f;
    public float speed = 2f;
    public bool isRight = true;

    private float moveSpeed = 0f;
    private float moveDir = -1f;
    private float startTime = 0f;
    private bool isOn = false;

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

        if (isOn)
        {
            playerTr.Translate(Vector3.forward * (moveSpeed * -PlayerCtrl.focusRight) * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isOn = true;
            WahleCtrl.curState = WahleCtrl.instance.StepHold();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isOn = false;
            WahleCtrl.instance.ChangeState(WahleState.MOVE);
        }
    }
}
