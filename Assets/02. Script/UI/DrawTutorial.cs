using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DrawTutorial : MonoBehaviour {

    public Image[] imgs;
    public Text txt;

    private bool isActive = false;
	
	void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isActive)
        {
            isActive = true;

            for (int i = 0; i < imgs.Length; i++)
                imgs[i].gameObject.SetActive(true);
            txt.gameObject.SetActive(true);

            StartCoroutine(Fade(1, 0));
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            StartCoroutine(Fade(-1, 1));
        }
    }

    IEnumerator Fade(float fadeDir, float alpha)
    {
        while (true)
        {
            Color color = txt.color;

            alpha += fadeDir * 1f * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            color.a = alpha;
            for(int i=0; i < imgs.Length; i++)
                imgs[i].GetComponent<Image>().color = color;
            txt.color = color;

            if (alpha == 0 || alpha == 1)
                break;

            yield return null;
        }

        if(fadeDir == -1f)
        {
            for (int i = 0; i < imgs.Length; i++)
                imgs[i].gameObject.SetActive(false);
            txt.gameObject.SetActive(false);
        }
    }
}
