using UnityEngine;
using System.Collections;

public class ShotRazorObj : MonoBehaviour {

    public GameObject laserObj;
    private AudioSource audioSource;
    public Transform startPoint;
    public Collider childColider;

    private float fadeSpeed = 1f;
    public float upSpeed = 2f;
    public float downSpeed = 0.5f;

    public float waitUpValue = 0.3f;
    public float waitingTime = 2f;
    public float durationTime = 5f;

    private bool isWait = true;
    private bool isActive = false;

    float alpha = 1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;

        StartCoroutine(ShotLaser());
    }

    void Update()
    {
        if (alpha == 1f)
            ShotRay();
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

            childColider.enabled = false;

            // 0 or 1이면 알파값 방향 및 속도 변경
            if (alpha == 0f || alpha == 1f)
            {
                fadeDir *= -1f;
                isWait = true;

                fadeSpeed = downSpeed;

                if (alpha == 0)
                    audioSource.Stop();
                else if (alpha == 1f)
                    childColider.enabled = true;

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

    void ShotRay()
    {
        RaycastHit hit;
        // 발사할 방향을 로컬 좌표에서 월드 좌표로 변환한다.
        Vector3 forward = startPoint.TransformDirection(Vector3.right);
        Debug.DrawRay(startPoint.position, forward, Color.blue, 1f);
        if (Physics.Raycast(startPoint.position, forward, out hit, 100f))
        {
            if (hit.collider.CompareTag("Player"))
            {
                StartCoroutine(PlayerCtrl.instance.PlayerDie());
            }
        }
    }
    void OnTriggerEnter(Collider col)
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
}
