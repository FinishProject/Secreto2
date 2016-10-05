using UnityEngine;
using System.Collections;

public abstract class WasteItem : MonoBehaviour {
    public float removeTime = 5.0f;         // 아이템이 사라지는데 걸리는 시간
    public float chaseRange = 5f;           // 추격 거리
    protected ItemStruct itemData;            // 아이템의 정보
    protected Transform thisTr;               // 위치 확인
    protected Transform playerTr;             // 플레이어 위치
    protected bool isGetPossible = false;     // 플레이어가 아이템을 습득 가능한지 판단
    protected bool isCollSomething = false;   // 땅이나 오브젝트 들에 충돌 했는지 체크
    protected bool isChasePlayer = false;     // 플레이어를 추격 중인지 확인
    protected float gr = 5f;

    // 자식 클래스에서 재정의 필요
    public abstract void initData(bool isBulkItem);
    
    // 드랍 해주는 함수       시작 위치        목표 위치       대용량 아이템 인가
    public void Dropped(Vector3 StartPos, Vector3 tagetPos, bool isBulkItem)
    {
        gameObject.SetActive(true);
        initData(isBulkItem);
        StartCoroutine(Moving(StartPos, tagetPos));
        StartCoroutine(OffActive());
    }

    // 목표지점까지 움직임
    IEnumerator Moving(Vector3 StartPos, Vector3 tagetPos)
    {
        thisTr = GetComponent<Transform>();
        thisTr.position = StartPos;

        float oldTime = 0;
        while (true)
        {
            oldTime += Time.deltaTime;
            // 일정 시간이 지나야지 습득 가능
            if (oldTime >= 0.4f)
            {
                isGetPossible = true;
                StartCoroutine(fall());
            }

            if (isGetPossible && Vector3.Distance(thisTr.position, playerTr.position) < chaseRange)
            {
                isChasePlayer = true;
                StartCoroutine(ChasePlayer());
                break;
            }

            // 목표위치 (산개 위치)까지 이동
            if (!isGetPossible && Vector3.Distance(thisTr.position, tagetPos) > 0.5f)
            {
                thisTr.position = Vector3.Lerp(thisTr.position, tagetPos, Time.deltaTime);
            }

            yield return null;
        }
    }

    // 바닥으로 떨어짐
    IEnumerator fall()
    {
        yield return new WaitForSeconds(1.0f);
        thisTr = GetComponent<Transform>();
        Vector3 tempPos = thisTr.position;
        while (true)
        {
            if (isCollSomething || isChasePlayer)
                break;

            tempPos.y -= gr * Time.deltaTime;
            thisTr.position = Vector3.Lerp(thisTr.position, tempPos, Time.deltaTime);

            yield return null;
        }
    }

    // 시간내 먹지않으면 사라짐
    IEnumerator OffActive()
    {
        yield return new WaitForSeconds(removeTime);
        gameObject.SetActive(false);
    }

    IEnumerator ChasePlayer()
    {
        while (true)
        {
            thisTr.position = Vector3.Slerp(thisTr.position, playerTr.position, (Vector3.Distance(thisTr.position, playerTr.position) * 0.5f + 1f) * Time.deltaTime);

            yield return null;
        }
    }
}
