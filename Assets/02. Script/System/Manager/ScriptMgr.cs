using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.UI;

// 스크립트 정보들
public class Script
{
    public string name;
    public string context;
    public int scriptType;
    public int speaker;
    public string questType;
    public string questTarget;
    public int completeNum;
}

public class SpokeNpc
{
    public string NpcName;
    public bool isQuestClear;
}

enum ScriptState
{
    basic, answer, refuse, end,
}

public class ScriptMgr : MonoBehaviour {

    public Text[] txtUi; // 대사 텍스트 출력 UI
    public GameObject[] bgUi; // 대사 출력 배경 UI
    
    public bool isQuest = false; // 퀘스트 완료 여부
    public static bool isSpeak = false;

    private List<Script> scriptData = new List<Script>(); //XML 데이터 저장
    private List<SpokeNpc> spokeNpc = new List<SpokeNpc>(); // 만난 NPC이름 저장
    private QuestInfo questInfo; // 퀘스트 정보 저장

    public static ScriptMgr instance;

    public bool isAnimQuest = false;

    void Awake()
    {
        instance = this;
        scriptData =  DataSaveLoad.LoadScript(); // 대사 XML 문서 불러오기
        spokeNpc = DataSaveLoad.LoadNpcName(); // 이미 대화한 NPC 이름 불러오기
    }

    // NPC 이름에 해당하는 대사들과 퀘스트 정보를 가져옴
    public void GetScript(string name)
    {
        List<Script> curScript = new List<Script>(); //현재 NPC의 대사를 저장
        isSpeak = true;

        // 이전에 대화를 하지 않는 NPC일 경우 대화를 위해 정보들을 가져옴
        if (!GetSpeakName(name))
        {
            for (int i = 0; i < scriptData.Count; i++)
            {
                if (scriptData[i].name == name && scriptData[i].scriptType == 0)
                {
                    // 대사 정보들을 저장
                    curScript.Add(new Script
                    {
                        name = name,
                        context = scriptData[i].context,
                        scriptType = scriptData[i].scriptType,
                        speaker = scriptData[i].speaker,
                    });
                    // 퀘스트 정보를 저장
                    questInfo = (new QuestInfo
                    {
                        questType = scriptData[0].questType,
                        targetName = scriptData[0].questTarget,
                        completNum = scriptData[0].completeNum,
                    });
                }
            }
            // NPC 이름과 퀘스트 달성 여부를 저장
            spokeNpc.Add(new SpokeNpc
            {
                NpcName = name,
                isQuestClear = false
            });

            QuestMgr.instance.GetQuestInfo(questInfo);
            StartCoroutine(ShowScript(curScript));
        }
        // 퀘스트 수락 후 완료 시
        else if (GetSpeakName(name) && isQuest)
        {
            for (int i = 0; i < scriptData.Count; i++)
            {
                if (scriptData[i].name == name && scriptData[i].scriptType == 1)
                {
                    // 대사 정보들을 저장
                    curScript.Add(new Script
                    {
                        name = name,
                        context = scriptData[i].context,
                        scriptType = scriptData[i].scriptType,
                        speaker = scriptData[i].speaker,
                    });
                }
            }
            // 퀘스트 완료후 퀘스트 달성 여부 수정 저장
            for(int i=0; i<spokeNpc.Count; i++)
            {
                if(spokeNpc[i].NpcName == name)
                {
                    spokeNpc[i].isQuestClear = true;
                }
            }
            isQuest = false;
            StartCoroutine(ShowScript(curScript));
        }
        else if (GetSpeakName(name) && !isQuest) 
        {
            Debug.Log("END SPEAK");
        }
    }

    IEnumerator ShowScript(List<Script> ShowScript)
    {
        int arrIndex = 0;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (arrIndex >= ShowScript.Count - 1)
                {
                    PlayerCtrl.instance.isMove = true;
                    isSpeak = false;
                    for (int i = 0; i < 2; i++)
                    {
                        bgUi[i].SetActive(false);
                    }
                    break;
                }
                else
                {
                    arrIndex++;
                }
            }
            ActiveUI(ShowScript[arrIndex].speaker, ShowScript[arrIndex].context);
            yield return null;
        }
    }

    void ActiveUI(int spekerNum, string script)
    {
        if (spekerNum == 0)
        {
            bgUi[1].SetActive(false);
            bgUi[0].SetActive(true);
            txtUi[0].text = script;
        }
        else if (spekerNum == 1)
        {
            bgUi[0].SetActive(false);
            bgUi[1].SetActive(true);
            txtUi[1].text = script;
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
