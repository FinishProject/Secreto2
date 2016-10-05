using UnityEngine;
using System.Collections;

public class CollectionItem : MonoBehaviour {

    public int itemID;
    private ItemStruct itemData;            // 아이템의 정보
    private Transform thisTr;               // 위치 확인

    void Start()
    {
        itemData = new ItemStruct();
        ItemMgr.instance.GetItem().ParseByID(itemData, itemID); // 엑셀 시트 ID값 참조
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Player"))
        {
            Inventory.instance.Insert(itemData);
            gameObject.SetActive(false);
        }
    }

}
