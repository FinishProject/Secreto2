using UnityEngine;
using System.Collections;

public class Altar_Puzzle : MonoBehaviour {

    public GameObject[] stepHolds;

    private float alpha = 1f;

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("OBJECT"))
        {
            stepHolds[0].SetActive(false);
            //for (int i = 0; i < stepHolds.Length; i++)
            //{
            //    stepHolds[i].SetActive(true);
            //}
            //StartCoroutine(FadeColor(1));
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.collider.CompareTag("OBJECT"))
        {
            
            StartCoroutine(FadeColor(-1));
        }
    }

    IEnumerator FadeColor(float fadeDir)
    {
        Color[] color = new Color[stepHolds.Length];

        for(int i=0; i<color.Length; i++)
        {
            color[i] = stepHolds[i].GetComponent<Renderer>().material.color;
        }

        while (true)
        {
            alpha += fadeDir * 1f * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            for (int i = 0; i < color.Length; i++)
            {
                color[i].a = alpha;
                stepHolds[i].GetComponent<Renderer>().material.color = color[i];
            }

            if (alpha <= 0f || alpha >= 1f)
                break;

            yield return null;
        }
        if (fadeDir.Equals(-1))
        {
            for (int i = 0; i < stepHolds.Length; i++)
                stepHolds[i].SetActive(false);
        }
    }
}
