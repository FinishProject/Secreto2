using UnityEngine;
using System.Collections;

public class Altar_Puzzle : MonoBehaviour {

    public GameObject[] stepHolds;
    public float fadeSpeed = 1f;
    private float alpha = 1f;

    void OnCollisionEnter(Collision col)
    {
        // 오브젝트 위치 했을 시 발판 꺼짐.
        if (col.collider.CompareTag("OBJECT"))
        {
            StartCoroutine(FadeObject(-1));
        }
    }

    void OnCollisionExit(Collision col)
    {
        // 오브젝트 나갔을 시 발판 켜짐.
        if (col.collider.CompareTag("OBJECT"))
        {
            SetActiveObject(true);
            StartCoroutine(FadeObject(1));
        }
    }
    // 오브젝트 알파값 조절
    IEnumerator FadeObject(float fadeDir)
    {
        Color[] color = new Color[stepHolds.Length];

        for(int i=0; i<color.Length; i++)
        {
            color[i] = stepHolds[i].GetComponent<Renderer>().material.color;
        }

        while (true)
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
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
        if (fadeDir == -1f)
            SetActiveObject(false);
    }
    // 오브젝트 Active 설정
    void SetActiveObject(bool isActive)
    {
        for (int i = 0; i < stepHolds.Length; i++)
            stepHolds[i].SetActive(isActive);
    }
}
