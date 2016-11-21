using UnityEngine;
using System.Collections;
//using System;

public enum PlayerEffectList
{
    DIE, BASIC_JUMP, DASH_JUMP,
}

public class PlayerCtrl : MonoBehaviour
{
    // 플레이어 이동 변수
    public float moveSpeed = 10f; // 이동 속도
    public float basicJumpHight = 3.0f; // 기본 점프 높이
    public float dashJumpHight = 4.0f; // 대쉬 점프 높이
    public float upGravity = 1f; // 점프 시 중력 값
    public float dropGravity = 5f; // 공중에 있을 때의 중력값
    public static float curGravity; // 현재 중력값

    public static float inputAxis = 0f;     // 입력 받는 키의 값
    public static bool isFocusRight = true; // 우측을 봐라보는 여부

    public static bool dying;      // 죽는중
    private bool isMove = true;       // 현재 이동 여부
    private bool isJumping = false;   // 현재 점프중인지 확인

    public static float focusRight = -1f;
    private float lockPosZ = 0f;

    public static Vector3 moveDir = Vector3.zero; // 이동 벡터
    public static CharacterController controller; // 캐릭터컨트롤러
    private Animator anim;

    private Vector3 saveLocation;

    public PlayerEffect pEffect;
    private WahleMove wahleMove;

    public AudioClip[] soundClips;
    private AudioSource source;

    private AnimatorStateInfo currentAnim;
    static int fallState = Animator.StringToHash("Base Layer.Jump_DownLoop");
    static int landJump = Animator.StringToHash("Base Layer.Jump_Land(43~50)");
    static int dashJump = Animator.StringToHash("Base Layer.DoubleJump_Up(25~50)");

    private ScoreUI scroreUI;

    public static PlayerCtrl instance;

    void Awake()
    {
        instance = this;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        wahleMove = GameObject.FindGameObjectWithTag("WAHLE").GetComponent<WahleMove>();

    }

    void Start()
    {
        //GetPlayerData();
        curGravity = dropGravity;
        lockPosZ = transform.position.z;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, lockPosZ);
        currentAnim = anim.GetCurrentAnimatorStateInfo(0);
        // 플레이어에게 조작권한이 있다면 움직임
        if (isMove)
        {
            Movement();
            //캐릭터 방향 회전
            // 왼쪽 회전
            if (inputAxis < 0 && isFocusRight) { TurnPlayer(); }
            // 오른쪽 회전
            else if (inputAxis > 0 && !isFocusRight) { TurnPlayer(); }
        }
        else if (!isMove)
        {
            moveDir.x = 0f;
            moveDir.y -= curGravity * Time.deltaTime;
            controller.Move(moveDir * moveSpeed * Time.deltaTime);
        }

        if (controller.velocity.y <= -50f)
            Die();
    }

    void Movement()
    {
        inputAxis = Input.GetAxis("Horizontal"); // 키 입력

        // 지상에 있을 시
        if (controller.isGrounded)
        {
            anim.SetBool("Jump", false);
            anim.SetBool("Dash", false);
            anim.SetBool("Fall", false);

            isJumping = true;
            curGravity = 35f;

            // 좌우 동시 입력을 막기위함
            if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow) ||
                Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
            {
                inputAxis = 0f;
                anim.SetBool("Run", false);
            }
            // 달리기 중
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                anim.SetBool("Run", true);

                if (!source.isPlaying)
                {
                    source.PlayOneShot(soundClips[3]);
                }
            }

            // 달리기 멈춤
            else
            {
                anim.SetBool("Run", false);
                source.Stop();
            }

            //이동
            moveDir = Vector3.right * inputAxis;
            // 점프
            if (Input.GetKeyDown(KeyCode.Space))
                StartBasicJump();
        }
        // 공중에 있을 시
        else if (!controller.isGrounded)
        {
            moveDir.x = inputAxis;

            if (Input.GetKeyDown(KeyCode.Space) && isJumping)
                StartDashJump();

            if (controller.velocity.y <= -0.01)
                curGravity = dropGravity;

            if (!controller.isGrounded && dashJump != currentAnim.nameHash && controller.velocity.y < -13f
            && !dying)
            {
                // 추락 애니메이션
                anim.SetBool("Fall", true);
            }
        }

        moveDir.y -= curGravity * Time.deltaTime;
        controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    void StartBasicJump()
    {
        if (source.isPlaying)
            source.Stop();
        source.PlayOneShot(soundClips[1]);
        curGravity = upGravity;
        moveDir.y += basicJumpHight;

        anim.SetBool("Jump", true);
        // 기본 점프 이펙트
        pEffect.StartEffect(PlayerEffectList.BASIC_JUMP);
    }

    void StartDashJump()
    {
        curGravity = upGravity;
        isJumping = false;

        moveDir.y = dashJumpHight;

        if (source.isPlaying)
            source.Stop();

        anim.SetBool("Dash", true);
        source.PlayOneShot(soundClips[1]);

    }

    //캐릭터 방향 회전
    public void TurnPlayer()
    {
        Quaternion localRot = transform.rotation;
        if (isFocusRight)
            localRot.w = -0.7f;
        else 
            localRot.w = 0.7f;
        transform.rotation = localRot;

        isFocusRight = !isFocusRight;
        focusRight *= -1f;
        wahleMove.ResetSpeed();
    }


    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("DeadLine") && !dying)
        {
            dying = true;
            Die();
        }
        else if (coll.CompareTag("StartPoint"))
            Save();
        else if (coll.CompareTag("Hold"))
            WahleCtrl.curState = WahleCtrl.instance.StepHold();
        else if (coll.CompareTag("Hold2"))
            WahleCtrl.curState = WahleCtrl.instance.StepHold2();
        
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Hold") || col.CompareTag("Hold2"))
        {
            WahleCtrl.instance.ChangeState(WahleState.MOVE);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("OBJECT"))
        {
            if (Input.GetKey(KeyCode.LeftShift) && transform.position.y < hit.transform.position.y)
            {
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                        hit.gameObject.GetComponent<PushBox>().PushObject(this.transform, isFocusRight);
                }
            }
        }
    }

    public void Die()
    {
        pEffect.StartEffect(PlayerEffectList.DIE);
        GameManager.instance.SetPlayerDie();
    }

    public void PlaySound()
    {
        source.PlayOneShot(soundClips[2]);
    }

    public void ResetAnim()
    {
        anim.Play("Idle", 0);
        anim.SetBool("Run", false);
        anim.SetBool("Jump", false);
        anim.SetBool("Dash", false);
        anim.SetBool("Fall", false);
    }

    // 플레이어 데이터 저장
    public void Save()
    {
        saveLocation = this.transform.position;
    }
    // 플레이어 위치값 가져오기
    public void GetPlayerData()
    {
        this.transform.position = saveLocation;
    }
    // Push 애니메이션 세팅
    public void SetPushAnim(bool isPush)
    {
        anim.SetBool("Push", isPush);
    }
    
    public void SetStopMove(bool isStopMove)
    {
        isMove = isStopMove;
    }

    public IEnumerator SetStopMoveDuration(float time)
    {
        isMove = false;
        yield return new WaitForSeconds(time);
        isMove = true;
    }
}