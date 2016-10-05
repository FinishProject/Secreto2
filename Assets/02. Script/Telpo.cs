using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    서로 다른 위치에 있는 문 오브젝트를 이동할 수 있는 클래스

    사용방법 :

    출구가 될 오브젝트를 eixtGate에 삽입하여 사용해야한다.
        
*************************************************************/

public class Telpo : MonoBehaviour
{

    public GameObject exitGate; // 출구 오브젝트
    public GameObject box;

    public Transform spawnPoint;

    private bool isTravel = true; // 이동 가능 여부
    private bool isBox = false;
    private Telpo telepGate; // 출구 오브젝트의 스크립트를 담을 변수
    private Transform olaTr, camTr;
    private Transform playerTr;

    void Start()
    {
        if (exitGate != null)
        {
            telepGate = exitGate.GetComponent<Telpo>();
        }
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        olaTr = GameObject.FindGameObjectWithTag("WAHLE").transform;
        //camTr = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("OBJECT"))
        {
            isBox = true;
        }

        else if (col.CompareTag("Player") && isTravel)
        {
            if (isBox)
            {
                telepGate.Block();
                StartCoroutine(Toghter(col));
            }
            else
            {
                // 반대편에 도착 시 잠시 이동 불가능하게 만듬
                telepGate.Block();
                StartCoroutine(MovePoint(col));
            }
        }
    }

    public void Block()
    {
        StartCoroutine(BlockTravel());
    }

    // 이동 후 잠시 텔레포트 이동 불가
    IEnumerator BlockTravel()
    {
        isTravel = false;
        yield return new WaitForSeconds(3f);
        isTravel = true;
    }

    IEnumerator MovePoint(Collider col)
    {
        CameraCtrl_6.instance.StartTeleport();

        // 출구 위치로 이동
        Vector3 movePoint = exitGate.transform.position;
        movePoint += exitGate.transform.up * 3f;
        movePoint -= exitGate.transform.forward * 3f;

        col.transform.position = movePoint;
        olaTr.position = movePoint;

        //        yield return new WaitForSeconds(0.5f);

        FadeInOut.instance.StartFadeInOut(1f, 1.8f, 1f);

        yield return new WaitForSeconds(1f);
        CameraCtrl_6.instance.EndTeleport();
    }

    IEnumerator Toghter(Collider col)
    {
        CameraCtrl_6.instance.StartTeleport();

        Vector3 movePoint = exitGate.transform.position;
        movePoint += exitGate.transform.up * 3f;
        movePoint -= exitGate.transform.forward * 3f;

        playerTr.transform.position = movePoint;
        box.transform.position = new Vector3(movePoint.x + 3f, movePoint.y, 0.71f);
        olaTr.position = movePoint;

        FadeInOut.instance.StartFadeInOut(1f, 1.8f, 1f);

        yield return new WaitForSeconds(1f);
        CameraCtrl_6.instance.EndTeleport();
        isBox = false;
    }
}

