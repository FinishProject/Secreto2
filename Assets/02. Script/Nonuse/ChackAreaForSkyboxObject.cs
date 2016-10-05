using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    지역에 따라 스카이 박스를 바꾸기 위해 필요한 오브젝트

    사용방법 :
    
    1. 자식 오브젝트들에 각각 센서 스크립트를 추가한다
    2. ★ 왼쪽 오브젝트의 인덱스는 1, 오른쪽은 2로 추가한다 ★

    ※ 사용 방법 2번을 꼭 확인하자
*************************************************************/

public class ChackAreaForSkyboxObject : MonoBehaviour, Sensorable_Player
{
    public int skyboxNumber = 0;
    private int saveIndex = 0;
    public bool ActiveSensor_Player(int index)
    {
        switch(saveIndex)
        {
            case 0:
                saveIndex = index;
                break;
            case 1:
                if (index == 2)
                {
                    // 왼쪽에서 오른쪽으로 갈때
                    SkyBoxMgr.instance.SwapSkyBoxAndLight(skyboxNumber, Dir.RIGHT);
                    saveIndex = 2;
                }
                else
                    saveIndex = 1;

                break;
            case 2:
                if (index == 1)
                {
                    // 오른쪽에서 왼쪽으로 갈때
                    SkyBoxMgr.instance.SwapSkyBoxAndLight(skyboxNumber, Dir.LEFT);
                    saveIndex = 1;
                }
                else
                    saveIndex = 2;

                break;
        }
        return false;
    }

}
