using UnityEngine;
using System.Collections;

/*************************   정보   **************************

    몬스터 - 달팽이
    포물선 탄환 발사 공격을 하는 몬스터

    사용방법 :
    
    몬스터 오브젝트에 스크립트 추가
    ItemDrop 스크립트가 몬스터 오브젝트에 추가 되어 있는지 확인

*************************************************************/

public class Slug : FSMBase
{
    private NavMeshAgent nvAgent;           // 네비메쉬
    private Vector3 earlyPos;               // 초기 위치
    private Transform playerTr;             // 플레이어 위치

    public Transform shootTr;
    private bool isPlayerLiftSide;          // 플레이어가 좌측에 있는지
    private bool isLookLiftSide;            // 좌측 방향을 바라 보는지
    public float discoverDist = 12f;        // 인식 범위
    public float traceDist = 10f;           // 추적 범위
    public float attackDist = 7f;           // 공격 범위

    private float distance;                 // 플레이어와 몬스터 사이 거리
    public float damage = 16;               // 공격력
    public float attackErrorRange = 1.5f;   // 공격 오차 범위 ( 0일경우 타겟에 무조건 맞음)
    private System.Enum oldStates;          // 기존 스테이트 저장용

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
        nvAgent = GetComponent<NavMeshAgent>();
        playerTr = PlayerCtrl.instance.transform;
    }

    //*******************************************************************************

    #region 대기
    IEnumerator Idle_EnterState()
    {
        yield return null;
    }

    void Idle_Update()
    {
        // 인식범위 체크
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
        yield return null;
    }

    void Discover_Update()
    {
        // 추적 범위 체크
        if (distance <= traceDist)
        {
            anim.SetBool("Run", true);
            anim.SetBool("Prepare", false);
            curState = EnemyStates.Chase;
            return;
        }
        // 인식 범위 체크
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
        nvAgent.Resume();
        yield return null;
    }

    void Chase_Update()
    {
        nvAgent.destination = playerTr.position;

        // 인식 범위 및 공격가능 각도 체크
        if (distance <= attackDist && isAttackAngle())
        {
            anim.SetBool("Attack", true);
            anim.SetBool("Run", false);
            curState = EnemyStates.Attacking;
            return;
        }

        // 추적 범위 체크
        if (distance > traceDist)
        {
            curState = EnemyStates.ComBback;
            return;
        }

    }

    // 공격 가능한 회전 각인지 체크하는 함수 ( 회전을 마치고 공격을 하기위해 )
    bool isAttackAngle()
    {
        if (Mathf.Abs(transform.localEulerAngles.y - 270f) < 5 &&  isPlayerLiftSide  ||
            Mathf.Abs(transform.localEulerAngles.y - 90f)  < 5 && !isPlayerLiftSide )
            return true;
        return false;
    }
    #endregion

    //*******************************************************************************

    #region 복귀
    IEnumerator ComBback_EnterState()
    {
        yield return null;
    }

    void ComBback_Update()
    {
        nvAgent.destination = earlyPos;

        // 추적 범위 안에 들어오면 추석 상태로 변경
        if (distance <= traceDist)
        {
            curState = EnemyStates.Chase;
            return;
        }

        // 복귀지점 도착시 대기 상태로 변경
        if (Vector3.Distance(earlyPos, transform.position) < 0.5f)
        {
            anim.SetBool("Prepare", true);
            anim.SetBool("Run", false);
            curState = EnemyStates.Idle;
            nvAgent.Stop();
            return;
        }

    }
    #endregion

    //*******************************************************************************

    #region 공격

    IEnumerator Attacking_EnterState()
    {
        nvAgent.Stop();
        yield return null;
    }
    
    void Attacking_Update()
    {
        // 거리와 공격가능한 각도가 아니면 추적 상태로
        if (distance > attackDist || !isAttackAngle())
        {
            anim.SetBool("Run", true);
            anim.SetBool("Attack", false);
            curState = EnemyStates.Chase;
            return;
        }
    }

    // 발사를 위한 함수 ( 애니메이션 끝난 후 이벤트로 호출할 함수)
    void ShotBullet()
    {
        ObjectMgr.instance.GetParabolaBullet().UseItem().
                GetComponent<BulletObject_Parabola>().Moving(shootTr, attackErrorRange);
        
    }

    #endregion

    //*******************************************************************************

    #region 피격

    IEnumerator Attacked_EnterState()
    {

        nvAgent.Stop();
        anim.SetTrigger("Attacked");
        yield return new WaitForSeconds(0.5f);

        // 남은 체력 체크
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

        yield return null;
    }

    #endregion

    //*******************************************************************************

    #region 사망
    IEnumerator Dying_EnterState()
    {
        isDeath = true;
        anim.SetBool("Death", true);
        
        GetComponent<ItemDrop>().DropItem();    // 아이템 드랍 ( 아이템 드랍 설정은 인스펙터 창에서 )
        yield return new WaitForSeconds(2f);

        gameObject.SetActive(false);
        yield return null;
    }

    void OnEnable()
    {
        curState = EnemyStates.Idle;
        isDeath = false;
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
        if(!curState.Equals(EnemyStates.Dying) && !curState.Equals(EnemyStates.Attacked))
        {
            oldStates = curState;
            curState = EnemyStates.Attacked;
        }
    }

    // 업데이트 함수 ( 모든 상태일때 적용 할 Update )
    public override void ChildUpdate()
    {
        distance = Vector3.Distance(playerTr.position, transform.position);
        isPlayerLiftSide = playerTr.position.x < transform.position.x ? true : false;
    }

    // 데미지 줄때
    public void GiveDamage()
    {
        PlayerCtrl.instance.getDamage(10);
    }

    #endregion
}