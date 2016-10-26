using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndingScroll : MonoBehaviour {

    public Text text; 
    public Image blackImg;
    public GameObject logo;
    public float speed;
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
        speed = (Screen.height / 100) * speed;

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


    
