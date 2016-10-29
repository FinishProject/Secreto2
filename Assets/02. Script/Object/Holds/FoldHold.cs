using UnityEngine;
using System.Collections;

public class FoldHold : MonoBehaviour {


    public float speed = 1f;
    public float length = 3f;
    public float moveDir = 1;

    private Vector3 originPos;

    void Start()
    {
        originPos = this.transform.position;
    }

    void Update()
    {
        transform.position = new Vector3(moveDir * Mathf.PingPong(speed * Time.time, length) + originPos.x, transform.position.y, transform.position.z);
    }


    //public float length = 10f;
    //public float speed = 2f;
    //public bool isRight = true;

    //private float moveSpeed = 0f;
    //private float moveDir = -1f;
    //private bool isOn = false;
    //public bool isActive = false;
    //float startTime = 0f;

    //private Transform playerTr;
    //private Vector3 originPos, finishPos;

    //void Start()
    //{
    //    playerTr = GameObject.FindGameObjectWithTag("Player").transform;

    //    originPos = this.transform.position;

    //    finishPos = originPos;
    //    finishPos.x += length;

    //    if (!isRight)
    //        speed *= moveDir;
    //}

    //IEnumerator Movement()
    //{
    //    yield return new WaitForSeconds(2f);
    //    while (isActive)
    //    {
    //        startTime += Time.deltaTime;

    //        moveSpeed = (Mathf.Sin(startTime * speed) * length);

    //        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

    //        if (isOn)
    //            playerTr.Translate(Vector3.forward * (moveSpeed * -PlayerCtrl.focusRight) * Time.deltaTime);

    //        yield return null;
    //    }
    //}

    //public void StartMove()
    //{
    //    isActive = true;
    //    StartCoroutine(Movement());
    //}

    //void OnTriggerEnter(Collider col)
    //{
    //    if (col.CompareTag("Player"))
    //    {
    //        isOn = true;
    //        WahleCtrl.curState = WahleCtrl.instance.StepHold();
    //    }
    //}

    //void OnTriggerExit(Collider col)
    //{
    //    if (col.CompareTag("Player"))
    //    {
    //        isOn = false;
    //        WahleCtrl.instance.ChangeState(WahleState.MOVE);
    //    }
    //}
}
