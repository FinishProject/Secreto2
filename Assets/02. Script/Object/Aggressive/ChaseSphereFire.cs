using UnityEngine;
using System.Collections;

public class ChaseSphereFire : MonoBehaviour {

    enum ChaseState
    {
        IDLE, CHARGE, SHOT,
    }

    ChaseState state = ChaseState.IDLE;

    public GameObject fireBall;

    public Transform[] blocks;

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

    public ShotFireball shotFire;

    public Renderer render;
    public ParticleSystem[] particle;
    private Color[] color;

    void Start()
    {
        source = GetComponent<AudioSource>();
        playerTr = PlayerCtrl.instance.transform;

        color = new Color[2];

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

    IEnumerator ShotFireball()
    {
        while (true)
        {
            if (state == ChaseState.IDLE && !fireBall.activeSelf)
            {
                state = ChaseState.CHARGE;

                source.Play();
                yield return FadeFire(1, 0);

                state = ChaseState.SHOT;
                source.Stop();
                Vector3 targetPos = new Vector3(playerTr.position.x, playerTr.position.y + 0.3f, playerTr.position.z);
                shotFire.GetTarget(targetPos);

                yield return new WaitForSeconds(0.5f);

                state = ChaseState.IDLE;

                yield return new WaitForSeconds(reloadTime);
            }

            if (!isShot)
            {
                fireBall.gameObject.SetActive(false);
                break;
            }

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
                Vector3 oriPos = blocks[i].localPosition;
                blocks[i].localPosition = oriPos + Random.insideUnitSphere * shakeAmount;
            }

            yield return null;
        }
    }

    IEnumerator FadeFire(float fadeDir, float alpha)
    {
        
        color[0] = render.material.color;
        color[1] = particle[0].startColor;

        for(int i=0; i<particle.Length; i++)
        {
            color[1].a = alpha;
            particle[i].startColor = color[1];
        }

        fireBall.gameObject.SetActive(true);
        fireBall.transform.position = transform.position;

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

    // 레이저 발사 시 블록들 외각으로 벌어짐
    IEnumerator ShotMove()
    {
        while (state == ChaseState.SHOT)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].position = Vector3.Lerp(blocks[i].position, originPos[i], 7f * Time.deltaTime);
            }
            yield return null;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isShot)
        {
            isShot = true;

            StartCoroutine(ShotFireball());
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
