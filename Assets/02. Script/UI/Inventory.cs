using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*************************   정보   **************************

    인벤토리

    사용방법 :


*************************************************************/

public class Inventory : MonoBehaviour {
    List<ItemStruct> ownItems = new List<ItemStruct>();

    public static Inventory instance; // 싱글톤 인스턴스

    void Awake()
    {
        instance = this;
    }

    // 아이템 삽입 ( 상호작용 아이템과 수집템만 인벤토리에 삽입 할 수 있다)
    public void Insert(ItemStruct item)
    {
        if(item.function.Equals((int)ItemFunction.Interaction) ||
            item.function.Equals((int)ItemFunction.Collection))
            ownItems.Add(item);

    }

    // 아이템의 아이디값과 일치하는 데이터 삭제
    public void Delete(int itemId)
    {
        ownItems.RemoveAt( ownItems.FindIndex( x => x.id == itemId));
    }

    public ItemStruct Find(int itemId)
    {
        return ownItems.Find(x => x.id == itemId);
    }

    /*
    void OnGUI()
    {
        string tempText;
        tempText = "소지 아이템 : ";
        for(int i = 0; i < ownItems.Count; i++)
        {
            tempText += ownItems[0].name;
        }
        GUI.TextField(new Rect(0, 30.0f, 300.0f, 30.0f), tempText);

    }
    */

}
