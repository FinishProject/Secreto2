﻿using UnityEngine;
using System.Collections;

public class StepDownObj : MonoBehaviour {

    public float downSpeed = 3f;
    public float upSpeed = 2f;
    public float fadeSpeed = 1f;
    private bool isBack = false;

    private Vector3 originPos;

    private Shader standard;
    public Shader transparent;

    void Start()
    {
        originPos = this.transform.position;

        standard = Shader.Find("Standard");

        if (transparent == null)
            transparent = Shader.Find("Custom/balpan_trans");
    }

    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (transparent != null && GetComponent<Renderer>().material.shader != transparent)
                GetComponent<Renderer>().material.shader = transparent;

            isBack = false;

            float curAlpha = Fade(-1);
            if (curAlpha <= 0f)
                GetComponent<Collider>().isTrigger = true;

            transform.position += Vector3.down * downSpeed * Time.deltaTime;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isBack = true;
            StartCoroutine(ReturnPosition());
        }
    }

    float Fade(float fadeDir)
    {
        Color color = this.GetComponent<Renderer>().material.color;
        float alpha = color.a;

        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        color.a = alpha;
        this.GetComponent<Renderer>().material.color = color;

        return alpha;
    }

    IEnumerator ReturnPosition()
    {
        while (isBack)
        {
            float curAlpha = Fade(1);
            if (curAlpha == 1f)
            {
                if(standard == null)
                    standard = Shader.Find("Standard");

                if (standard != null)
                    GetComponent<Renderer>().material.shader = standard;
            }
            else if (curAlpha >= 0.5f)
                GetComponent<Collider>().isTrigger = false;

            transform.position = Vector3.MoveTowards(transform.position, originPos, upSpeed * Time.deltaTime);

            if (this.transform.position.y >= originPos.y - 0.05f)
                break;

            yield return null;
        }
    }
}
