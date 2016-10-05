using UnityEngine;
using System.Collections;

public class SplitMon : MonoBehaviour {

    private enum State { idle, trace }; // 몬스터 상태

    public float traceDist = 10f; // 추격 범위

    private Transform playerTr; // 플레이어 위치
    private NavMeshAgent nvAgent;

    private State state = State.idle; // 상태

    MonsterMgr parentScript;

    void OnEnable()
    {
        nvAgent = GetComponent<NavMeshAgent>();
        parentScript = transform.parent.GetComponent<MonsterMgr>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine("FindTarget");
        StartCoroutine("MoveMonster");
    }
    //몬스터 이동
    IEnumerator MoveMonster()
    {
        while (true)
        {
            switch (state)
            {
                case State.idle:
                    nvAgent.Stop();
                    break;
                case State.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.Resume();
                    break;
            }
            yield return null;
        }
    }
    //타겟 탐색
    IEnumerator FindTarget()
    {
        while (true)
        {
            float distance = Vector3.Distance(playerTr.position, transform.position);

            if (distance <= traceDist) { state = State.trace; }
            else { state = State.idle; }

            yield return new WaitForSeconds(1f);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "OBJECT" && this.gameObject.name == "Parent_Mon") {
            parentScript.Split(this.transform.position);
            this.gameObject.SetActive(false);
        }
        else if(coll.gameObject.tag == "OBJECT")
        {
            gameObject.GetComponent<ItemDrop>().DropItem();
            this.gameObject.SetActive(false);
        }
    }
}
