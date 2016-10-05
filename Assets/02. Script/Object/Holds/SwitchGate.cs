using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    특정 오브젝트를 스위치 위에 올리면 문이 열리게하는 클래스

    사용방법 :
            
*************************************************************/

public class SwitchGate : MonoBehaviour {

    
    public float speed;
    public float maxLength; // 최대 이동 거리
    private bool isActive = false;

    public GameObject[] gates; // 양쪽 문 오브젝트
    private Vector3[] finishPos; // 최종 위치

    void Start()
    {
        finishPos = new Vector3[gates.Length];
        // 최종 위치를 구함
        for (int i = 0; i < finishPos.Length; i++)
        {
            finishPos[i] = gates[i].transform.position;
            finishPos[i].y += maxLength;
            maxLength *= -1f;
        }
    }

	void OnTriggerEnter(Collider col)
    {
        
        // 특정 오브젝트와 충돌시(위에 올려졌을 시)
        if (col.CompareTag("OBJECT") && !isActive)
        {
            StartCoroutine(OpenGate());
        }
    }

    IEnumerator OpenGate()
    {
        isActive = true;
        while (true)
        {
            for(int i=0; i<gates.Length; i++)
            {
                if (gates[0].transform.position.y >= finishPos[0].y)
                {
                    for (int j = 0; j < gates.Length; j++)
                    {
                        gates[j].SetActive(false);
                    }
                    break;
                }
                
                gates[i].transform.Translate(Vector3.forward * speed * Time.deltaTime);
                speed *= -1f;
            }
            yield return null;
        }
        
    }
}
