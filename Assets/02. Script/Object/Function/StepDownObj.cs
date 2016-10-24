using UnityEngine;
using System.Collections;

public class StepDownObj : MonoBehaviour {

    public float maxLength = 10f;
    public float downSpeed = 3f;
    public float upSpeed = 2f;
    public float fadeSpeed = 1f;
    private bool isBack = false;

    private Vector3 originPos;

    private Shader standard;
    private Shader trans;

    void Start()
    {
        originPos = this.transform.position;
        standard = Shader.Find("Standard");
        trans = Shader.Find("Custom/balpan_trans");
        Debug.Log(trans);
    }

    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (GetComponent<Renderer>().material.shader != trans)
                GetComponent<Renderer>().material.shader = trans;

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
                GetComponent<Renderer>().material.shader = standard;

            transform.position = Vector3.MoveTowards(transform.position, originPos, upSpeed * Time.deltaTime);

            if (this.transform.position.y >= originPos.y - 0.05f)
                break;

            yield return null;
        }
    }
}
