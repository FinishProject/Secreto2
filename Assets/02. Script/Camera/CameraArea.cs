using UnityEngine;
using System.Collections;


/********************************************** 사용 방법 ***************************************************

    맵에 카메라 위치정보를 가지고 있는 오브젝트 (스크립트)
    CameraCtrl_4를 사용하기 위해 필요하다
    
    ※ 사용 방법
    1. 플레이어가 지나갈 위치에 CameraArea 스크립트를 포함한 Collider를 추가해야한다. 
    2. 포커싱이 필요한 위치인 경우 forcusTr에 오브젝트 위치를 추가해 주면된다.

************************************************************************************************************/


public class CameraArea : MonoBehaviour {
    public Vector3 playerDistance;
    public bool hasFocus;
    public Transform focusTr;
    public float camSpeed = 5f;
}
