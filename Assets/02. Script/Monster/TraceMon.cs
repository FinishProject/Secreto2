using UnityEngine;
using System.Collections;

public class TraceMon : MonoBehaviour {

    private Vector3 startPos, finishPos; // 시작 위치, 도착 위치

    private float time = 0f, gr = 9.8f; // 시간, 중력값

    public float damage = 200f;

    void Awake () {
        startPos = this.transform.position;   // 시작 위치
        finishPos = new Vector3(startPos.x, 0f, startPos.z - 25f); // 도착 위치
    }

    void Update()
    {
        // 바닥 아래로 내려가면 다음 오브젝트로 변경
        if (this.transform.position.y <= -5f)
            TraceMonMgr.instance.SetChildObj();
        else
        { // 포물선을 그리며 이동
            float vz = (finishPos.z - startPos.z) * 0.5f; // z 속도
            float vy = (finishPos.y - startPos.y + 2f * gr) * 0.5f; // y 속도

            time += Time.deltaTime;

            float sz = startPos.z + vz * time; // z 값
            float sy = startPos.y + vy * time - 0.5f * gr * time * time; // y 값
            transform.position = new Vector3(startPos.x, sy, sz);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            PlayerCtrl.instance.getDamage(damage);
        }
    }

    void OnEnable()
    {
        this.transform.position = startPos;
        time = 0f;
    }
}

