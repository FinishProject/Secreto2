using UnityEngine;
using System.Collections;

public class RepeatHold : MonoBehaviour {

    public float speed = 3f;
    public float length = 3f;
    private bool isOn = false;

    private Vector3 moveDir = Vector3.zero;
    private Vector3 originPos;

    private float anlge = 0f;

    private float oldX;

    void Start()
    {
        originPos = transform.position;
    }

    void Update()
    {
        anlge += speed;
        if (anlge > 360)
            anlge = 0f;

        moveDir.x = Mathf.Sin(anlge * Mathf.PI / 180f) * length;

        //moveDir.x = Mathf.Sin(speed * Time.time) * length;

        transform.position = originPos + moveDir;

        // 발판 방향으로 동일하게 플레이어 이동
        if (isOn)
            PlayerCtrl.instance.transform.Translate(Vector3.forward * (moveDir.x - oldX) * -PlayerCtrl.focusRight);

        oldX = moveDir.x;
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isOn = true;
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
