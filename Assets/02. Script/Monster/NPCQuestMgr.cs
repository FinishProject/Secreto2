using UnityEngine;
using System.Collections;

public class NPCQuestMgr : MonoBehaviour {

    QuestInfo questInfo;

    public QuestType questType;

    public string targetName;
    public int completNum;

    public static NPCQuestMgr instance;

    void Awake()
    {
        instance = this;
    }
    // 퀘스트 정보 전달
    public void SetQuest()
    {
        questInfo.targetName = this.targetName;
        questInfo.completNum = this.completNum;
        //questInfo.questType = this.questType;

        QuestMgr.instance.GetQuestInfo(questInfo);
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Player")
        {
            Debug.Log("꽝");
        }
    }
}
