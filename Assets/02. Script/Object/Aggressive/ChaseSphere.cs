using UnityEngine;
using System.Collections;

public class ChaseSphere : MonoBehaviour {

    public Transform laser;
    public Renderer laserRender;
    public Transform[] blocks;
    public Transform finishPoint;

    private Transform playerTr;

    private float laserSpeed = 100f;
    public float[] blockSpeed;
    private float moveDir = 1f;
    private bool isShot = false;

    private float rndSpeed = 0f;
    private bool isMove = false;

    void Start()
    {
        playerTr = PlayerCtrl.instance.transform;

        StartCoroutine(MovementBlock());
    }

    //void Update()
    //{
    //    MovementBlock();

    //    if(isShot)
    //        ShotLaser();
    //}

    IEnumerator ShotLaser()
    {
        Vector3 targetPos = playerTr.position;
        isMove = false;

        while (true)
        {
            // 타겟 추적
            Quaternion targetRot = Quaternion.LookRotation(targetPos - laser.position, Vector3.forward);
            targetRot.x = 0f;
            targetRot.y = 0f;

            laser.transform.rotation = Quaternion.Slerp(laser.rotation, targetRot, laserSpeed * Time.deltaTime);

            // 레이저 생성 및 타겟 방향을 종료 방향으로 바꿈
            if (!isMove)
            {
                yield return FadeLaser(1, 0);
                isMove = true;
                targetPos = finishPoint.position;
                laserSpeed = 0.5f;
            }

            // 레이저와 타겟에 각도를 구함
            float angle = Vector3.Angle(laser.transform.right, (laser.position - targetPos));

            // 일정 각도 안에 들어왔을 시 레이저 삭제
            if(angle <= 97f && isMove)
            {
                yield return FadeLaser(-1, 1);

                laser.gameObject.SetActive(false);
                isMove = false;
                laserSpeed = 100f;
                targetPos = playerTr.position;

                // 플레이어가 범위 안에 나갔을 시 레이저 종료
                if (!isShot)
                    break;

                yield return new WaitForSeconds(2f);
                laser.gameObject.SetActive(true);
            }

            yield return null;
        }
    }

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


    IEnumerator MovementBlock()
    {

        while (true)
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

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isShot = true;
            laser.gameObject.SetActive(true);
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
