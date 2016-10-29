using UnityEngine;
using System.Collections;

public class ShotRazorObj : MonoBehaviour {

    private float maxLength = 150f;
    private float fadeSpeed = 1f;
    public float upSpeed = 0.3f;
    public float downSpeed = 2f;
    public float chargeWaitTime = 2.5f;
    public float fullWaitTime = 5f;
    private float interValue = 0.06f;

    public GameObject startObj;
    public GameObject lazerObj;
    public Transform startPoint;

    public GameObject lazerMat;
    private Vector3 shotPoint;

    private float fadeDir = -1f;
    private float alpha = 0f;

    public bool isLand = true;

    private bool isActive = false;
    private bool isSound = false;

    public AudioClip clip;
    public AudioSource source;

    void Start()
    {
        StartCoroutine(SetLazer());
    }

	void Update () {
        if(alpha >= 0.9f)
            ShotRay();
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isActive)
        {
            source.volume = 1f;
            isActive = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            source.volume = 0f;
            isActive = false;
        }
    }
    

    IEnumerator SetLazer()
    {
        Renderer meshRender = lazerMat.GetComponent<Renderer>();
        Color setColor = meshRender.material.color;

        bool isUp = true;
        while (true)
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            setColor.a = alpha;
            meshRender.material.color = setColor;

            if (alpha == 0f || alpha == 1f)
            {
                fadeDir *= -1f;
                if (fadeDir == 1f)
                {
                    if (isActive)
                        source.Stop();

                    isUp = true;
                    fadeSpeed = upSpeed;
                }
                else
                    fadeSpeed = downSpeed;

                yield return new WaitForSeconds(fullWaitTime);
            }

            if (isUp && fadeSpeed <= 1f && setColor.a >= 0.2f)
            {
                yield return new WaitForSeconds(chargeWaitTime);
                if (!source.isPlaying)
                    source.PlayOneShot(clip);

                if (!isActive)
                    source.volume = 0f;
                //if (isActive)
                //    SoundMgr.instance.PlayAudio("Laser", false, 0.8f);
                yield return new WaitForSeconds(0.5f);
                isUp = false;
                fadeSpeed = 5f;
            }

            yield return null;
        }
    }

    void ShotRay()
    {
        RaycastHit hit;
        // 발사할 방향을 로컬 좌표에서 월드 좌표로 변환한다.
        Vector3 forward = transform.TransformDirection(-Vector3.up);
        if (Physics.Raycast(startPoint.position, forward, out hit, maxLength))
        {
            if (hit.collider.CompareTag("Player"))
            {
                StartCoroutine(PlayerCtrl.instance.PlayerDie());
            }
        }
    }
}
