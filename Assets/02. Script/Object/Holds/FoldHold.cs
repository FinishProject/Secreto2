using UnityEngine;
using System.Collections;

public class FoldHold : MonoBehaviour {


    public float speed = 1f;
    public float length = 3f;
    public float moveDir = 1;

    private bool isPlayerOn = false;

    private Vector3 originPos;
    float startTime = 0f;

    public bool isActive = false;

    private float moveSpeed = 0f;
    private Transform playerTr;

    Vector3 finishPos;

    public bool isRight = true;
    private bool isOn = false;

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;

        originPos = this.transform.position;

        finishPos = originPos;
        finishPos.x += length;

        if (!isRight)
            speed *= moveDir;
    }

    IEnumerator Movement()
    {
        //yield return new WaitForSeconds(2f);
        while (isActive)
        {
            startTime += Time.deltaTime;

            moveSpeed = (Mathf.Sin(startTime * speed) * length);

            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

            if (isOn)
                playerTr.Translate(Vector3.forward * (moveSpeed * PlayerCtrl.focusRight) * Time.deltaTime);

            yield return null;
        }
    }

    public void StartMove()
    {
        isActive = true;
        StartCoroutine(Movement());
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

    //void Start()
    //{
    //    originPos = transform.position;
    //}

    //void Update()
    //{

    //        float moveSpeed = Mathf.Sin(speed * Time.time) * length;

    //        transform.Translate(Vector3.right * -moveSpeed * Time.deltaTime);

    //        if (isPlayerOn)
    //            PlayerCtrl.instance.transform.Translate(Vector3.forward * (moveSpeed * PlayerCtrl.focusRight) * Time.deltaTime);

    //        //transform.position = new Vector3(moveVector, transform.position.y, transform.position.z);
  
    //}


    //void OnTriggerEnter(Collider col)
    //{
    //    if (col.CompareTag("Player"))
    //    {
    //        isPlayerOn = true;
    //    }
    //}

    //void OnTriggerExit(Collider col)
    //{
    //    if (col.CompareTag("Player"))
    //    {
    //        isPlayerOn = false;
    //    }
    //}

//}
