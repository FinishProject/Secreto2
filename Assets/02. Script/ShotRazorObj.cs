using UnityEngine;
using System.Collections;

public class ShotRazorObj : MonoBehaviour {

    public GameObject laserObj;
    public AudioClip cilp;
    private AudioSource audioSource;

    private float fadeSpeed = 1f;
    public float upSpeed = 2f;
    public float downSpeed = 0.5f;

    public float waitUpValue = 0.3f;
    public float waitingTime = 2f;
    public float durationTime = 5f;

    private bool isWait = true;
    private bool isActive = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;

        StartCoroutine(ShotLaser());
    }

    IEnumerator ShotLaser()
    {
        Renderer laserRender = laserObj.GetComponent<Renderer>();
        Color laserColor = laserRender.material.color;

        float alpha = 1f;
        float fadeDir = -1f;

        while (true)
        {
            // 알파값을 0~1 사이로 조절
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            // 레이저 알파값 변경
            laserColor.a = alpha;
            laserRender.material.color = laserColor;

            // 0 or 1이면 알파값 방향 및 속도 변경
            if(alpha == 0f || alpha == 1f)
            {
                fadeDir *= -1f;
                isWait = true;

                fadeSpeed = downSpeed;

                if (alpha == 0)
                    audioSource.Stop();

                yield return new WaitForSeconds(durationTime);
            }
            // 알파값 증가중 일정 값에서 대기
            else if(alpha >= waitUpValue && isWait)
            {
                isWait = false;
                fadeSpeed = upSpeed;
                yield return new WaitForSeconds(waitingTime);

                if(!audioSource.isPlaying)
                    audioSource.Play();
            }

            yield return null;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (isActive)
                StopCoroutine(AdjustSound(audioSource.volume, 1));

            StartCoroutine(AdjustSound(audioSource.volume, 1));
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (isActive)
                StopCoroutine(AdjustSound(audioSource.volume, 1));

            StartCoroutine(AdjustSound(audioSource.volume, -1));
        }
    }

    IEnumerator AdjustSound(float volume, float dir)
    {
        isActive = true;
        while (isActive)
        {
            volume += dir * 1f * Time.deltaTime;
            volume = Mathf.Clamp01(volume);

            audioSource.volume = volume;

            if (volume == 0f || volume == 1f)
                break;

            yield return null;
        }
        isActive = false;
    }

 //   private float fadeSpeed = 1f;
 //   public float upSpeed = 0.3f;
 //   public float downSpeed = 2f;
 //   public float chargeWaitTime = 2.5f;
 //   public float fullWaitTime = 5f;

    //   public GameObject startObj;
    //   public GameObject lazerObj;
    //   public Transform startPoint;

    //   public GameObject lazerMat;
    //   private Vector3 shotPoint;

    //   private float fadeDir = -1f;
    //   private float alpha = 0f;

    //   public bool isLand = true;

    //   private bool isActive = false;
    //   private bool isSound = false;

    //   public AudioClip clip;
    //   public AudioSource source;

    //   void Start()
    //   {
    //       StartCoroutine(SetLazer());
    //   }

    //void Update () {
    //       if(alpha >= 0.9f)
    //           ShotRay();
    //}

    //   void OnTriggerEnter(Collider col)
    //   {
    //       if (col.CompareTag("Player") && !isActive)
    //       {
    //           source.volume = 1f;
    //           isActive = true;
    //       }
    //   }

    //   void OnTriggerExit(Collider col)
    //   {
    //       if (col.CompareTag("Player"))
    //       {
    //           source.volume = 0f;
    //           isActive = false;
    //       }
    //   }


    //   IEnumerator SetLazer()
    //   {
    //       Renderer meshRender = lazerMat.GetComponent<Renderer>();
    //       Color setColor = meshRender.material.color;

    //       bool isUp = true;
    //       while (true)
    //       {
    //           alpha += fadeDir * fadeSpeed * Time.deltaTime;
    //           alpha = Mathf.Clamp01(alpha);

    //           setColor.a = alpha;
    //           meshRender.material.color = setColor;

    //           if (alpha == 0f || alpha == 1f)
    //           {
    //               fadeDir *= -1f;
    //               if (fadeDir == 1f)
    //               {
    //                   if (isActive)
    //                       source.Stop();

    //                   isUp = true;
    //                   fadeSpeed = upSpeed;
    //               }
    //               else
    //                   fadeSpeed = downSpeed;

    //               yield return new WaitForSeconds(fullWaitTime);
    //           }

    //           if (isUp && fadeSpeed <= 1f && setColor.a >= 0.2f)
    //           {
    //               yield return new WaitForSeconds(chargeWaitTime);
    //               if (!source.isPlaying)
    //                   source.PlayOneShot(clip);

    //               if (!isActive)
    //                   source.volume = 0f;

    //               yield return new WaitForSeconds(0.5f);
    //               isUp = false;
    //               fadeSpeed = 5f;
    //           }

    //           yield return null;
    //       }
    //   }

    //   void ShotRay()
    //   {
    //       RaycastHit hit;
    //       // 발사할 방향을 로컬 좌표에서 월드 좌표로 변환한다.
    //       Vector3 forward = transform.TransformDirection(-Vector3.up);
    //       if (Physics.Raycast(startPoint.position, forward, out hit, 100f))
    //       {
    //           if (hit.collider.CompareTag("Player"))
    //           {
    //               StartCoroutine(PlayerCtrl.instance.PlayerDie());
    //           }
    //       }
    //   }
}
