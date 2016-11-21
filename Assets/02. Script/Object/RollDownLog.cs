using UnityEngine;
using System.Collections;


/********************************************** 사용 방법 ***************************************************

    경사에서 굴러가는 통나무 오브젝트
    일정 구역에 들어가면(RollDownLog_Trigger 스크립트) 통나무가 굴러간다
    플레이어와 충돌하면 죽는다.
    
    ※ 사용 방법
    1. 오브젝트 구조
    부모오브젝트
        └ 통나무           <- 스크립트 추가
        └ 구역(Collider)   <- RollDownLog_Trigger 스크립트 추가


************************************************************************************************************/

public class RollDownLog : MonoBehaviour {

    public float LogSpeed = 1500;   // 통나무 가속도
    public bool isStart = false;    // 작동이 시작되었는지
    float startShakeRange = 20;     // 카메라 흔들림 거리

    Vector3 orignPos;               // 원위치를 저장할 변수
    Quaternion orignRot;            // 원각도를 저장할 변수

    new Rigidbody rigidbody;        // 리지드 바디
    MeshRenderer LogMesh;           // 매쉬를 껐다 키기 위해 변수 만듬
    new CapsuleCollider collider;   // 컬리더를 껐다키기 위해 변수 만듬

    bool isMoving;                  // 움직이고 있는지
    bool isShaking;                 // 흔들리는지

    void Awake()
    {
        orignPos = transform.position;
        orignRot = transform.rotation;

        collider = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        LogMesh = GetComponentInChildren<MeshRenderer>();
        SetLog();
    }

    // 통나무 세팅
    void SetLog()
    {
        rigidbody.isKinematic = true;
        isMoving = false;
        LogMesh.enabled = false;
        collider.enabled = false;
        isShaking = false;

        transform.position = orignPos;
        transform.rotation = orignRot;

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    // 통나무 작동시 호출할 함수
    public void StartRollDownLog()
    {
       rigidbody.isKinematic = false;
       isMoving = true;
       LogMesh.enabled = true;
       collider.enabled = true;
        
    }

    public IEnumerator loopRollDown()
    {
        while(true)
        {
            StartRollDownLog();
            yield return null;

        }
    }

    void Update()
    {
        if(isMoving)
        {
            
            rigidbody.AddForce(Vector3.right * LogSpeed, ForceMode.Force);
            if (!isShaking && Vector3.Distance(transform.position, PlayerCtrl.instance.transform.position) < startShakeRange)
            {
                isShaking = true;
                CameraCtrl_1.instance.ShakeStart(transform, startShakeRange + 1);
            }
        }
    } 

    
    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("DeadLine"))
        { 
            SetLog();
        }

        if(isMoving && col.CompareTag("Player"))
        {
            RollDownLog_Trigger.isStarted = false;
            PlayerCtrl.instance.Die();

        }
    }
}
