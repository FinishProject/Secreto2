using UnityEngine;
using System.Collections;

public class TraceMonMgr : MonoBehaviour {

    public GameObject[] childObj = new GameObject[3];

    private int count = 0;
    public static TraceMonMgr instance;
	
	void Start () {
        instance = this;
	    for(int i=1; i<childObj.Length; i++)
        {
            childObj[i].SetActive(false);
        }
	}
    // 현재 오브젝트가 바닥 아래이면 다음 오브젝트 불러옴
    public void SetChildObj()
    {
        childObj[count].SetActive(false);
        count++;

        if (count >= childObj.Length)
            count = 0;

        childObj[count].SetActive(true);
    }
}
