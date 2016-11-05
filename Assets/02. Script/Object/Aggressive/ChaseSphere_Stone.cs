using UnityEngine;
using System.Collections;

public class ChaseSphere_Stone : MonoBehaviour {

    enum ChaseState
    {
        IDLE, CHARGE, SHOT,
    }

    ChaseState state = ChaseState.IDLE;

    public Transform laser;
    public Renderer laserRender;
    public Transform[] stones;
    public Transform finishPoint;

    private Transform playerTr;
    private Vector3[] fillPos;
    private Vector3[] originPos;

    public float shakeAmount = 0.1f;
    private float moveDir = 1f;

    private float rndSpeed = 0f;
    private bool isMove = false;

    void Start()
    {
        playerTr = PlayerCtrl.instance.transform;

        // 초기 위치 설정
        originPos = new Vector3[stones.Length];
        for (int i = 0; i < originPos.Length; i++)
        {
            originPos[i] = stones[i].position;
        }

        StartCoroutine(MovementBlock());
    }

    // 레이저 알파값 조절
    IEnumerator FadeLaser(float fadeDir, float alpha)
    {
        Color color = laserRender.material.color;
        while (true)
        {
            alpha += fadeDir * 0.6f * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            color.a = alpha;
            laserRender.material.color = color;

            if (alpha == 1 || alpha == 0)
                break;

            yield return null;
        }
    }

    // 블럭 움직이기
    IEnumerator MovementBlock()
    {
        while (true)
        {

            switch (state)
            {
                case ChaseState.IDLE:
                    yield return IdleMove();
                    break;
                case ChaseState.CHARGE:
                    yield return ChargeMove();
                    break;
                case ChaseState.SHOT:
                    yield return ShotMove();
                    break;
            }

            yield return null;
        }
    }

    // 대기 상태에서 반복하여 움직임
    IEnumerator IdleMove()
    {
        while (state == ChaseState.IDLE)
        {
            float moveSpeed = Mathf.Sin(Time.time);

            for (int i = 0; i < stones.Length; i++)
            {
                float speed = moveSpeed * 2f;
                speed *= 0.1f;
                stones[i].Translate(Vector3.right * (speed * moveDir) * Time.deltaTime);
                moveDir *= -1f;
            }
            moveDir = 1f;

            yield return null;
        }
    }

    IEnumerator ChargeMove()
    {
        Vector3 origin = transform.localPosition;
        while (state == ChaseState.CHARGE)
        {
            transform.localPosition = origin + Random.insideUnitSphere * shakeAmount;
            for (int i = 0; i < 6; i++)
            {
                stones[i].position = Vector3.Lerp(stones[i].position,
                    fillPos[i], 2f * Time.deltaTime);
            }

            yield return null;
        }
        transform.localPosition = origin;
    }

    // 레이저 발사 시 블록들 외각으로 벌어짐
    IEnumerator ShotMove()
    {
        while (state == ChaseState.SHOT)
        {
            for (int i = 0; i < stones.Length; i++)
            {
                stones[i].position = Vector3.Lerp(stones[i].position, originPos[i], 5f * Time.deltaTime);
            }
            yield return null;
        }
    }
}
