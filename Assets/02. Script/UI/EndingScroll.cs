using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndingScroll : MonoBehaviour {

    public Text text; 
    public Image blackImg;
    public GameObject logo;
    float speed;
    Vector3 t;


    RectTransform textTr;
    float orign;
    float moveRange;
    
    float screenHeight;
    float textHeight;
    // Use this for initialization
    void Start () {
        blackImg.enabled = false;
        logo.SetActive(false);

        textTr = text.GetComponent<RectTransform>();
        orign = textTr.anchoredPosition.y;
        textHeight = textTr.rect.height;
        moveRange = 0;
        screenHeight = Screen.height;
        speed = 170f;

        StartCoroutine(Scroll());
    }

    IEnumerator Scroll()
    {
        while(true)
        {
            if (moveRange > textHeight + screenHeight)
            {
//                FadeInOut.instance.StartFadeInOut(1, 1, 1);
                blackImg.enabled = true;
                logo.SetActive(true);
                StartCoroutine(FadeOut(3f));
                break;
            }
            else
            {
                t = textTr.anchoredPosition;
                moveRange += speed * Time.deltaTime;
                t.y += speed * Time.deltaTime;
                textTr.anchoredPosition = t;
            }

            yield return null;
        }
        
    }


    IEnumerator FadeLogo()
    {
        Image[] logoImg = logo.GetComponentsInChildren<Image>();
        Color color = logoImg[0].color;
        float alpha = 0f;

        while (true)
        {
            alpha += 0.7f * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            color.a = alpha;

            for (int i = 0; i < logoImg.Length; i++)
            {
                logoImg[i].color = color;
            }

            if (alpha >= 1f)
            {
                yield return new WaitForSeconds(1f);
                Application.LoadLevel("MainScene 1");
            }

            yield return null;
        }
    }

    IEnumerator FadeOut(float fadeOutTime)
    {
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= (1 / fadeOutTime) * Time.deltaTime;
            blackImg.color = new Color(0, 0, 0, alpha);
            yield return true;
        }
        Application.LoadLevel("MainScene 1");
    }
}


    
