using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*************************   정보   **************************

    스킬 매니저 클래스

    사용방법 :
    
    이 스크립트를 오브젝트에 추가하자

*************************************************************/

// 스킬 정보 틀
public class SkillStruct
{
    public int id;
    public string name;
    public int function;
    public int value;
    public int precedeID;
    public int level;

    public void SetData(int id, string name, int function, int value, int precedeID, int level)
    {
        this.id = id;
        this.name = name;
        this.function = function;
        this.value = value;
        this.precedeID = precedeID;
        this.level = level;
    }
}

// 스킬 기능
public enum SkillFunction
{
    None = 0,   // 기본
    Ga,
    Na,
    Da,
    Ra,
    Size = 5,   // 기능 개수
}

public class SkillMgr : MonoBehaviour {

    public static SkillMgr instance;    // 싱글톤 인스턴스
    public float needMentalPoint;          // 스킬포인트를 받기까지의 필요 경험치
    public GameObject SkillWindow;      // 스킬 UI 관리

    private SkillStruct[] curSkills;    // 현재 각 기능별 스킬 정보
    private float curMentalPoint;          // 현재 경험치
    private bool hasSkillPoint = false; // 스킬 포인트를 가지고 있는지

    private CSVParser skillList;        // 스킬 리스트
 
    void Awake()
    {
        instance = this;
        skillList = new CSVParser("SkillList");
        skillList.Load();
        SkillInit();
    }

    // 임시
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            hasSkillPoint = true;

        if (Input.GetKeyDown(KeyCode.K))
        {
            SkillWindow.SetActive(!SkillWindow.activeSelf);
            if(Time.timeScale != 0)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }

    // 스킬 레벨업 시켜줌
    public void SkillLevelUp(int skillFunction)
    {
        skillList.ParseByFunctionAndPrecedelID(curSkills[skillFunction], skillFunction, curSkills[skillFunction].id);
    }

    // 스킬 정보 받아옴
    public string getSkillInfo(int Function)
    {
        return skillList.ParseByIDReturnInfo(curSkills[Function].id);
    }

    // 스킬 포인트가 있는지 반환
    public bool HasSkillPoint()
    {
        return hasSkillPoint;
    }

    // 스킬 포인트 사용
    public void UseSkillPoint()
    {
        hasSkillPoint = false;
    }

    // 스킬 레벨 반환
    public int getSkillLevel(int Function)
    {
        return curSkills[Function].level;
    }

    // 스킬 목록 초기화
    void SkillInit()
    {
        curSkills = new SkillStruct[(int)SkillFunction.Size];

        for (int i = 0; i < (int)SkillFunction.Size; i++)
        {
            curSkills[i] = new SkillStruct();
            skillList.ParseByFunctionAndPrecedelID(curSkills[i], i, (int)SkillFunction.None);
        }
    }

    // 경험치를 얻을때
    public void GetMentaloint()
    {
        curMentalPoint++;
        if(curMentalPoint <= needMentalPoint)
        {
            hasSkillPoint = true;
            curMentalPoint = 0;
        }
    }


}
