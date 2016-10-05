using UnityEngine;
using System.Collections;

public class VerticalHold : MonoBehaviour {

    private Transform playerTr;
    private Vector3 maxLengthPos, originPos;

    public float speed = 3f;
    public float length = 8f;

    Vector3 moveDir = Vector3.zero;

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        originPos.y = transform.position.y; // 기본 이동 길이(하)
        maxLengthPos.y = originPos.y + length; //최대 이동 길이(상)
    }

    void FixedUpdate()
    {
        if (transform.position.y >= maxLengthPos.y && speed >= 1) { speed *= -1; }
        else if (transform.position.y <= originPos.y && speed <= -1) { speed *= -1; }

        moveDir = Vector3.up * speed;

        moveDir = transform.TransformDirection(moveDir);
        transform.position += new Vector3(0, speed * Time.deltaTime, 0);
    }

    void OnTriggerStay(Collider coll)
    {
        // 플레이어가 발판 위에 있을 시 발판과 같이 이동
        if (coll.CompareTag("Player"))
        {
            WahleCtrl.curState = WahleCtrl.instance.StepHold();
            playerTr.Translate(Vector3.up * speed * Time.deltaTime);
            WahleCtrl.instance.StepHold();
        }
    }

    void OnTriggerExit(Collider coll)
    {
        WahleCtrl.instance.ChangeState(WahleState.MOVE);
    }
}
