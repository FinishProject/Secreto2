using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    서로 다른 위치에 있는 문 오브젝트를 이동할 수 있는 클래스

    사용방법 :

    출구가 될 오브젝트를 eixtGate에 삽입하여 사용해야한다.
        
*************************************************************/

public class TeleportGate : MonoBehaviour {

    public Transform exitGate;
    private TeleportGate exitTelpo;
    private Transform boxTr;

    public bool isRight = true;
    private bool isBox = false;
    private float focusDir = 1;

    void Start()
    {
        exitTelpo = exitGate.GetComponent<TeleportGate>();

        if (!isRight)
            focusDir = -1f;
    }

    void OnTriggerEnter(Collider col)
    {
        // 플레이어 체크
        if (col.CompareTag("Player"))
        {
            
            FadeInOut.instance.StartFadeInOut(1f, 1.8f, 1f);
            StartCoroutine(MoveGate());
        }
        // 오브젝트 체크, 오브젝트 있을 시 플레이어와 같이 이동하기 위해
        else if (col.CompareTag("OBJECT"))
        {
            isBox = true;
            boxTr = col.transform;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("OBJECT"))
        {
            isBox = false;
        }
    }

    IEnumerator MoveGate()
    {
        // 출구 위치 설정
        Vector3 exitPoint = exitGate.position;
        exitPoint += Vector3.right * focusDir * 3f;
        exitPoint -= Vector3.up * 4.5f;
        StartCoroutine(Movement());
        yield return new WaitForSeconds(1f);

        // 오브젝트가 있을 시 오브젝트 이동
        if (isBox && boxTr != null)
        {
            boxTr.position = new Vector3(exitPoint.x + (3f * focusDir), exitPoint.y + 7f, boxTr.position.z);
        }
        // 플레이어 이동
        PlayerCtrl.instance.transform.position = exitPoint;
        PlayerCtrl.instance.TurnPlayer();
    }

    IEnumerator Movement()
    {
        int playingAnim = PlayerCtrl.instance.GetPlayingAnimation();

        if(playingAnim == Animator.StringToHash("Base Layer.Jump"))
            Debug.Log(playingAnim);
        while (true)
        {
            
            yield return null;
        }
    }
}

