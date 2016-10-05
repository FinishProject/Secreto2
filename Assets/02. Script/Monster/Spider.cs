using UnityEngine;
using System.Collections;

public class Spider : FSMBase
{

    private NavMeshAgent nvAgent;
    private Vector3 earlyPos;
    private Transform playerTr;
    public float discoverDist = 12f;
    public float traceDist = 9f;
    public float attackDist = 2f;
    public float damage = 16;
    private System.Enum oldStates;
    private bool isPlayerLiftSide;          // 플레이어가 좌측에 있는지

    // 몬스터 상태
    public enum EnemyStates
    {
        Dying = 0,      // 사망
        Attacked = 2,   // 피격
        Discover = 3,   // 발견
        Chase = 4,      // 추적
        ComBback = 5,   // 복귀
        Attacking = 6,  // 공격
        Idle = 7,       // 대기

    }

    void Start()
    {
        earlyPos = transform.position;
        nvAgent = GetComponent<NavMeshAgent>(); //anim.SetBool()
        playerTr = PlayerCtrl.instance.transform;
    }

    //*******************************************************************************

    #region 대기
    IEnumerator Idle_EnterState()
    {
        Debug.Log("Idle 상태 돌입");
        yield return null;
    }

    void Idle_Update()
    {
        float distance = Vector3.Distance(playerTr.position, transform.position);
        if (distance <= discoverDist)
        {
            anim.SetBool("Prepare", true);
            curState = EnemyStates.Discover;
        }
        else
            anim.SetBool("Prepare", false);
    }
    #endregion

    //*******************************************************************************

    #region 발견
    IEnumerator Discover_EnterState()
    {
        Debug.Log("Discover 상태 돌입");
        yield return null;
    }

    void Discover_Update()
    {
        float distance = Vector3.Distance(playerTr.position, transform.position);
        if (distance <= traceDist)
        {
            anim.SetBool("Run", true);
            anim.SetBool("Prepare", false);
            curState = EnemyStates.Chase;
            return;
        }
        else if (distance > discoverDist)
        {
            anim.SetBool("Prepare", false);
            curState = EnemyStates.Idle;
        }

    }
    #endregion

    //*******************************************************************************

    #region 추적
    IEnumerator Chase_EnterState()
    {
        Debug.Log("Chase 상태 돌입");
        anim.speed = 2.8f;
        nvAgent.Resume();
        yield return null;
    }

    void Chase_Update()
    {
        float distance = Vector3.Distance(playerTr.position, transform.position);

        if(!canActiveAngle())
        {
            nvAgent.destination = transform.position;
            transform.Rotate(new Vector3(0, 1, 0), 100f * Time.deltaTime);
        }
        else
        {
            nvAgent.destination = playerTr.position;
        }

        if (distance <= attackDist && canActiveAngle())
        {
            anim.SetBool("Attack", true);
            anim.SetBool("Run", false);
            curState = EnemyStates.Attacking;
            return;
        }

        if (distance > traceDist)
        {
            curState = EnemyStates.ComBback;
            return;
        }

    }

    bool canActiveAngle()
    {
        if (Mathf.Abs(transform.localEulerAngles.y - 270f) < 5 &&  isPlayerLiftSide ||
            Mathf.Abs(transform.localEulerAngles.y - 90f)  < 5 && !isPlayerLiftSide)
            return true;
        return false;
    }

    IEnumerator Chase_ExitState()
    {
        anim.speed = 1;
        nvAgent.Resume();
        nvAgent.destination = playerTr.position;
        yield return null;
    }
    #endregion

    //*******************************************************************************

    #region 복귀
    IEnumerator ComBback_EnterState()
    {
        Debug.Log("ComBback 상태 돌입");
        nvAgent.speed = 2.8f;
        anim.speed = 1.5f;
        yield return null;
    }

    void ComBback_Update()
    {
        nvAgent.destination = earlyPos;

        float distance = Vector3.Distance(playerTr.position, transform.position);

        if (distance <= traceDist)
        {
            curState = EnemyStates.Chase;
            return;
        }

        if (Vector3.Distance(earlyPos, transform.position) < 0.5f)
        {
            anim.SetBool("Prepare", true);
            anim.SetBool("Run", false);
            curState = EnemyStates.Idle;
            return;
        }

    }

    IEnumerator ComBback_ExitState()
    {
        nvAgent.speed = 3.8f;
        anim.speed = 1f;
        yield return null;
    }
    #endregion

    //*******************************************************************************

    #region 공격

    IEnumerator Attacking_EnterState()
    {
        Debug.Log("Attacking 상태 돌입");
        nvAgent.Stop();
        yield return null;
    }

    void Attacking_Update()
    {
        float distance = Vector3.Distance(playerTr.position, transform.position);
        if (distance > attackDist || !canActiveAngle())
        {
            anim.SetBool("Run", true);
            anim.SetBool("Attack", false);
            curState = EnemyStates.Chase;
            return;
        }
    }

    IEnumerator Attacking_ExitState()
    {
        nvAgent.Resume();
        nvAgent.destination = playerTr.position;
        yield return null;
    }

    void Attacking_OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Player"))
            GiveDamage();
    }

    #endregion

    //*******************************************************************************

    #region 피격

    IEnumerator Attacked_EnterState()
    {
        Debug.Log("Attacked 상태 돌입");

        nvAgent.Stop();
        anim.SetTrigger("Attacked");

        if (curHp <= 0)
        {
            anim.SetBool("Run", false);
            anim.SetBool("Attack", false);
            curState = EnemyStates.Dying;
        }
        else
        {
            curState = oldStates;
        }
        yield return new WaitForSeconds(0.2f);
    }

    #endregion

    //*******************************************************************************

    #region 사망
    IEnumerator Dying_EnterState()
    {
        isDeath = true;
        Debug.Log("쮸금");
        nvAgent.Stop();
        anim.SetBool("Death", true);
        GetComponent<ItemDrop>().DropItem();

        yield return new WaitForSeconds(2f);

        gameObject.SetActive(false);
        MonsterRespawnMgr.instance.Respawn(gameObject);
        yield return null;
    }

    void OnEnable()
    {
        isDeath = false;
        curState = EnemyStates.Idle;
    }

    void OnDisable()
    {
        curHp = oldHp;
        transform.position = earlyPos;
    }


    #endregion

    //*******************************************************************************

    #region 기타 함수

    // 데미지 입었을때 ( 외부 호출 )
    public override void GetDamage(float damage)
    {
        base.GetDamage(damage);

        Debug.Log("피격");
        if (!curState.Equals(EnemyStates.Dying) && !curState.Equals(EnemyStates.Attacked))
        {
            oldStates = curState;
            curState = EnemyStates.Attacked;
        }

        /*
        if (hp <= 0)
        {
            anim.SetBool("Prepare", false);
            anim.SetBool("Run", false);
            anim.SetBool("Attack", false);
            curState = EnemyStates.Dying;
        }
        */
    }

    // 업데이트 함수 ( 모든 상태일때 적용 할 Update )
    public override void ChildUpdate()
    {
        isPlayerLiftSide = playerTr.position.x < transform.position.x ? true : false;
    }


    // 데미지 줄때
    public void GiveDamage()
    {
        PlayerCtrl.instance.getDamage(10);
    }
    #endregion
}
