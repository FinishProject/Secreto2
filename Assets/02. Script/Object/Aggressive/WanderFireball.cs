﻿using UnityEngine;
using System.Collections;

public class WanderFireball : MonoBehaviour {

    public float speed = 2f;
    public float maxLength = -150f;
    public float waitTime = 2f;

    private bool isMove = false;
    private Vector3 originLocation, finishLocation;

    private bool isUpMove = false;

    public GameObject fireEyes;

	// Use this for initialization
	void Start () {
        originLocation = transform.position;
        finishLocation = originLocation;
        finishLocation.x += maxLength;

        StartCoroutine(Movement());
	}

    IEnumerator Movement()
    {
        Vector3 targetLocation = finishLocation;
        float waitForTime = 0f;
        float moveSpeed = 0f;
        while (true)
        {

            float distance = (targetLocation - transform.position).sqrMagnitude;
            
            if(distance <= 5f && !isUpMove)
            {
                isUpMove = true;
                fireEyes.SetActive(true);
                StartCoroutine(VerticalMovement());

                yield return new WaitForSeconds(3f);

                fireEyes.SetActive(false);
                moveSpeed = 0f;
                isUpMove = false;

                if (targetLocation == originLocation)
                    targetLocation = finishLocation;
                else
                    targetLocation = originLocation;
            }

            moveSpeed = IncrementSpeed(moveSpeed, 2f, 0.2f);

            transform.position = Vector3.Lerp(transform.position, targetLocation, moveSpeed * Time.deltaTime);

            yield return null;
        }
    }

    IEnumerator VerticalMovement()
    {
        float moveSpeed = 0f;
        float startTime = 0f;
        
        while (isUpMove)
        {
            startTime += Time.deltaTime;
            moveSpeed = Mathf.Sin(1f * startTime) * 0.5f;

            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            yield return null;
        }
    }

    // 이동 속도 증가
    public float IncrementSpeed(float initSpeed, float maxSpeed, float accel)
    {
        if (initSpeed.Equals(maxSpeed))
            return initSpeed;
        else
        {
            initSpeed += accel * Time.deltaTime;
            // 기본 속도가 최고 속도를 넘을 시 음수가 되어 maxSpeed만 반환
            return (Mathf.Sign(maxSpeed - initSpeed).Equals(1)) ? initSpeed : maxSpeed;
        }
    }

    // 이동 속도 감소
    public float DecreaseSpeed(float initSpeed, float minSpeed, float accel)
    {
        if (initSpeed.Equals(minSpeed))
            return initSpeed;
        else
        {
            initSpeed -= accel * Time.deltaTime;
            // 기본 속도가 최고 속도를 넘을 시 음수가 되어 maxSpeed만 반환
            return (Mathf.Sign(minSpeed - initSpeed).Equals(1)) ? minSpeed : initSpeed;
        }
    }
}
