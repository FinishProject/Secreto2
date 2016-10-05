using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    스위치

    사용방법 :
    
    1. 사용할 오브젝트 스크립트에 SwitchObject 을 public 선언
    2. inspector 창에서 연결해서 사용하자!
    
*************************************************************/

public class SwitchObject : MonoBehaviour {
    private bool isSwitchOn;
    public bool IsSwitchOn
    {
        get { return isSwitchOn; }
        set
        {
            if (IsCanUseSwitch)
                isSwitchOn = value;
        }
        
    }
    public bool IsCanUseSwitch { set; get; }
}
