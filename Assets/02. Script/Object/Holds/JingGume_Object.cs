using UnityEngine;
using System.Collections;

public class JingGume_Object : MonoBehaviour
{

    public bool isMatchObject;
    public string matchName;
    public GameObject[] gObjects = null;
    public int[] requisiteCnts = null;     // 필요 조건 (개수)
    private int Cnt = 0;

    JingGume_Object parent;

    void Start()
    {
        //발판 끄기
        for (int i = 1; i < gObjects.Length; i++)
        {
            gObjects[i].SetActive(false);
        }
        if (transform.parent)
            parent = transform.parent.GetComponentInParent<JingGume_Object>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Cnt++;

            //parent.OnHolds(PlayerCtrl.instance.getCarryItemName());
            //parent.OnHolds(PlayerCtrl.instance.getCarryItemName());
        }
    }

    void OnHolds(string name)
    {
        for (int i = 1; i < gObjects.Length; i++)
        {
            if(isMatchObject)
            {
                if (requisiteCnts[i] <= gObjects[i - 1].GetComponent<JingGume_Object>().Cnt && string.Equals(matchName, name))
                {
                    gObjects[i].SetActive(true);
                }
            }
            else
            {
                if (requisiteCnts[i] <= gObjects[i - 1].GetComponent<JingGume_Object>().Cnt)
                {
                    gObjects[i].SetActive(true);
                }
            }
            
        }
    }

}
