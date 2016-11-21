using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FallMonster : MonoBehaviour {

    public float fadeSpeed = 0.5f;
    public int endNum = 2;
    private bool isActive = false;

    private int fallCount = 0;
    public GameObject worldCanvas;

    public AudioSource source;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("OBJECT"))
        {   
            //PlayerCtrl.instance.SetStopMove();
            if (!isActive)
            {
                isActive = true;
                fallCount++;
                StartCoroutine(RotateObj(col));
                StartCoroutine(ShowScript());
            }
            
        }
    }
    IEnumerator ShowScript()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(StartEndingScript());
    }
    // 보스 몬스터 석상 회전 시킴.
    IEnumerator RotateObj(Collider col)
    {
        if (fallCount >= endNum)
            StartCoroutine(StartEndingScript());

        PlayerCtrl.instance.SetStopMove(false);

        PlayerCtrl.instance.ResetAnim();

        FallStoneArea.isActive = false;

        float waitTime = 0f;
        while (true)
        {
            col.transform.Translate(Vector3.right * 0.8f * Time.deltaTime);
            col.transform.RotateAround(Vector3.forward, -0.5f * Time.deltaTime);

            waitTime += Time.deltaTime;

            if (waitTime >= 2f)
                break;
                    
            yield return null;
        }

        PlayerCtrl.instance.SetStopMove(true);
    }

    // 엔딩 대사 씬 출력 및 대사 종료 후 씬 전환
    IEnumerator StartEndingScript()
    {
        worldCanvas.SetActive(false);
        yield return new WaitForSeconds(2f);
        ScriptMgr.instance.GetScript("ending");
        while (true)
        {
            if (!ScriptMgr.isSpeak)
            {
                FadeInOut.instance.StartFadeInOut(1f, 3f, 1f);
                StartCoroutine(SetVolume());
                yield return new WaitForSeconds(3f);
                Application.LoadLevel("EndCutScene");
            }

            yield return null;
        }
    }

    // 씬 넘어가면서 볼륨 낮춤
    IEnumerator SetVolume()
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


}
