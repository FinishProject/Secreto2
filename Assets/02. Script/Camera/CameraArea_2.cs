using UnityEngine;
using System.Collections;

/********************************************** 사용 방법 ***************************************************

    CameraCtrl_5 스크립트로 작동하는 카메라를 위한 스크립트
    플레이어가 지나갈 위치에 CameraArea_2 스크립트를 포함한 Collider를 추가해야한다. 
    
    ※ 사용 방법
    1. 

************************************************************************************************************/

public class CameraArea_2 : MonoBehaviour {

    public float val;
    public bool moving;
    public Transform moving_bace;

    private Vector3 orign;
    
    void Start()
    {
        orign = transform.position;
        if (moving)
            StartCoroutine(UpdatePos());
    }

    IEnumerator UpdatePos()
    {
        while(true)
        {
            orign.y = moving_bace.transform.position.y+1;
            transform.position = orign;
            val = moving_bace.position.y + 1;
            yield return null;
        }   
    }

}
