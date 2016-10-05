using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    투사체를 발사하는 오브젝트~!

    사용방법 :
    
    1. 오브젝트 메니저 오브젝트 풀에서
       발사 오브젝트를 먼서 생성을 해줘야 한다

    2. 오브젝트에 추가한 후 방향, 딜레이 타임을 설정하자
    
*************************************************************/

public class ShootingObject : MonoBehaviour {

    public float delayTime = 10f;
    public Dir shootDir;
    void Start()
    {
        StartCoroutine(CreateBullet());
    }

    IEnumerator CreateBullet()
    {
        while (true)
        {
            StartCoroutine( 
                ObjectMgr.instance.GetFourWayBullet().UseItem().
                GetComponent<BulletObject_FourWay>().Moveing(gameObject.transform, shootDir, 10f));

            yield return new WaitForSeconds(delayTime);
        }
    }
}
