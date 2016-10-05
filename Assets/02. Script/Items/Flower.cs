using UnityEngine;
using System.Collections;

/****************************   정보   ****************************

    꽃 오브젝트
    플레이어가 먹으면 속성이 바뀐다

    사용방법 :

    매터리얼 연결...
    별거 없고 코드가 짧으니 읽어보자
    
    
******************************************************************/

public class Flower : MonoBehaviour {
    public Material red;
    public Material blue;
    public GameObject redEffect;
    public GameObject blueEffect;
    public bool isRed; 

	//void Start () {
 //       if(isRed)
 //       {
 //           gameObject.GetComponentInChildren<MeshRenderer>().material = red;
 //           redEffect = Instantiate(redEffect);
 //           redEffect.transform.position = gameObject.transform.position;
 //       }
 //       else
 //       {
 //           gameObject.GetComponentInChildren<MeshRenderer>().material = blue;
 //           blueEffect = Instantiate(blueEffect);
 //           blueEffect.transform.position = gameObject.transform.position;
 //       }
 //   }

    void OnTriggerEnter(Collider col)
    {
        if (QuestMgr.isQuest)
        {
            if (col.tag.Equals("Player"))
            {
                if (isRed)
                {
                    SkillCtrl.instance.ChangeAttribute(AttributeState.red);
                    redEffect.SetActive(false);
                }
                else
                {
                    SkillCtrl.instance.ChangeAttribute(AttributeState.blue);
                    blueEffect.SetActive(false);
                }
                gameObject.SetActive(false);

            }
        }
    }
}
