using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    밟았을때 아이템을 드랍하는 오브젝트

    사용방법 :
    
    그냥 오브젝트에 이 스크립트와 ItemDorp 스크립트를 추가하자
    
    플레이어가 밟으면 ItemDorp 스크립트에서 설정한 아이템 드롭

*************************************************************/
public class TrampleObject : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Player") && PlayerCtrl.instance.isJumping
            && PlayerCtrl.instance.transform.position.y > gameObject.transform.localScale.y)
        {
            gameObject.GetComponent<ItemDrop>().DropItem();
            gameObject.SetActive(false);
        }
    }
}
