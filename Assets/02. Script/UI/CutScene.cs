using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CutScene : MonoBehaviour {
    public float normalTransTime;

    [System.Serializable]
    public struct ImgInfo
    {
        public GameObject cutImg;
//        public float transTime;
        public bool playFadeOut;
    }
    public ImgInfo[] imgInfo;
    public Image BlackImg;
    private bool onSkipButton;
    private int index;
    private int imgCnt;
    float alpha;

    void Start () {
        index = 0;
        onSkipButton = false;
        imgCnt = imgInfo.Length;
        
        imgInfo[0].cutImg.SetActive( true);
        for (int i = 1; i < imgCnt; i++)
            imgInfo[i].cutImg.SetActive(false);

        StartCoroutine(SlideShow());
    }

    IEnumerator SlideShow()
    {
        FadeInOut.instance.StartFadeInOut(0, normalTransTime * 0.2f, normalTransTime * 0.8f);
        StartFadeInOut(0, normalTransTime * 0.2f, normalTransTime * 0.8f);
        yield return new WaitForSeconds( 2f);
        while (true)
        {

            //if (imgInfo[index].transTime > 0)
            //{
            //    yield return new WaitForSeconds(imgInfo[index].transTime);
            //}
            //else
            //{
                yield return new WaitForSeconds(normalTransTime);
            //}

            if(imgCnt <= index + 1)
            {
                StartFadeInOut(normalTransTime * 0.2f, 100f, normalTransTime * 0.2f);
                yield return new WaitForSeconds(1.5f);
            }
            else if (imgInfo[index].playFadeOut)
            {
                StartFadeInOut(normalTransTime * 0.4f, normalTransTime * 0.2f, normalTransTime * 0.4f);
                yield return new WaitForSeconds(normalTransTime * 0.6f);
            }

            if (imgCnt <= index + 1)
            {
                //Application.LoadLevel("MainScene 1");
                yield break;
            }

            imgInfo[index++].cutImg.SetActive(false);
            imgInfo[index].cutImg.SetActive(true);
        }
    }

    public void StartFadeInOut(float fadeInTime, float waitTime, float fadeOutTime)
    {
            StartCoroutine(Load(fadeInTime, waitTime, fadeOutTime));
    }

    IEnumerator Load(float fadeInTime, float waitTime, float fadeOutTime)
    {

        alpha = 0;

        while (alpha < 1 && fadeInTime > 0)
        {
            alpha += (1 / fadeInTime) * Time.deltaTime;
            BlackImg.color = new Color(0, 0, 0, alpha);
            yield return true;
        }

        if (waitTime > 0)
            yield return new WaitForSeconds(waitTime);

        alpha = 1;
        while (alpha > 0)
        {

            alpha -= (1 / fadeOutTime) * Time.deltaTime;
            BlackImg.color = new Color(0, 0, 0, alpha);
            yield return true;
        }
    }

    public void OnSkipButton()
    {
        onSkipButton = true;
    }
}
