﻿using UnityEngine;
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

    void Start()
    {
        shotPoint = startPoint.position;
        shotPoint.x += 0.5f;
        startObj.transform.position = shotPoint;

        //Vector3 scale = lazerObj.transform.localScale;
        // 보간 값을 곱하여 레이저의 길이를 조절한다.
        //scale.x = maxLength * interValue;
        //lazerObj.transform.localScale = scale;
        //startObj.transform.position = startPoint.position;

        StartCoroutine(SetLazer());

    }

	void Update () {
        if(alpha >= 0.9f)
            ShotRay();
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
                yield return new WaitForSeconds(fullWaitTime);

                if (fadeDir == 1f)
                {
                    isUp = true;
                    fadeSpeed = upSpeed;
                }
                else
                    fadeSpeed = downSpeed;
            }

            if (isUp && fadeSpeed <= 1f && setColor.a >= 0.2f)
            {
                yield return new WaitForSeconds(chargeWaitTime);
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
            if (hit.collider.CompareTag("Player") && alpha == 1f)
            {
                PlayerCtrl.instance.PlayerDie();
            }
            else if (hit.collider.CompareTag("Land") && isLand)
            {
                //레이저 크기를 레이캐스트 충돌 위치와의 거리를 구하여 크기를 변경
                //Vector3 scale = lazerObj.transform.localScale;
                //보간 값을 곱하여 레이저의 길이를 조절한다.
                //scale.x = hit.distance * interValue;
                //lazerObj.transform.localScale = scale;
                //startObj.transform.position = startPoint.position;
            }

        }
    }
}
