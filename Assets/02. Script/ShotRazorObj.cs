using UnityEngine;
using System.Collections;

public class ShotRazorObj : MonoBehaviour {

    public GameObject laserObj;
    private AudioSource audioSource;
    public Collider childColider;

    private float fadeSpeed = 1f;
    public float upSpeed = 2f;
    public float downSpeed = 0.5f;

    public float waitUpValue = 0.3f;
    public float waitingTime = 2f;
    public float durationTime = 5f;

    private bool isWait = true;
    private bool isActive = false;

    private bool isAdjust = false;

    private bool isPlayer = false;

    float alpha = 1f;

    Vector3 originScale;
    float scaleX = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;

        originScale = laserObj.transform.localScale;

        StartCoroutine(ShotLaser());
    }

    void ScaleChange(float dir)
    {
        Vector3 scale = laserObj.transform.localScale;
        scaleX += 0.2f * Time.deltaTime;
        scaleX = Mathf.Clamp01(scaleX);
        if (scaleX <= 0.7f)
        {
            scale.x = scaleX;
            laserObj.transform.localScale = scale;
        }
    }

    IEnumerator ShotLaser()
    {
        Color laserColor = laserObj.GetComponent<Renderer>().material.color;
        
        float fadeDir = -1f;

        while (true)
        {
            // 알파값을 0~1 사이로 조절
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            // 레이저 알파값 변경
            laserColor.a = alpha;
            laserObj.GetComponent<Renderer>().material.color = laserColor;

            // 사운드 볼륨 점차 감소
            if (fadeDir <= -1f && isPlayer)
                audioSource.volume = alpha;

            

            if(fadeDir == 1 && isWait)
                ScaleChange(fadeDir);

            // 0 or 1이면 알파값 방향 및 속도 변경
            if (alpha == 0f || alpha == 1f)
            {
                fadeDir *= -1f;
                isWait = true;

                fadeSpeed = downSpeed;

                if (alpha == 0) // 레이저가 사라지면 사운드 중지
                {
                    audioSource.Stop();
                    Vector3 scale = laserObj.transform.localScale;
                    scale.x *= 0.2f;
                    laserObj.transform.localScale = scale;
                    scaleX = laserObj.transform.localScale.x;

                    childColider.enabled = false;
                }

                yield return new WaitForSeconds(durationTime);
            }
            // 알파값 증가 중 일정값에서 대기
            else if(alpha >= waitUpValue && isWait)
            {
                isWait = false;
                fadeSpeed = upSpeed;

                yield return new WaitForSeconds(waitingTime);
                laserObj.transform.localScale = originScale;
                // 사운드 재생
                if (!audioSource.isPlaying)
                {
                    if (!isAdjust && isPlayer)
                        audioSource.volume = 1f;

                    audioSource.Play();
                }

                childColider.enabled = true;
            }
            yield return null;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isPlayer = true;
            if (isAdjust)
                StopCoroutine(AdjustSound(audioSource.volume, 1));

            StartCoroutine(AdjustSound(audioSource.volume, 1));
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isPlayer = false;
            if (isAdjust)
                StopCoroutine(AdjustSound(audioSource.volume, 1));

            StartCoroutine(AdjustSound(audioSource.volume, -1));
        }
    }

    IEnumerator AdjustSound(float volume, float dir)
    {
        isAdjust = true;
        while (isAdjust)
        {
            volume += dir * 1f * Time.deltaTime;
            volume = Mathf.Clamp01(volume);

            audioSource.volume = volume;

            if (volume == 0f || volume == 1f)
                break;

            yield return null;
        }
        isAdjust = false;
    }
}
