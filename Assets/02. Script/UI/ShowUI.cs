using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowUI : MonoBehaviour {

    public Image shiftImg;
    public float fadeSpeed = 2f;
    private int fadeDir = -1;
    private bool isActive = true;

    private float alpha = 0f;
    public static ShowUI instanace;

    void Awake()
    {
        instanace = this;
    }

    public void OnImage(bool isShowUI)
    {
        isActive = isShowUI;
        if (isShowUI)
        {
            shiftImg.gameObject.SetActive(true);
            StartCoroutine(OffImage());
        }
        else if (!isShowUI)
        {
            shiftImg.gameObject.SetActive(false);
        }
    }

    public void SetPosition(Transform boxTr, float yLength)
    {
        Vector3 setPosition = boxTr.position;
        setPosition.y += yLength;
        shiftImg.transform.position = setPosition;
    }

    IEnumerator OffImage()
    {
        while (isActive)
        {
            Color imgColor = shiftImg.color;
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            imgColor.a = alpha;

            shiftImg.color = imgColor;

            if (alpha >= 1 || alpha <= 0)
                fadeDir *= -1;

            yield return null;
        }
    }
}
