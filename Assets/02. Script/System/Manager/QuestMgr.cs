using UnityEngine;
using System.Collections;

public class QuestInfo
{
    public string questType;
    public string targetName = null;
    public int completNum = 0;
    //public GameObject rewardItem;
}

public enum QuestType { HUNT, COLLECT, }


public class QuestMgr : MonoBehaviour {

    public static QuestInfo questInfo;
    public int curCompletNum = 0;
    private QuestType questType;
    public static bool isQuest = false;
    public static QuestMgr instance;

    void Awake()
    {
        instance = this;
    }

    // 퀘스트 정보를 받아옴
    public void GetQuestInfo(QuestInfo info)
    {
        Debug.Log("Get Quest");
        questInfo = info;
        isQuest = true;
        QuestTypes();
    }

    // 퀘스트 종류
    void QuestTypes()
    {
        switch (questInfo.questType)
        {
            case "HUNT":
                Debug.Log("Hunt");
                StartCoroutine(HuntQuest());
                break;
            case "COLLECT":
                Debug.Log("Collect");
                StartCoroutine(CollectQuest());
                break;
        }
    }

    IEnumerator HuntQuest()
    {
        while (true)
        {
            if(questInfo.completNum <= curCompletNum)
            {
                Debug.Log("Complete");
                isQuest = false;
                ScriptMgr.instance.isQuest = true;
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    // 수집 퀘스트
    IEnumerator CollectQuest()
    {
        while (true)
        {
            // 목표 아이템 개수 이상 수집 시 퀘스트 완료
            if(questInfo.completNum <= curCompletNum)
            {
                Debug.Log("Complete");
                isQuest = false;
                ScriptMgr.instance.isQuest = true;
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
