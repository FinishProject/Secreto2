using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    아이템 드랍을 해주는 클래스

    사용방법 :
    
    1. 아이템을 드랍할 오브젝트에 이 클래스를 추가
    2. 아이템을 드랍할 상황에 DropItem() 함수를 호출

     ★ 별도의 제어 변수는 주석 참조

*************************************************************/

public class ItemDrop : MonoBehaviour {

    public bool isRandomNumberDrap;     // 갯수 랜덤

    [System.Serializable]               // 구조체를 인스펙터 창에 출력해줌
    public struct DarpItemInfo
    {
        public bool HpRecover;          // 회복 아이템 드랍 할 것인가?
        public int  HpRecoverNumber;    // 드랍할 회복 아이템 개수
        public bool Mental;                // 경험치 
        public int  MentalNumber;          // 경험치 개수
    }
    public DarpItemInfo dropItemList;   // 입력받은 구조체
    public float dropRange;             // 드랍 범위

    private Transform thisTr;           // 이 오브젝트의 위치
    
    void Start () {
        thisTr = GetComponent<Transform>();
        
        if(isRandomNumberDrap)
        {
            dropItemList.HpRecoverNumber = Random.Range(0, dropItemList.HpRecoverNumber);
            dropItemList.MentalNumber = Random.Range(0, dropItemList.MentalNumber);
        }
    }
	
    // 아이템 드랍
    public void DropItem()
    {
        GameObject tempItem;
        
        for (int i = 0; dropItemList.HpRecover && i < dropItemList.HpRecoverNumber; i++)
        {
            tempItem = ItemMgr.instance.GetItem(ItemFunction.HpRecovery).UseItem();

            Vector3 randPos = new Vector3(
                Random.Range(thisTr.position.x - dropRange, thisTr.position.x + dropRange), 
                Random.Range(thisTr.position.y + dropRange, thisTr.position.y + (dropRange * 2)),
                -2);
            tempItem.GetComponent<HpRecoveryItem>().Dropped(thisTr.position, randPos, true);
        }

        for (int i = 0; dropItemList.Mental && i < dropItemList.MentalNumber; i++)
        {
            tempItem = ItemMgr.instance.GetItem(ItemFunction.Mental).UseItem();
            Vector3 randPos = new Vector3(
                Random.Range(thisTr.position.x - dropRange, thisTr.position.x + dropRange),
                Random.Range(thisTr.position.y + dropRange, thisTr.position.y + (dropRange * 2)),
                -2);
            tempItem.GetComponent<MentalItem>().Dropped(thisTr.position, randPos, true);
        }

    }

}
