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

    private Transform playerTr;
    private Vector3[] gapPos;
    private Vector3[] originPos;

    private float laserSpeed = 100f;
    public float[] blockSpeed;
    public float shakeAmount = 0.1f;
    private float moveDir = 1f;
    private bool isShot = false;

    private float rndSpeed = 0f;
    private bool isMove = false;

    void Start()
    {
        playerTr = PlayerCtrl.instance.transform;

        originPos = new Vector3[blocks.Length];
        for (int i = 0; i < originPos.Length; i++)
        {
            originPos[i] = blocks[i].position;
        }

        gapPos = new Vector3[blocks.Length];

        gapPos[0] = new Vector3(blocks[0].position.x - 0.3f, blocks[0].position.y + 0.3f, blocks[0].position.z);
        gapPos[1] = new Vector3(blocks[1].position.x - 0.3f, blocks[1].position.y - 0.3f, blocks[1].position.z);
        gapPos[2] = new Vector3(blocks[2].position.x + 0.3f, blocks[2].position.y, blocks[2].position.z);

        StartCoroutine(MovementBlock());
    }

    IEnumerator ShotLaser()
    {
        laser.gameObject.SetActive(true);
        Vector3 targetPos = playerTr.position;

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
                //state = ChaseState.CHARGE;

                yield return FadeLaser(1, 0);
                targetPos = finishPoint.position;
                laserSpeed = 0.5f;

                state = ChaseState.SHOT;
            }

            // 레이저와 타겟에 각도를 구함
            float angle = Vector3.Angle(laser.transform.right, (laser.position - targetPos));

            // 일정 각도 안에 들어왔을 시 레이저 삭제
            if(angle <= 97f && state == ChaseState.SHOT)
            {
                state = ChaseState.IDLE;

                yield return FadeLaser(-1, 1);

                laser.gameObject.SetActive(false);
                laserSpeed = 100f;
                targetPos = playerTr.position;

                // 플레이어가 범위 안에 나갔을 시 레이저 종료
                if (!isShot)
                    break;

                yield return new WaitForSeconds(4f);
                laser.gameObject.SetActive(true);
            }

            yield return null;
        }
    }

    IEnumerator FadeLaser(float fadeDir, float alpha)
    {
        Color color = laserRender.material.color;
        Vector3 origin = transform.localPosition;
        while (true)
        {
            transform.localPosition = origin + Random.insideUnitSphere * shakeAmount;

            alpha += fadeDir * 0.6f * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            color.a = alpha;
            laserRender.material.color = color;

            if (alpha == 1 || alpha == 0)
                break;

            yield return null;
        }

        transform.localPosition = origin;
    }


    IEnumerator MovementBlock()
    {

        while (true)
        {
            switch (state)
            {
                case ChaseState.IDLE:
                    yield return IdleMove();
                    break;
                case ChaseState.SHOT:
                    yield return ShotMove();
                    break;
            }

            yield return null;
        }
    }

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

    IEnumerator ShotMove()
    {
        float countTime = 0f;
        while (true)
        {
            if (state == ChaseState.SHOT)
            {
                for (int i = 0; i < blocks.Length; i++)
                    blocks[i].position = Vector3.Lerp(blocks[i].position, gapPos[i], 5f * Time.deltaTime);
            }
            else if(state == ChaseState.IDLE)
            {
                for (int i = 0; i < blocks.Length; i++)
                    blocks[i].position = Vector3.Lerp(blocks[i].position, originPos[i], 2f * Time.deltaTime);

                countTime += Time.deltaTime;

                if (countTime >= 1f)
                    break;
            }

            yield return null;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isShot = true;
            
            StartCoroutine(ShotLaser());
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isShot = false;
        }
    }
}
