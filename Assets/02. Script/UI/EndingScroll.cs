using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndingScroll : MonoBehaviour {

    public Text text; 
    public Image blackImg;
    public GameObject logo;
    float speed;
    Vector3 t;

    public AudioSource source;
    public Image SkipImg;
    RectTransform textTr;
    float orign;
    float moveRange;
    
    float screenHeight;
    float textHeight;

    bool escTrigger;
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
        StartCoroutine(SkipButton());
    }

    IEnumerator SkipButton()
    {
        yield return new WaitForSeconds(3f);
        StartCoroutine(fadeSkip(true));
        while (true)
        {
            if (Input.GetKey(KeyCode.Escape) && !escTrigger)
            {
                escTrigger = true;
                StartCoroutine(fadeSkip(false));
                FadeInOut.instance.transform.localScale = new Vector3(100, 100, 100);
                FadeInOut.instance.StartFadeInOut(1.5f, 5f, 1.5f);
                yield return new WaitForSeconds(2f);
                Application.LoadLevel("LoadingScene");
                break;
            }
            yield return null;
        }
    }
    IEnumerator Scroll()
    {
        while (true)
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
                StartCoroutine(SetVloume());
                yield return new WaitForSeconds(3f);
                Application.LoadLevel("MainScene 1");
            }

            yield return null;
        }
    }
    IEnumerator SetVloume()
    {
        float volume = source.volume;
        while (true)
        {
            volume -= 0.3f * Time.deltaTime;
            volume = Mathf.Clamp01(volume);
            source.volume = volume;

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
        StartCoroutine(SetVloume());
        yield return new WaitForSeconds(3f);
        Application.LoadLevel("MainScene 1");
    }

    IEnumerator fadeSkip(bool fadeIn)
    {
        Color tempAlpha = SkipImg.color;
        float pressKeyFadeSpeed = 0.5f;

        if (!fadeIn)
        {
            tempAlpha.a = 1;
            pressKeyFadeSpeed *= -1;
        }
        else
        {
            tempAlpha.a = 0;
        }
        SkipImg.color = tempAlpha;

        while (true)
        {
            tempAlpha.a = pressKeyFadeSpeed * Time.deltaTime;
            SkipImg.color += tempAlpha;
            if (SkipImg.color.a >= 1.0f || SkipImg.color.a <= 0)
                break;

            yield return null;
        }
    }
}


    
