using UnityEngine;
using System.Collections;

public class FireSphere : BaseSphere {

    public GameObject fireBall;
    public ShotFireball shotFire;

    public Renderer render;
    public ParticleSystem[] particle;
    private Color[] color;

    protected override void Start()
    {
        base.Start();
        color = new Color[2];

        // 초기 위치 저장
        originPos = new Vector3[stones.Length];
        if (originPos.Length > 0)
        {
            originPos = SetBlockOriginPosition(stones);
        }

        // 각 블록들이 벌어질 위치 설정
        fillPos = new Vector3[setFillPositions.Length];
        if (fillPos.Length > 0)
        {
            fillPos = SetBlockFillPosition(stones, setFillPositions);
        }

        StartCoroutine(UpdateSphere());
    }
    
    /* 각 상태에 따른 코루틴 함수들을 업데이트 */
    protected override IEnumerator UpdateSphere()
    {
        while (true)
        {
            switch (state)
            {
                case ChaseState.IDLE:
                    yield return Idle();
                    break;
                case ChaseState.CHARGE:
                    yield return Charge();
                    break;
                case ChaseState.SHOT:
                    yield return ShotMove();
                    break;
            }
            yield return null;
        }
    }

    /* 대기 상태, 각 돌들이 서로 각기의 움직임을 나타내도록 함 */
    protected override IEnumerator Idle()
    {
        while (state == ChaseState.IDLE)
        {
            float moveSpeed = Mathf.Sin(Time.time);

            for (int i = 0; i < stones.Length; i++)
            {
                float speed = moveSpeed * blockSpeed[i];
                speed *= 0.1f;
                stones[i].Translate(Vector3.right * (speed * moveDir) * Time.deltaTime);
                moveDir *= -1f;
            }
            moveDir = 1f;

            yield return null;
        }
    }

    /* 충전 상태, 공격 모션 전의 각 돌들의 움직임을 나타냄. 
     * 돌들이 임의의 방향으로 흔들림 */
    protected override IEnumerator Charge()
    {
        while (state == ChaseState.CHARGE)
        {
            for (int i = 0; i < 6; i++)
            {
                stones[i].position = Vector3.Lerp(stones[i].position,
                    fillPos[i], 2f * Time.deltaTime);
                Vector3 oriPos = stones[i].localPosition;
                stones[i].localPosition = oriPos + Random.insideUnitSphere * shakeAmount;
            }

            yield return null;
        }
    }

    /* 발사 후 상태, 발사체를 발사한 후의 상태.
     * 각 돌들의 벌어질 위치로 이동되어짐. */
    IEnumerator ShotMove()
    {
        while (state == ChaseState.SHOT)
        {
            for (int i = 0; i < stones.Length; i++)
            {
                stones[i].position = Vector3.Lerp(stones[i].position, originPos[i], 7f * Time.deltaTime);
            }
            yield return null;
        }
    }

    /* 발사 상태, 발사체 생성 후 플레이어를 향해 발사체 발사 */
    protected override IEnumerator Fire()
    {
        while (true)
        {
            if (state == ChaseState.IDLE && !fireBall.activeSelf)
            {
                // 발사체 충전
                state = ChaseState.CHARGE;
                audioSoruce.Play();
                yield return Fade(1, 0); // 발사체의 투명도를 조절함(Alpha : 1)

                // 발사체 발사
                state = ChaseState.SHOT;
                audioSoruce.Stop();
                Vector3 targetPos = new Vector3(playerTr.position.x, playerTr.position.y + 0.3f, playerTr.position.z);
                shotFire.GetTarget(targetPos);
                yield return new WaitForSeconds(0.5f);

                // 대기 상태
                state = ChaseState.IDLE;
                yield return new WaitForSeconds(reloadTime);
            }

            // 플레이어가 빠져나간 후
            if (!isShot)
            {
                fireBall.gameObject.SetActive(false);
                break;
            }
            yield return null;
        }
    }

    /* 발사체의 대한 투명도 조절 */
    protected override IEnumerator Fade(float fadeDir, float alpha)
    {
        color[0] = render.material.color;
        color[1] = particle[0].startColor;

        for (int i = 0; i < particle.Length; i++)
        {
            color[1].a = alpha;
            particle[i].startColor = color[1];
        }

        // 발사체를 현재 위치로 생성
        fireBall.gameObject.SetActive(true);
        fireBall.transform.position = transform.position;

        // 알파값 조절
        while (true)
        {
            alpha += fadeDir * 0.8f * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            color[0].a = alpha;
            color[1].a = alpha;

            render.material.color = color[0];

            if (alpha <= 0.6f)
            {
                for (int i = 0; i < particle.Length; i++)
                    particle[i].startColor = color[1];
            }

            if (alpha == 1 || !isShot)
                break;

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isShot)
        {
            isShot = true;

            StartCoroutine(Fire());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && isShot)
        {
            isShot = false;
        }
    }
}
