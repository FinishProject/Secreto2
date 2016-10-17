using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CutScene : MonoBehaviour {
    public float normalTransTime;

    [System.Serializable]
    public struct ImgInfo
    {
        public GameObject cutImg;
        public float transTime;
        public bool playFadeOut;
    }
    public ImgInfo[] imgInfo;
    private bool onSkipButton;
    private int index;
    private int imgCnt;

	void Start () {
        index = 0;
        onSkipButton = false;
        imgCnt = imgInfo.Length;
        
        imgInfo[0].cutImg.SetActive( true);
        for (int i = 1; i < imgCnt; i++)
            imgInfo[i].cutImg.SetActive(false);

        StartCoroutine(SlideShow());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            onSkipButton = true;
    }

    IEnumerator SlideShow()
    {
        FadeInOut.instance.StartFadeInOut(0, 1, 2.5f);
        yield return new WaitForSeconds( 2f);
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

            imgInfo[index++].cutImg.SetActive(false);
            imgInfo[index].cutImg.SetActive(true);
        }
    }

    public void OnSkipButton()
    {
        onSkipButton = true;
    }
}
