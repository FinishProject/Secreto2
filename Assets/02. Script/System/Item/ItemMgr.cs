using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    아이템 매니저

    사용방법 :

    1. 오브젝트에 추가
    2. 인스펙터 창에 오브젝트 풀을 실행할
       아이템 프리펩과 생성할 오브젝트 개수를 입력한다

*************************************************************/

// 아이템 정보
public class ItemStruct
{
    public int id;
    public string name;
    public int function;
    public int value;

    public void SetData(int id, string name, int function, int value)
    {
        this.id = id;
        this.name = name;
        this.function = function;
        this.value = value;
    }
}

// 아이템 종류(기능)
public enum ItemFunction
{
    HpRecovery = 0,     // 체력회복
    Mental,                // 스킬 경험치 습득
    LearnSkill,         // 스킬 습득
    Interaction,        // 상호작용 아이템 (ex. 나뭇잎)
    Collection,         // 수집템 (ex. 열쇠)
}


// 오브젝트 풀
public partial class ItemMgr : MonoBehaviour
{
    [System.NonSerialized]
    public ItemPool[] items = null;

    public GameObject HpRecoveryItemPrefab;
    public int HpRecoveryItemNumber;
    public GameObject MentalItemPrefab;
    public int MentalItemNumber;

    public void ItemPoolInit()
    {
        items = new ItemPool[2];
        items[(int)ItemFunction.HpRecovery] = gameObject.AddComponent<ItemPool>();
        items[(int)ItemFunction.HpRecovery].CreateItemPool(HpRecoveryItemPrefab, HpRecoveryItemNumber);
        items[(int)ItemFunction.Mental] = gameObject.AddComponent<ItemPool>();
        items[(int)ItemFunction.Mental].CreateItemPool(MentalItemPrefab, MentalItemNumber);
    }

    public ItemPool GetItem(ItemFunction itemType)
    {
        return items[(int)itemType];
    }
}


// 싱글톤
public partial class ItemMgr : MonoBehaviour
{
    public static ItemMgr instance;
    private static GameObject ItemPool;
    public static CSVParser csvParser;
    
    void Awake()
    {
        instance = this;                    // 싱글톤에 접근할 인스턴스
        csvParser = new CSVParser("Item");  // 파일 입출력 클래스 선언 및 생성자에 파일 이름 입력
        csvParser.Load();                   // 파일을 로드한다  ※ 없으면 절대 안된다
        ItemPoolInit();                     // 오브젝트 풀을 실행한다
    }

    // 아이템 정보를 리턴해준다.
    public CSVParser GetItem()
    {
        return csvParser;
    }
}

