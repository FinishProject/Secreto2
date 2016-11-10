using UnityEngine;
using System.Collections;

/************************************ 사용 방법 ****************************************

    배치 되어있는 라인을 따라 움직이는 카메라
    라인은 위치 정보를 가진 노드들로 이루어져 있다
    ※ 사용 방법
    1. 카메라에 스크립트를 추가한다
    2. 카메라가 지나갈 위치에 CamNode_2 스크립트를 추가한다. ( CamNode_2 사용법 참고 )

****************************************************************************************/

public class CameraCtrl_7 : MonoBehaviour
{

    Transform tr;                   // 현재 Transform
    Transform playerTr;             // 현재 Transform

    public Transform prevNode;      // 이전 위치 Node의 Transform정보
    public Transform curNode;       // 현재 위치 Node의 Transform정보
    public Transform nextNode;      // 다음 위치 Node의 Transform정보 

    Transform[] prevNodes;          // 이전 위치 Node의 Transform정보
    Transform[] nextNodes;          // 이전 위치 Node의 Transform정보

    Vector3 nodePointPos;
    Vector3 nodePointRot;
    Vector3 nodePointRevision;      // 카메라 노드 포인트값의 보정치
    Vector3 nodeVector;             // 방향 벡터
    Vector3 camAddPos;              // 기본 캐릭터와 카메라 사이의 거리 차이
    Vector3 quat;

    int nextNodeIdx;
    float curRange;                 // 다음 노드 까지의 거리
    float totalRange;               // 총 거리
    float ratio;                    // 비율
    bool curRightSide;              // cutNode기준 오른쪽 방향에 있는지 체크


    void Start()
    {
        tr = transform;
        playerTr = PlayerCtrl.instance.transform;
        camAddPos = tr.position - playerTr.position;

        curRightSide = true;

        nodeVector = nextNode.position - curNode.position;
        quat = nextNode.eulerAngles - curNode.eulerAngles;
    }

    // 현재 카메라의 포지션이 curNode 어느 방향에 있는지 체크
    void CheckSide()
    {
        if (tr.position.x > curNode.position.x)
            curRightSide = true;
        else
            curRightSide = false;
    }

    void CheckNodePos()
    {
        // 진행방향이 오른쪽일 때
        if (PlayerCtrl.isFocusRight)
        {
            // 오른편에서 nextNode를 지났을 때
            if (curRightSide && nextNode.position.x < tr.position.x)
            {
                GetNextNode();
                nodeVector = nextNode.position - curNode.position;
                quat = nextNode.eulerAngles - curNode.eulerAngles;
            }
            // 왼편에서 curNode를 지났을 때
            else if (!curRightSide && curNode.position.x < tr.position.x)
            {
                nodeVector = nextNode.position - curNode.position;
                quat = nextNode.eulerAngles - curNode.eulerAngles;

            }
        }
        // 진행 방향이 왼쪽일 때
        else
        {
            // 오른편에서 curNode를 지났을 때
            if (curRightSide && curNode.position.x > tr.position.x)
            {
                nodeVector = prevNode.position - curNode.position;
                quat = prevNode.eulerAngles - curNode.eulerAngles;

            }
            // 왼편에서 prevNode를 지났을 때
            else if (!curRightSide && prevNode.position.x > tr.position.x)
            {
                GetPrevNode();
                nodeVector = prevNode.position - curNode.position;
                quat = prevNode.eulerAngles - curNode.eulerAngles;

            }
        }
        nodeVector.x = 0;   // X값은 캐릭터위치를 따라 가야하므로 0
    }

    // 근처 노드를 검사
    Transform getNearTr(Transform[] nodesTr)
    {
        int nearIdx = 0;
        float nearRange = 9999;

        float curRange;
        for (int i = 0; i < nodesTr.Length; i++)
        {
            curRange = Vector3.Distance(tr.position, nodesTr[i].position);
            if (curRange < nearRange)
            {
                nearRange = curRange;
                nearIdx = i;
            }
        }
        return nodesTr[nearIdx];
    }

    void GetNextNode()
    {
        curNode = nextNode;

        nextNodes = curNode.GetComponent<CamNode_2>().NextNodes;
        prevNodes = curNode.GetComponent<CamNode_2>().PrevNodes;

        nextNode = nextNodes[0];
        prevNode = prevNodes[0];
    }

    void GetPrevNode()
    {
        curNode = prevNode;
        nextNodes = curNode.GetComponent<CamNode_2>().NextNodes;
        prevNodes = curNode.GetComponent<CamNode_2>().PrevNodes;

        nextNode = nextNodes[0];
        prevNode = prevNodes[0];
    }


    void Update()
    {
        CheckNodePos();
        CheckSide();

        if (curRightSide)
        {
            totalRange = Mathf.Round((nextNode.position.x - curNode.position.x) * 100) / 100;
            curRange = Mathf.Round((nextNode.position.x - tr.position.x) * 100) / 100;
        }
        else
        {
            totalRange = Mathf.Round((prevNode.position.x - curNode.position.x) * 100) / 100;
            curRange = Mathf.Round((prevNode.position.x - tr.position.x) * 100) / 100;
        }

        ratio = 1 - (curRange / totalRange);                                  // (다음노드 까지 거리 / 총거리의) 비율
        nodePointPos = curNode.position - (playerTr.position + camAddPos);    // curNode.position - (playerTr.position + camAddPos) 로 바꿔주면 정말 루트 대로 움직임
        nodePointRot = curNode.eulerAngles;

        nodePointPos.x = 0;                                        // x 축은 캐릭터를 따라감
        nodePointRevision = nodePointPos + (nodeVector * ratio);

        tr.position = playerTr.position + camAddPos + nodePointRevision;

        tr.eulerAngles = nodePointRot + (quat * ratio);



        ratio = 1 - (curRange / totalRange);                                  
        nodePointPos = curNode.position - (playerTr.position + camAddPos);    
        nodePointRevision = nodePointPos + (nodeVector * ratio);
        tr.position = playerTr.position + camAddPos + nodePointRevision;
    }
}
