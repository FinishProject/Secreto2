using UnityEngine;
using System.Collections;

public class NpcMgr : MonoBehaviour {

    protected float distance = 0f; // 거리 차
    protected bool isSpeak = true;

    protected Transform playerTr;
    protected Animator anim;
    protected Transform CameTr;

    void Awake()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        CameTr = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }
    // 자식 객체에서 초기화
    protected void Init()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        CurUpdate();
    }

    protected virtual void CurUpdate() { }

    // 플레이어와 거리 차이를 구함
    protected virtual float GetDistance(Vector3 npcPos)
    {
        return (playerTr.position - npcPos).sqrMagnitude;
    }

    protected virtual void StartSpeak(string npcName)
    {
        // 대화한 적이 없다면 || 대화한 적이 있고, 퀘스트 완료 시
        if (!ScriptMgr.instance.GetSpeakName(npcName) ||
            ScriptMgr.instance.GetSpeakName(npcName) && ScriptMgr.instance.isQuest)
        {
            ScriptMgr.instance.GetScript(npcName);
            PlayerCtrl.instance.SetStopMove();
        }
    }
}
