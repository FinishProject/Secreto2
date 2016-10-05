using UnityEngine;
using System.Collections;


public class Hold_Switch_Show : MonoBehaviour {

    bool isActive;
    bool isOnBox;
    float space = 2.5f;
    int elevatorCnt;                    // 엘리베이터 개수
    int curIdx;                         // 현재 엘레베이터 인덱스
    
    public float nextExecutionLevel = 0.65f;

    public GameObject elevatorParent;
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
    ElevatorInfo[] elevators;


    void Start () {
        isActive = false;
        curIdx = 0;
        elevatorsInit();
    }
    IEnumerator Play;
    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("OBJECT"))
        {
            isOnBox = true;

            StopAllCoroutines();
            StartCoroutine(TimeAboutPlay(true));
        }
    }


    void OnCollisionExit(Collision col)
    {
        if (col.collider.CompareTag("OBJECT") && isOnBox)
        {
            isOnBox = false;
            StopAllCoroutines();
            StartCoroutine(TimeAboutPlay(false));
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isOnBox)
        {
            StopAllCoroutines();
            StartCoroutine(TimeAboutPlay(true));
        }
    }


    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player") && !isOnBox)
        {
            StopAllCoroutines();
            StartCoroutine(TimeAboutPlay(false));
        }
    }
    
    IEnumerator TimeAboutPlay(bool isMoveUp)
    {
        /*
        if(!isMoveUp)
        {
            StartCoroutine(OpacityUp(curIdx, false));
            StartCoroutine(moveUP(curIdx, false));
            curIdx--;
        }*/
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
                
                /*
                if (curIdx > 0 && elevators[curIdx + 1].executionLevel < -nextExecutionLevel) 
                {
                    StartCoroutine(OpacityUp(curIdx, false));
                    StartCoroutine(moveUP(curIdx, false));
                    curIdx--;
                }
                else if (curIdx == 0)
                {
                    StartCoroutine(OpacityUp(curIdx, false));
                    StartCoroutine(moveUP(curIdx, false));
                    break;
                }
                */
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

    //0.35
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

    /*
    IEnumerator OpacityUp(int idx, bool isUp)
    {
        Color tempColor = elevators[idx].color;

        if (isUp)
        {
//            tempColor.a = 0;
            while (true)
            {
                tempColor.a += 3f * Time.deltaTime; ;
                elevators[idx].meshRender.material.color = tempColor;
                elevators[idx].executionLevel = tempColor.a;                   // 진행 정도를 저장하기 위해

                if (tempColor.a > 1f)
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
            //            tempColor.a = 1;
            Debug.Log(tempColor.a);
            while (true)
            {
                tempColor.a -= 3f * Time.deltaTime;
                elevators[idx].meshRender.material.color = tempColor;
                elevators[idx].executionLevel = tempColor.a;                   // 진행 정도를 저장하기 위해
                Debug.Log(idx + " : " + tempColor.a);
                if (tempColor.a < -1f)
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
    }*/
}

/*
 public class Hold_Switch_Show : MonoBehaviour {

    public GameObject elevator;
    bool isOnBox;
    MeshRenderer[] meshRender;
    BoxCollider[] colliders;
    GameObject[] elevators;

    Vector3 orignPos;
    Vector3 destinationPos;
    float space = 2.5f;

    void Start () {
        meshRender = elevator.GetComponentsInChildren<MeshRenderer>();
        colliders = elevator.GetComponentsInChildren<BoxCollider>();

        for (int i = 0; i < meshRender.Length; i++)
        {
            meshRender[i].material.color = new Color(meshRender[0].material.color.r, meshRender[0].material.color.g, meshRender[0].material.color.b, -1);
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        isOnBox = false;

        destinationPos = orignPos = elevator.transform.position;
        destinationPos.y -= space;
        elevator.transform.position = destinationPos;
    } 
    
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            StartCoroutine(changeOpacity(false));
            StartCoroutine(moveUP(false));
        }

        if (col.CompareTag("OBJECT"))
        {
            isOnBox = true;
            StartCoroutine(changeOpacity(false));
        }
        
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if(isOnBox)
             return;
            StartCoroutine(changeOpacity(true));
            StartCoroutine(moveUP(true));
        }
        else if(col.CompareTag("OBJECT"))
        {
            StartCoroutine(changeOpacity(true));
            isOnBox = false;
        }    
    }
    // 0.35
    IEnumerator moveUP(bool isUp)
    {
        while (true)
        {
            if (isUp)
            { 
                elevator.transform.position = Vector3.Lerp(elevator.transform.position, orignPos, 3f * Time.deltaTime);
                if (Vector3.Distance(elevator.transform.position, orignPos) < 0.1f)
                    break;

                yield return null;
            }
            else
            {
                elevator.transform.position = Vector3.Lerp(elevator.transform.position, destinationPos, 3f * Time.deltaTime);
                if (Vector3.Distance(elevator.transform.position, destinationPos) < 0.1f)
                    break;

                yield return null;
            }
        }
    }
    

    IEnumerator changeOpacity(bool isClear)
    {
        Color tempColor = new Color(meshRender[0].material.color.r, meshRender[0].material.color.g, meshRender[0].material.color.b);

        if (isClear)
        {
            tempColor.a = 1;
            while (true)
            {
                Debug.Log(tempColor.a);
                tempColor.a -= 3f * Time.deltaTime;
                for (int i = 0; i < meshRender.Length; i++)
                {
                    meshRender[i].material.color = tempColor;
                }

                if(tempColor.a < -1f)
                {
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        colliders[i].enabled = false;
                    }
                    break;
                }
               
                yield return null;
            }
        }
        else
        {
            tempColor.a = 0;
            while (true)
            {
                Debug.Log("+ " + tempColor.a);

                tempColor.a += 3f * Time.deltaTime; ;
                for (int i = 0; i < meshRender.Length; i++)
                {
                    meshRender[i].material.color = tempColor;
                }

                if (tempColor.a > 1f)
                {
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        colliders[i].enabled = true;
                    }
                    break;
                }
                yield return null;
            }
        }


    }
}
*/
  