using UnityEngine;
using System.Collections;


/********************************************** 사용 방법 ***************************************************

    플레이어나 오브젝트가 collider에 충돌하면 투명한 엘리베이터 발판들이 올라오며 점점 불투명으로 바뀌는 기능
    
    ※ 사용 방법
    1. elevatorParent 의 구조
    elevatorParent          <- 빈 오브젝트
        └ elevator_01      <- 이름은 크게 상관 없지만 배치 순서에 따른 하이라키 순서는 중요하다
        └ elevator_02
        └ elevator_03
        └  .....

************************************************************************************************************/
public class Hold_Switch_Show : MonoBehaviour {

    bool isActive;
    bool isOnBox;
    float space = 2.5f;
    int elevatorCnt;                    // 엘리베이터 개수
    int curIdx;                         // 현재 엘레베이터 인덱스
    
    public float nextExecutionLevel = 0.65f;

    public GameObject elevatorParent;
    public GameObject scriptArea;
    struct ElevatorInfo
    {
        public GameObject elevator;     // 엘레베이터 본체
        public MeshRenderer meshRender; // 색정보 변경을 위한 변수
        public Color color;             // 색정보 (오퍼시티 값을 조절하기 위해)
        public BoxCollider[] colliders; // 콜리더 ( 한 발판에 콜리더 여러개 붙어 있는 경우가 있음)
        public Vector3 orignPos;        // 기본 위치
        public Vector3 destinationPos;  // 목표 위치
        public bool curMoveing;         // 움직임 상태 체크
        public float executionLevel;    // 실행정도
    }
    public Transform viewTartgetPos;
    ElevatorInfo[] elevators;

    GameObject holdObj;
    private Shader standard;

    void Start () {
        isActive = false;
        curIdx = 0;
        elevatorsInit();

        standard = Shader.Find("Standard");
    }

    IEnumerator SetObjectAlpha()
    {
        float alpha = 0f;
        float speed = 2f;
        Color objColor = holdObj.GetComponent<Renderer>().material.color;
        while (true)
        {
            // alpha가 Clamp01 함수를 사용하여 1까지만 올라가도록 함.
            alpha += speed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            // 현재 오브젝트의 알파값 변경
            objColor.a = alpha;
            holdObj.GetComponent<Renderer>().material.color = objColor;

            // 알파값이 1이고 현재 셰이더가 스탠다드 셰이더가 아닐 시 스탠다드 셰이더로 변경
            if (alpha == 0 && holdObj.GetComponent<Renderer>().material.shader != standard)
            {
                holdObj.GetComponent<Renderer>().material.shader = standard;
            }

            yield return null;
        }
    }
    
    IEnumerator Play;
    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("OBJECT") && !isActive)
        {
            isActive = true;
            isOnBox = true;
            scriptArea.SetActive(false);
            StopAllCoroutines();
            StartCoroutine(TimeAboutPlay(true));
        }
    }

    /*
    void OnCollisionExit(Collision col)
    {
        if (col.collider.CompareTag("OBJECT") && isOnBox)
        {
            isOnBox = false;
            StopAllCoroutines();
            StartCoroutine(TimeAboutPlay(false));
        }
    }
    */

        /*
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isOnBox)
        {
            StopAllCoroutines();
            StartCoroutine(TimeAboutPlay(true));
        }
    }
    */

    /*
    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player") && !isOnBox)
        {
            StopAllCoroutines();
            StartCoroutine(TimeAboutPlay(false));
        }
    }
    */

    IEnumerator TimeAboutPlay(bool isMoveUp)
    {
        /*
        if(!isMoveUp)
        {
            StartCoroutine(OpacityUp(curIdx, false));
            StartCoroutine(moveUP(curIdx, false));
            curIdx--;
        }*/
        CameraCtrl_1.instance.StartViewTargetCam(viewTartgetPos);
        yield return new WaitForSeconds(2f);
        int curIdx = 0;
        while (true)
        { 
            if(isMoveUp)
            {
                if(curIdx == 0 ) 
                {
                    StartCoroutine(OpacityUp(curIdx, true));
                    StartCoroutine(moveUP(curIdx, true));
                    curIdx++;
                }
                else if(curIdx < elevatorCnt && elevators[curIdx-1].executionLevel > nextExecutionLevel)
                {
                    StartCoroutine(OpacityUp(curIdx, true));
                    StartCoroutine(moveUP(curIdx, true));

                    if (curIdx + 1 < elevatorCnt)
                        curIdx++;
                    else
                        break;
                }
            }
            else
            {
                
                if (curIdx == 0)
                {
                    StartCoroutine(OpacityUp(curIdx, false));
                    StartCoroutine(moveUP(curIdx, false));
                    curIdx++;
                }
                else if (curIdx < elevatorCnt && elevators[curIdx - 1].executionLevel > -nextExecutionLevel)
                {
                    StartCoroutine(OpacityUp(curIdx, false));
                    StartCoroutine(moveUP(curIdx, false));

                    if (curIdx + 1 < elevatorCnt)
                        curIdx++;
                    else
                        break;
                }
            }
            yield return null;
        } 
    }

    // 자식 오브젝트 받아 올 때 ( 자식이 없으면 자신을 반환 )
    GameObject[] GetChildObj(GameObject obj)
    {
        Transform[] tempObjs = obj.GetComponentsInChildren<Transform>();
        int arraySize = tempObjs.Length;
        arraySize = arraySize > 1 ? arraySize - 1 : arraySize;
        GameObject[] objs = new GameObject[arraySize];

        if (arraySize == 1)
            objs[0] = tempObjs[0].gameObject;

        for (int i = 0; i < tempObjs.Length - 1; i++)
            objs[i] = tempObjs[i + 1].gameObject;

        return objs;
    }

    // 엘리베이터 초기화
    void elevatorsInit()
    {
        GameObject[] objs = GetChildObj(elevatorParent);

        elevatorCnt = objs.Length;
        elevators = new ElevatorInfo[elevatorCnt];

        for (int i = 0; i < elevatorCnt; i++)
        {
            elevators[i].elevator = objs[i];
            elevators[i].meshRender = elevators[i].elevator.GetComponent<MeshRenderer>();
            elevators[i].color = new Color(elevators[i].meshRender.material.color.r,
                                           elevators[i].meshRender.material.color.g,
                                           elevators[i].meshRender.material.color.b,
                                           -1);
            elevators[i].meshRender.material.color = elevators[i].color;
            elevators[i].colliders = elevators[i].elevator.GetComponentsInChildren<BoxCollider>();

            for(int j = 0; j < elevators[i].colliders.Length; j++)
                elevators[i].colliders[j].enabled = false;

            elevators[i].orignPos = elevators[i].elevator.transform.position;
            elevators[i].destinationPos = elevators[i].elevator.transform.position;
            elevators[i].destinationPos.y -= space;
            elevators[i].elevator.transform.position = elevators[i].destinationPos;
            elevators[i].curMoveing = false;
        }
    }

    IEnumerator moveUP(int idx, bool isUp)
    {
        // 실행중인 다른 코루틴을 정지 시키기 위해
        elevators[idx].curMoveing = false;
        yield return null;
        elevators[idx].curMoveing = true;

        while (elevators[idx].curMoveing)
        {
            if (isUp)
            {
                elevators[idx].elevator.transform.position = Vector3.Lerp(elevators[idx].elevator.transform.position, elevators[idx].orignPos, 3f * Time.deltaTime);
                if (Vector3.Distance(elevators[idx].elevator.transform.position, elevators[idx].orignPos) < 0.05f)
                    break;

                yield return null;
            }
            else
            {
                elevators[idx].elevator.transform.position = Vector3.Lerp(elevators[idx].elevator.transform.position, elevators[idx].destinationPos, 3f * Time.deltaTime);
                if (Vector3.Distance(elevators[idx].elevator.transform.position, elevators[idx].destinationPos) < 0.05f)
                    break;

                yield return null;
            }
        }
    }

    IEnumerator OpacityUp(int idx, bool isUp)
    {
        if (isUp)
        {
//            tempColor.a = 0;
            while (true)
            {
                elevators[idx].color.a += 3f * Time.deltaTime; ;
                elevators[idx].meshRender.material.color = elevators[idx].color;
                elevators[idx].executionLevel = elevators[idx].color.a;                   // 진행 정도를 저장하기 위해

                if (elevators[idx].color.a > 1f)
                {
                    for (int i = 0; i < elevators[idx].colliders.Length; i++)
                    {
                        elevators[idx].colliders[i].enabled = true;
                    }
                    break;
                }
                yield return null;
            }
        }
        else
        {
            while (true)
            {
                elevators[idx].color.a -= 3f * Time.deltaTime;
                elevators[idx].meshRender.material.color = elevators[idx].color;
                elevators[idx].executionLevel = elevators[idx].color.a;                   // 진행 정도를 저장하기 위해

                if (elevators[idx].color.a < -1f)
                {
                    for (int i = 0; i < elevators[idx].colliders.Length; i++)
                    {
                        elevators[idx].colliders[i].enabled = false;
                    }
                    break;
                }

                yield return null;
            }
            
        }
    }
}
