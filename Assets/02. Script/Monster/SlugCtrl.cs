using UnityEngine;
using System.Collections;

enum State
{
    idle, trace, attack,
};

public class SlugCtrl : MonoBehaviour {

    State state = State.idle;

    public float traceDis = 8f;
    public float attackDis = 5f;

    private NavMeshAgent nvAgent;
    private Transform playerTr;

    void Start()
    {
        nvAgent = GetComponent<NavMeshAgent>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(FindTarget());
        StartCoroutine(MoveMonster());
    }

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
                case State.attack:
                    break;
            }
            yield return null;
        }
    }

    IEnumerator FindTarget()
    {
        while (true)
        {
            float distance = (playerTr.position - transform.position).sqrMagnitude;
            Debug.Log(state);
            if (distance <= attackDis)
            {
                state = State.attack;
            }

            else if (distance <= traceDis)
            {
                state = State.trace;
            }
            
            else if (distance >= traceDis)
            {
                state = State.idle;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
