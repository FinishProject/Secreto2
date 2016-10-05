using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CutScene : MonoBehaviour {
    public float normalTransTime;

    [System.Serializable]
    public struct ImgInfo
    {
        public Image cutImg;
        public float transTime;
        public bool playFadeOut;
    }
    public ImgInfo[] imgInfo;
    public Image SkipImg;
    private bool onSkipButton;
    private int index;
    private int imgCnt;

	void Start () {
        index = 0;
        onSkipButton = false;
        imgCnt = imgInfo.Length;
        
        imgInfo[0].cutImg.enabled = true;
        for (int i = 1; i < imgCnt; i++)
            imgInfo[i].cutImg.enabled = false;

        StartCoroutine(SlideShow());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            onSkipButton = true;
    }

    IEnumerator fadeSkip(bool fadeIn)
    {
        Color tempAlpha = SkipImg.color;
        float pressKeyFadeSpeed = 0.8f;

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
    IEnumerator SlideShow()
    {
        StartCoroutine(fadeSkip(true));
        while (true)
        {

            if (imgInfo[index].transTime > 0)
            {
                yield return new WaitForSeconds(imgInfo[index].transTime);
            }
            else
            {
                yield return new WaitForSeconds(normalTransTime);
            }

            if(imgCnt <= index + 1)
            {
                StartCoroutine(fadeSkip(false));
                FadeInOut.instance.StartFadeInOut(0.5f, 5f, 0.5f);
                yield return new WaitForSeconds(1.5f);
            }
            else if (imgInfo[index].playFadeOut)
            {
                FadeInOut.instance.StartFadeInOut(0.5f, 0.3f, 0.5f);
                yield return new WaitForSeconds(0.5f);
            }

            if (onSkipButton || imgCnt <= index + 1)
            {
                Application.LoadLevel("LoadingScene");
                yield break;
            }

            imgInfo[index++].cutImg.enabled = false;
            imgInfo[index].cutImg.enabled = true;
        }
    }

    public void OnSkipButton()
    {
        onSkipButton = true;
    }
}
