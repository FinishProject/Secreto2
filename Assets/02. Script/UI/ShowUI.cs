using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowUI : MonoBehaviour {

    public Image shiftImg;
    private float fadeSpeed = 2f;
    private bool isActive = true;

    public float alpha = 0f;
    public static ShowUI instanace;

    private Camera cam;
    private RectTransform rectTr;

    void Awake()
    {
        instanace = this;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rectTr = GetComponent<RectTransform>();
    }

    public void OnImage(float fadeDir)
    {
        if (fadeDir == 1)
            shiftImg.gameObject.SetActive(true);
        else
            shiftImg.gameObject.SetActive(false);

        //if (fadeDir == -1)
        //    StartCoroutine(OffImage());
        //else
        //{
        //    Color imgColor = shiftImg.material.color;
        //    alpha += fadeDir * fadeSpeed * Time.deltaTime;
        //    alpha = Mathf.Clamp01(alpha);

        //    imgColor.a = alpha;

        //    shiftImg.material.color = imgColor;
        //}
    }

    public void SetPosition(Transform boxTr, float yLength)
    {
        Vector3 setPosition = boxTr.position;
        setPosition.y += yLength;
        shiftImg.transform.position = setPosition;
    }

    IEnumerator OffImage()
    {
        while (alpha > 0f)
        {
            Color imgColor = shiftImg.material.color;
            alpha -= fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            imgColor.a = alpha;

            shiftImg.material.color = imgColor;

            yield return null;
        }
    }
}
