using UnityEngine;
using System.Collections;

public class FoldHold : MonoBehaviour
{

    public float speed = 3f;
    public float minSpeed = 0.1f;
    public float accel = 2f;
    public float length = 3f;
    private float angle = 0f;
    private bool isOn = false;

    private Vector3 moveDir;
    private Vector3 origin, finishPos;
    private Vector3 targetPos;

    void Start()
    {
        origin = transform.position;
        finishPos = transform.position;
        finishPos.x += length;

        targetPos = finishPos;
    }

    void Update()
    {
        //angle += speed;
        //if (angle > 360f)
        //    angle = 0f;

        //// 발판 이동
        //moveDir.x = Mathf.Cos(angle * Mathf.PI / 180) * length;

        //transform.Translate(moveDir * Time.deltaTime);

        //if(moveDir.x == length)
        //{
        //    angle = 0f;
        //}

        Move();

        // 발판 방향으로 동일하게 플레이어 이동
        if (isOn)
            PlayerCtrl.instance.transform.Translate(
                Vector3.forward * (moveDir.x * PlayerCtrl.focusRight) * Time.deltaTime);
    }

    void Move()
    {
        transform.position = origin + new Vector3(Mathf.Sin(speed  * Time.time) * length, 0, 0);
        //angle += speed;
        //if (angle > 360f)
        //    angle = 0f;

        //// 발판 이동
        ////float moveSpeed = Mathf.Cos(angle * Mathf.PI / 180);


        //speed = DecreaseSpeed(speed, minSpeed, accel);

        //transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);

        //float dis = (targetPos - transform.position).sqrMagnitude;

        //if (dis <= 0.03f)
        //{
        //    if (targetPos.Equals(origin))
        //        targetPos = finishPos;
        //    else
        //        targetPos = origin;

        //    speed = 1f;

        //}

    }

    public float DecreaseSpeed(float initSpeed, float minSpeed, float accel)
    {
        if (initSpeed.Equals(minSpeed))
            return initSpeed;
        else
        {
            initSpeed -= accel * Time.deltaTime;
            // 기본 속도가 최고 속도를 넘을 시 음수가 되어 maxSpeed만 반환
            return (Mathf.Sign(minSpeed - initSpeed).Equals(1)) ? minSpeed : initSpeed;
        }
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isOn = true;
            //WahleCtrl.curState = WahleCtrl.instance.StepHold();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isOn = false;
        }
    }
}
