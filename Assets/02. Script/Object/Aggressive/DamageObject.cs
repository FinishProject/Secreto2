using UnityEngine;
using System.Collections;

/****************************   정보   ****************************

    충돌시 데미지를 주는 오브젝트

    사용방법 :

    오브젝트에 추가 끝.
    코드 4줄, 끝.
******************************************************************/

public class DamageObject : MonoBehaviour {

    public float damage;

    void OnTriggerEnter(Collider col)
    {
        if(col.tag.Equals("Player"))
        {
            PlayerCtrl.instance.getDamage(damage);
        }
    }
}
