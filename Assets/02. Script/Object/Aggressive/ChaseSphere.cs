using UnityEngine;
using System.Collections;

public class ChaseSphere : MonoBehaviour {

    enum ChaseState
    {
        IDLE, CHARGE, SHOT,
    }

    ChaseState state = ChaseState.IDLE;

    public Transform laser;
    public Renderer laserRender;
    public Transform[] blocks;
    public Transform finishPoint;
    public Collider childCol;

    private Transform playerTr;
    private Vector3[] fillPos;
    private Vector3[] originPos;

    private AudioSource source;

    private float laserSpeed = 100f;
    public float[] blockSpeed;
    public float shakeAmount = 0.1f;
    private float moveDir = 1f;
    private bool isShot = false;
    public float reloadTime = 5f;

    private float rndSpeed = 0f;
    private bool isMove = false;

    void Start()
    {
        source = GetComponent<AudioSource>();
        playerTr = PlayerCtrl.instance.transform;

        // 초기 위치 설정
        originPos = new Vector3[blocks.Length];
        for (int i = 0; i < originPos.Length; i++)
        {
            originPos[i] = blocks[i].position;
        }

        // 오브젝트들 모이는 위치 설정
        fillPos = new Vector3[blocks.Length];

        fillPos[0] = new Vector3(blocks[0].position.x + 0.41f, blocks[0].position.y - 0.16f, blocks[0].position.z);
        fillPos[1] = new Vector3(blocks[1].position.x - 0.371f, blocks[1].position.y - 0.204f, blocks[1].position.z);
        fillPos[2] = new Vector3(blocks[2].position.x - 0.2f, blocks[2].position.y + 0.2f, blocks[2].position.z - 0.204f);
        fillPos[3] = new Vector3(blocks[3].position.x + 0.1f, blocks[3].position.y - 0.34f, blocks[3].position.z - 0.53f);
        fillPos[4] = new Vector3(blocks[4].position.x - 0.269f, blocks[4].position.y - 0.027f, blocks[4].position.z + 1.634f);
        fillPos[5] = new Vector3(blocks[5].position.x + 0.429f, blocks[5].position.y + 0.389f, blocks[5].position.z + 0.864f);

        StartCoroutine(MovementBlock());
    }

    IEnumerator ShotLaser()
    {
        laser.gameObject.SetActive(true);
        Vector3 targetPos = playerTr.position;
        float curDir = 0f;

        while (true)
        {
            // 타겟 추적
            Quaternion targetRot = Quaternion.LookRotation(targetPos - laser.position, Vector3.forward);
            targetRot.x = 0f;
            targetRot.y = 0f;

            laser.transform.rotation = Quaternion.Slerp(laser.rotation, targetRot, laserSpeed * Time.deltaTime);

            // 레이저 생성 및 타겟 방향을 종료 방향으로 바꿈
            if (state == ChaseState.IDLE)
            {
                // 충전
                state = ChaseState.CHARGE;

                curDir = Mathf.Sign(playerTr.position.x - transform.position.x);

                // 레이저 이동 목표 방향 설정
                if (curDir == 1 && transform.position.x <= finishPoint.position.x)
                    finishPoint.position += Vector3.right * -10f;
                else if(curDir == -1 && transform.position.x >= finishPoint.position.x)
                    finishPoint.position += Vector3.right * 10f;

                yield return FadeLaser(1, 0);

                // 발사
                state = ChaseState.SHOT;

                targetPos = finishPoint.position;
                laserSpeed = 0.5f;

                source.volume = 1f;
                source.Play();
                childCol.enabled = true;
            }

            // 레이저와 타겟에 각도를 구함
            float angle = Vector3.Angle(laser.transform.right, (laser.position - targetPos));

            // 일정 각도 안에 들어왔을 시 레이저 삭제
            if ((curDir == 1 && angle >= 87f
                || curDir == -1 && angle <= 97f) && state == ChaseState.SHOT || !isShot)
            {
                state = ChaseState.IDLE;
                childCol.enabled = false;

                if(isShot)
                    yield return FadeLaser(-1, 1);

                source.Stop();

                laser.gameObject.SetActive(false);
                laserSpeed = 100f;

                //플레이어가 범위 안에 나갔을 시 레이저 종료
                if (!isShot)
                {
                    Color color = laserRender.material.color;
                    color.a = 0f;
                    laserRender.material.color = color;
                    break;
                }

                yield return new WaitForSeconds(reloadTime);
                targetPos = playerTr.position;
                laser.gameObject.SetActive(true);
            }

            yield return null;
        }
    }

    // 레이저 알파값 조절
    IEnumerator FadeLaser(float fadeDir, float alpha)
    {
        Color color = laserRender.material.color;
        float fadeSpeed = 0.6f;
        bool isCharge = true;
        while (true)
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            color.a = alpha;
            laserRender.material.color = color;

            if (fadeDir == 1f && alpha >= 0.3f && isCharge)
            {
                isCharge = false;
                yield return new WaitForSeconds(1f);
                fadeSpeed = 2f;
            }

            if (fadeDir == -1f)
                source.volume = alpha;

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

            for (int i = 0; i < blocks.Length; i++)
            {
                float speed = moveSpeed * blockSpeed[i];
                speed *= 0.1f;
                blocks[i].Translate(Vector3.right * (speed * moveDir) * Time.deltaTime);
                moveDir *= -1f;
            }
            moveDir = 1f;

            yield return null;
        }
    }

    IEnumerator ChargeMove()
    {
        while (state == ChaseState.CHARGE)
        {
            for (int i = 0; i < 6; i++)
            {
                blocks[i].position = Vector3.Lerp(blocks[i].position,
                    fillPos[i], 2f * Time.deltaTime);
            }

            yield return null;
        }
    }

    // 레이저 발사 시 블록들 외각으로 벌어짐
    IEnumerator ShotMove()
    {
        while (state == ChaseState.SHOT)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].position = Vector3.Lerp(blocks[i].position, originPos[i], 5f * Time.deltaTime);
                Vector3 oriPos = blocks[i].localPosition;
                blocks[i].localPosition = oriPos + Random.insideUnitSphere * shakeAmount;
            }
            yield return null;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isShot)
        {
            isShot = true;

            StartCoroutine(ShotLaser());
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player") && isShot)
        {
            isShot = false;
        }
    }
}
