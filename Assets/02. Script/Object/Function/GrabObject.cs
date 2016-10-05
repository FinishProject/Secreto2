using UnityEngine;
using System.Collections;

public class GrabObject : MonoBehaviour {

    private Transform playerTr; // 플레이어 위치
    private Rigidbody rb;
    private float focusDir = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
    }

	void OnTriggerStay(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            // E 키 입력 시 오브젝트 잡기
            if (Input.GetKey(KeyCode.E))
            {
                if (PlayerCtrl.isFocusRight)
                {
                    focusDir = 1f;
                }
                else
                    focusDir = -1f;

                // 플레이어의 자식 객체로 들어감
                this.transform.parent = playerTr.parent;
                rb.isKinematic = true;
                // 오브젝트 이동
                //Vector3 relativePos = playerTr.position - transform.position;
                transform.position = Vector3.Lerp(transform.position, 
                    new Vector3(playerTr.position.x + 1f * focusDir, playerTr.position.y + 0.8f, transform.position.z), 200f);
            }
            // 플레이어의 자식 객체에서 나옴
            else if (Input.GetKeyUp(KeyCode.E)) {
                this.transform.parent = null;
                rb.isKinematic = false;
            }
        }
    }
}
