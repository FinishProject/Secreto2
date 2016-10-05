using UnityEngine;
using System.Collections;

/******************************** 사용/설명 **********************************
 
    오브젝트가 일정 위치를 벗어나면 원래 위치로 나타나게 해주는 스크립트
    (ex. 미는 오브젝트가 데드라인으로 추락했을 때)
    
    사용방법
    1. 오브젝트에 스크립트를 추가한다.
    2. 오브젝트의 이동 제한에 에 "LimitArea"를 추가 해준다.


 *****************************************************************************/
public class InitObjectPos : MonoBehaviour {

    Vector3 orignPos;   // 초기 위치
    Quaternion orignRot;   // 초기 위치
    MeshRenderer meshRender;
    Collider[] colliders;
    Color tempColor;
    new  Rigidbody rigidbody;
    bool playTrigger;
    public bool initPlayerDies;

    void Start () {
        orignPos = transform.position;
        orignRot = transform.rotation;
        meshRender = GetComponent<MeshRenderer>();   
        GetComponentsInChildren<BoxCollider>();
        tempColor = meshRender.material.color;
        rigidbody = GetComponent<Rigidbody>();
    }
	void Update()
    {
        if (initPlayerDies && !playTrigger && PlayerCtrl.dying)
            resetPos();
    }
	void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("LimitArea"))
        {
            resetPos();
        }
    }

    public void resetPos()
    {
        playTrigger = true;
        rigidbody.isKinematic = true;
        tempColor.a = -1;
        meshRender.material.color = tempColor;
        transform.position = orignPos;
        transform.rotation = orignRot;
        rigidbody.isKinematic = false;
        StartCoroutine(OpacityUp(true));
    }

    IEnumerator OpacityUp( bool isUp)
    {
        if (isUp)
        {
            while (true)
            {
                tempColor.a += 2f * Time.deltaTime; ;
                meshRender.material.color = tempColor;

                if (tempColor.a > 1f)
                {
                    break;
                }
                yield return null;
            }
        }
        else
        {
            while (true)
            {
                tempColor.a -= 3f * Time.deltaTime;
                meshRender.material.color = tempColor;

                if (tempColor.a < -1f)
                {
                    break;
                }

                yield return null;
            }

        }
        playTrigger = false;
    }
}
