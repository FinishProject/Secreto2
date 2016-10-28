using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.UI;

// 스크립트 정보들
public class Script
{
    public string id;
    public string context;
    public int speaker;
}

public class SpokeNpc
{
    public string NpcName;
    public bool isQuestClear;
}

public class ScriptMgr : MonoBehaviour {

    public Text[] txtUi; // 대사 텍스트 출력 UI
    public GameObject[] bgUi; // 대사 출력 배경 UI
    
    public static bool isSpeak = false;

    private List<Script> scriptData = new List<Script>(); //XML 데이터 저장
    private List<SpokeNpc> spokeNpc = new List<SpokeNpc>(); // 만난 NPC이름 저장

    public static ScriptMgr instance;

    void Awake()
    {
        instance = this;
        scriptData =  DataSaveLoad.LoadScript(); // 대사 XML 문서 불러오기
        //spokeNpc = DataSaveLoad.LoadNpcName(); // 이미 대화한 NPC 이름 불러오기
    }

    // NPC 이름에 해당하는 대사들과 퀘스트 정보를 가져옴
    public void GetScript(string curId)
    {
        List<Script> curScript = new List<Script>(); //현재 NPC의 대사를 저장할 리스트
        isSpeak = true;

        for (int i = 0; i < scriptData.Count; i++)
        {
            if (scriptData[i].id == curId)
            {
                // 대사 정보들을 저장
                curScript.Add(new Script
                {
                    id = scriptData[i].id,
                    context = scriptData[i].context,
                    speaker = scriptData[i].speaker,
                });
            }
        }
        if (curScript[0].id.Equals("meet") || curScript[0].id.Equals("ending"))
            StartCoroutine(ShowScript(curScript));

        else
            ActiveUI(curScript[0].speaker, curScript[0].context);
    }

    IEnumerator ShowScript(List<Script> ShowScript)
    {
        
        int arrIndex = 0;
        PlayerCtrl.instance.isMove = false;
        PlayerCtrl.instance.animReset();
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                if (arrIndex >= ShowScript.Count - 1)
                {
                    PlayerCtrl.instance.isMove = true;
                    isSpeak = false;
                    for (int i = 0; i < bgUi.Length; i++)
                        bgUi[i].SetActive(false);
                    break;
                }
                else
                    arrIndex++;
            }
            ActiveUI(ShowScript[arrIndex].speaker, ShowScript[arrIndex].context);
            yield return null;
        }
    }


    void ActiveUI(int spekerNum, string script)
    {
        // UI  초기화
        for (int i = 0; i < bgUi.Length; i++)
        {
            bgUi[i].SetActive(false);
        }
        // UI 출력
        switch (spekerNum)
        {
            case 0: // 플레이어
                bgUi[0].SetActive(true);
                txtUi[0].text = script;
                break;
            case 1: // 올라
                bgUi[1].SetActive(true);
                bgUi[4].SetActive(true);
                txtUi[1].text = script;
                break;
            case 3:
                bgUi[1].SetActive(true);
                bgUi[3].SetActive(true);
                txtUi[1].text = script;
                break;
        }
    }

    public void SetActiveUI(bool isActive, string context)
    {
        if (isActive)
        {
            bgUi[2].SetActive(isActive);
            txtUi[2].text = context;

            StartCoroutine(Fade(1, 0));
        }
        else
        {
            StartCoroutine(Fade(-1, 1));
        }
    }

    IEnumerator Fade(float fadeDir, float alpha)
    {
        while (true)
        {
            Color color = txtUi[2].color;

            alpha += fadeDir * 1f * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            color.a = alpha;
            bgUi[2].GetComponent<Image>().color = color;
            txtUi[2].color = color;
           
            if (alpha == 0 || alpha == 1)
                break;

            yield return null;
        }

        if(alpha == 0)
        {
            bgUi[2].SetActive(false);
        }
    }

    

    public bool GetSpeakName(string name)
    {
        
        if (spokeNpc == null)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < spokeNpc.Count; i++)
            {
                if (spokeNpc[i].NpcName == name)
                    return true;
            }
            return false;
        }
    }

    // 대화한 NPC 이름 저장
    void SaveNpcName()
    {
        DataSaveLoad.SaveNpcName(spokeNpc);
    }

    void OnEnable()
    {
        WayPoint.OnSave += SaveNpcName;
    }
}
