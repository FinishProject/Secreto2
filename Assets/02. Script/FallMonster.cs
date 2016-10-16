using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FallMonster : MonoBehaviour {

    public float fadeSpeed = 0.5f;
    public int endNum = 2;
    private bool isActive = false;

    private int fallCount = 0;


    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
            StartCoroutine(StartEndingScript());
        if (col.CompareTag("OBJECT"))
        {   
            PlayerCtrl.instance.SetStopMove();
            if (!isActive)
            {
                isActive = true;
                fallCount++;
                StartCoroutine(RotateObj(col));
            }
            
        }
    }
    // 보스 몬스터 석상 회전 시킴.
    IEnumerator RotateObj(Collider col)
    {
        if (fallCount >= endNum)
            StartCoroutine(StartEndingScript());

        PlayerCtrl.instance.isMove = false;
        PlayerCtrl.instance.animReset();

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

        PlayerCtrl.instance.isMove = true;
    }

    IEnumerator StartEndingScript()
    {
        yield return new WaitForSeconds(2f);
        ScriptMgr.instance.GetScript("ending");

        while (true)
        {
            if (!ScriptMgr.isSpeak)
            {
                FadeInOut.instance.StartFadeInOut(1f, 3f, 1f);
                yield return new WaitForSeconds(3f);
                Application.LoadLevel("MainScene 1");
            }

            yield return null;
        }
    }


}
