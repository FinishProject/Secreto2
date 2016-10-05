using UnityEngine;
using System.Collections;

public class PlayerFunc : MonoBehaviour {

    public static PlayerFunc instance;
    PlayerCtrl playerCtrl;

    void Awake()
    {
        playerCtrl = GetComponent<PlayerCtrl>();
        instance = this;
    }

	public void SetImpactObject()
    {
        Collider[] hitColl = Physics.OverlapSphere(this.transform.position, 5f);
        for(int i = 0; i< hitColl.Length; i++) {
            if(hitColl[i].CompareTag("OBJECT")) {
                hitColl[i].SendMessage("GetImpact");
            }
        }
    }

    public void SetPowerDamage()
    {
        Collider[] hitColl = Physics.OverlapSphere(this.transform.position, 10f);
        for(int i = 0; i < hitColl.Length; i++)
        {
            hitColl[i].SendMessage("GetDamage");
        }
    }

    //펫 타기
    public void RidePet()
    {
        Debug.Log("Riding Pet");
    }

    //ScriptMgr에서 NPC이름을 찾아서 대화 생성
    public void ShowScript(string name)
    {
        if (name != null)
        {
            if (!ScriptMgr.instance.bgUi[1].activeSelf)
            {
                //대화 중이면 true, 캐릭터 정지
                PlayerCtrl.inputAxis = 0f;
                PlayerCtrl.instance.isMove = ScriptMgr.instance.bgUi[1].activeSelf;
                ScriptMgr.instance.GetScript(name);
                //NPCQuestMgr.instance.SetQuest();
            }
        }
    }
}
