using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LogoCtrl : MonoBehaviour {

    public float fadeSpeed = 0.8f;
    private float alpha;
    private float fadeDir = 1f;
    private int imgCount = 0;

    public Image[] logoImg;
    private Color colorValue;

    // Use this for initialization
    void Start () {
       
        for(int i=0; i<logoImg.Length; i++)
        {
            colorValue = logoImg[i].color;
            colorValue.a = 0;
            logoImg[i].color = colorValue;
        }

        StartCoroutine(FadeLogo());
	}

    IEnumerator FadeLogo()
    {
        while (true)
        {
            // 로고 전부 보여진 후 씬 이동
            if (imgCount >= logoImg.Length)
            {
                Application.LoadLevel(Application.loadedLevel + 1);
            }
            // 아무 키 입력시 로고 넘김
            else if (Input.anyKeyDown && imgCount < logoImg.Length)
            {
                colorValue.a = 0f;
                logoImg[imgCount].color = colorValue;
                alpha = 0f;
                imgCount++; 
            }
            // 로고 그리기
            else 
            {
                alpha += fadeDir * fadeSpeed * Time.deltaTime;
                alpha = Mathf.Clamp01(alpha);

                colorValue.a = alpha;
                logoImg[imgCount].color = colorValue;

                if (alpha == 1)
                {
                    yield return new WaitForSeconds(1f);
                    fadeDir = -1f;
                }
                else if (alpha == 0)
                {
                    imgCount++;
                    fadeDir = 1f;
                }
            }
            yield return null;
        }
    }
}
