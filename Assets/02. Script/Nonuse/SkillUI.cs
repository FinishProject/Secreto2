using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/****************************   정보   ****************************

    스킬창 UI
    스킬, 스킬 레벨, 스킬 관련 팝업 출력

    사용방법 :
    컴포넌트를 연결 시켜주자
    

******************************************************************/

public class SkillUI : MonoBehaviour {
    public Text skillInfo;
    public Text skillPoint;
    public GameObject selectPopup;
    public GameObject errorPopup;

    [System.Serializable]
    public struct CompleteSkill
    {
        public GameObject skill_1;
        public GameObject skill_2;
        public GameObject skill_3;
    }
    public CompleteSkill[] completeSkill = new CompleteSkill[(int)SkillFunction.Size];

    private int pressedID;

    void Start()
    {
        skillInfo.text = "여기는 스킬 정보창";
    }

    // 임시
    void Update()
    {
        PrintSkillPoint();
    }


    // 스킬포인트 유무
    private void PrintSkillPoint()
    {
        if (SkillMgr.instance.HasSkillPoint())
            skillPoint.text = "SkillPoint \n 있음";
        else
            skillPoint.text = "SkillPoint \n 없음";
    }


    // 스킬 버튼 선택시 
    public void PressSkillButton(int skillID)
    {
        // 처음 클릭이 아니면
        if (pressedID.Equals(skillID))
        {
            selectPopup.SetActive(true);
        }
        // 처음 클릭 했으면
        else
        {
            skillInfo.text = SkillMgr.instance.getSkillInfo(skillID);
        }

        pressedID = skillID;
    }


    // 에러창이 떴을 때 버튼 입력
    public void PressYesButton_Error()
    {
        errorPopup.SetActive(false);
    }


    // Yes, No 창이 떴을때 버튼 입력
    public void PressSelectButton(bool select)
    {
        if(select)
        {
            if(SkillMgr.instance.HasSkillPoint())
            {
                ActiveSkillUI(pressedID);

                SkillMgr.instance.SkillLevelUp(pressedID);
                skillInfo.text = SkillMgr.instance.getSkillInfo(pressedID);
                SkillMgr.instance.UseSkillPoint();
            }
            else
            {
                errorPopup.SetActive(true);
            }

            selectPopup.SetActive(false);
        }
        else
        {
            selectPopup.SetActive(false);
        }
    }


    // 스킬 UI
    private void ActiveSkillUI(int pressedID)
    {
        switch(SkillMgr.instance.getSkillLevel(pressedID))
        {
            case 1: completeSkill[pressedID].skill_1.GetComponent<Image>().color = new Color(0f, 255f, 0f); break;
            case 2: completeSkill[pressedID].skill_2.GetComponent<Image>().color = new Color(0f, 255f, 0f); break;
            case 3: completeSkill[pressedID].skill_3.GetComponent<Image>().color = new Color(0f, 255f, 0f); break;
        }
        
    }

}
