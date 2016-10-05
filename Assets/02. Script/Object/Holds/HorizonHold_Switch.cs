using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    스위치를 이용한 이동 발판

    사용방법 :
    
    1. bar 모양 오브젝트에 스크립트 추가
    2. 일회용 발판인지 아닌지 체크를 해주자
    3. 스위치 오브젝트와 목표지점 오브젝트를 연결

    ※ 일회용 발판의 경우 스위치를 조작시 발판이 지정되어 있는 위치로 움직인 후,
    플레이어가 탑승하면 원위치로 움직이는데 다시 원 위치로 돌아갈 수 없다.

*************************************************************/

public class HorizonHold_Switch : MonoBehaviour
{
    public GameObject switchObject;
    public Transform targetTr;

    public bool isDisposable;   // 일회용 발판인가?
    public float speed = 3f;    // 발판 이동 속도

    private float pSpeed = 3f;  // 플레이어 이동 속도
    private Vector3 targetPos, originPos;
    private CharacterController controller;

    private bool isFocus;       // 플레이어가 왼쪽을 봐라보는지
    private bool isMove2Target;
    private bool isOnPlayer;
    void Start()
    {
        pSpeed = speed;
        isMove2Target = true;
        targetPos = targetTr.transform.position;
        originPos = transform.position;
    }

    void Update()
    {
        if (switchObject.GetComponent<SwitchObject>().IsSwitchOn)
        {
            Moving();
        }
    }

    void Moving()
    {

        // 타겟 위치로 이동
        if (isMove2Target)
        {
            if (Vector3.Distance(transform.position, targetPos) < 0.5f)
            {
                isMove2Target = false;
                switchObject.GetComponent<SwitchObject>().IsSwitchOn = false;
                return;
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
            if (isOnPlayer)
                controller.Move(Vector3.right * pSpeed * Time.deltaTime);
        }
        // 원 위치로 이동
        else if(!isMove2Target)
        {
            // 일회용 발판일때
            if (isDisposable)
            {
                if (isOnPlayer && Vector3.Distance(transform.position, originPos) > 0.5f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, originPos, Time.deltaTime * speed);
                    controller.Move(Vector3.right * pSpeed * Time.deltaTime);
                }
            }
            // 일회용 발판이 아닐 때
            else if (!isDisposable)
            { 
                if (Vector3.Distance(transform.position, originPos) < 0.5f)
                {
                    isMove2Target = true;
                    switchObject.GetComponent<SwitchObject>().IsSwitchOn = false;
                    return;
                }
                transform.position = Vector3.MoveTowards(transform.position, originPos, Time.deltaTime * speed);
                if(isOnPlayer)
                    controller.Move(-Vector3.right * pSpeed * Time.deltaTime);
            }
        }       

    }

    void OnTriggerStay(Collider coll)
    {
        // 플레이어가 발판 위에 있을 시 발판과 같이 이동
        if (coll.gameObject.tag == "Player")
        {
            controller = coll.GetComponent<CharacterController>();
            isOnPlayer = true;
            if(isDisposable)
            {
                switchObject.GetComponent<SwitchObject>().IsSwitchOn = true;
                isMove2Target = false;
            }
                

        }
    }
    
    void OnTriggerExit(Collider coll)
    {
        controller = null;
        isOnPlayer = false;
    }
}
