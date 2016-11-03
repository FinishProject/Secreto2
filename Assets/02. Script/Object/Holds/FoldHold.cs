using UnityEngine;
using System.Collections;

public class FoldHold : MonoBehaviour {

    public float speed = 3f;
    public float length = 3f;
    private float angle = 0f;
    private bool isOn = false;

    private Vector3 moveDir;
    
    void Update()
    {
        angle += speed;
        if (angle >= 360f)
            angle = 0f;

        // 발판 이동
        moveDir.x = Mathf.Cos(angle * Mathf.PI / 180) * length;
        transform.Translate(moveDir * Time.deltaTime);

        // 발판 방향으로 동일하게 플레이어 이동
        if (isOn)
            PlayerCtrl.instance.transform.Translate(
                Vector3.forward * (moveDir.x * PlayerCtrl.focusRight) * Time.deltaTime);
    }


    //public void StartMove()
    //{
    //    isActive = true;
    //    StartCoroutine(Movement());
    //}

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isOn = true;
            WahleCtrl.curState = WahleCtrl.instance.StepHold();
        }
    }


    //    public float speed = 1f;
    //    public float length = 3f;
    //    public float moveDir = 1;

    //    private bool isPlayerOn = false;

    //    private Vector3 originPos;
    //    float startTime = 0f;

    //    public bool isActive = false;

    //    private float moveSpeed = 0f;
    //    private Transform playerTr;

    //    Vector3 finishPos;

    //    public bool isRight = true;
    //    private bool isOn = false;

    //    void Start()
    //    {
    //        playerTr = GameObject.FindGameObjectWithTag("Player").transform;

    //        originPos = this.transform.position;

    //        finishPos = originPos;
    //        finishPos.x += length;

    //        if (!isRight)
    //            speed *= moveDir;
    //    }

    //    IEnumerator Movement()
    //    {
    //        //yield return new WaitForSeconds(2f);
    //        while (isActive)
    //        {
    //            startTime += Time.deltaTime;

    //            moveSpeed = (Mathf.Sin(startTime * speed) * length);

    //            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

    //            if (isOn)
    //                playerTr.Translate(Vector3.forward * (moveSpeed * PlayerCtrl.focusRight) * Time.deltaTime);

    //            yield return null;
    //        }
    //    }

    //    void OnTriggerExit(Collider col)
    //    {
    //        if (col.CompareTag("Player"))
    //        {
    //            isOn = false;
    //            WahleCtrl.instance.ChangeState(WahleState.MOVE);
    //        }
    //    }
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
