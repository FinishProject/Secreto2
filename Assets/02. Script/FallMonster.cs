using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FallMonster : MonoBehaviour {

    public GameObject endUi;
    public Image lineImg;
    public Text[] textUi;

    public float fadeSpeed = 0.5f;
    private bool isActive = false;


    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Statue"))
        {   
            PlayerCtrl.instance.SetStopMove();
            if (!isActive)
            {
                isActive = true;
                StartCoroutine(RotateObj(col.transform.parent));
            }
            
        }
    }
    // 보스 몬스터 석상 회전 시킴.
    IEnumerator RotateObj(Transform bossTr)
    {
        StartCoroutine(ShowEndUI());

        ShowUI.instanace.SetActiveImage(false);

        while (true)
        {
            bossTr.transform.Translate(Vector3.right * 0.8f * Time.deltaTime);
            bossTr.transform.RotateAround(Vector3.forward, -0.5f * Time.deltaTime);
            bossTr.transform.RotateAround(Vector3.up, -0.5f * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator ShowEndUI()
    {
        yield return new WaitForSeconds(2f);
        // 엔딩 UI 출력
        endUi.SetActive(true);
        lineImg.enabled = true;
        for (int i = 0; i < textUi.Length; i++)
            textUi[i].enabled = true;
        
        yield return new WaitForSeconds(4f);
        FadeInOut.instance.StartFadeInOut(1f, 3f, 1f);

        // 엔딩 UI 종료
        yield return new WaitForSeconds(1f);
        StartCoroutine(FadeText(-1, 1));

        // 메인 화면으로 씬 변경
        yield return new WaitForSeconds(3f);
        Application.LoadLevel("MainScene 1");
    }

    IEnumerator FadeText(float FadeDir, float alpha)
    {
        Color color;
        Color lineColor = lineImg.color;
        while (true)
        {
            alpha -= fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            for (int i = 0; i < textUi.Length; i++)
            {
                color = textUi[i].color;
                color.a = alpha;
                textUi[i].color = color;
            }
            lineColor.a = alpha;
            lineImg.color = lineColor;

            if (alpha == 0f || alpha == 1f)
                break;

            yield return null;
        }
    }
}
