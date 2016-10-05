using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    플레이어가 밟을 시 일정 시간 후 사라지는 발판

    사용방법 :
        
*************************************************************/

public class TimeDown : MonoBehaviour {

    public float cnt = 3f;

    void OnTriggerStay(Collider coll)
    {
        cnt -= Time.deltaTime;
        if (cnt <= 0f) { this.gameObject.SetActive(false); }
    }

    void OnTriggerExit(Collider coll)
    {
        cnt = 0f;
    }

    void OnDisable()
    {
        GameObject ob = this.gameObject;
    }
}
