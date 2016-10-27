using UnityEngine;
using System.Collections;
using System;

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
    public static bool dying;      // 죽는중

    public static float inputAxis = 0f;     // 입력 받는 키의 값
    public static bool isFocusRight = true; // 우측을 봐라보는 여부

    [System.NonSerialized]
    public bool isMove = true;       // 현재 이동 여부
    [System.NonSerialized]
    public bool isJumping = false;   // 현재 점프중인지 확인

    public static float focusRight = 1f;
    private float lockPosZ = 0f;

    public static Vector3 moveDir = Vector3.zero; // 이동 벡터
    public static CharacterController controller; // 캐릭터컨트롤러
    private Animator anim;

    public GameObject lunaModel;
    public GameObject clothModel;
    private PlayerEffect pEffect;
    private WahleMove wahleMove;
    public AudioClip runSound;
    private AudioSource audioSource;

    private AnimatorStateInfo currentAnim;
    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int runState = Animator.StringToHash("Base Layer.Run");
    static int jumpDownState = Animator.StringToHash("Base Layer.Jump_Down");
    static int JumpUpState = Animator.StringToHash("Base Layer.Jump_Up(5~25)");

    public static PlayerCtrl instance;

    void Awake()
    {
        instance = this;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        pEffect = GetComponent<PlayerEffect>();
        wahleMove = GameObject.FindGameObjectWithTag("WAHLE").GetComponent<WahleMove>();
    }

    void Start()
    {
        //GetPlayerData();
        curGravity = dropGravity;
        lockPosZ = transform.position.z;
    }


    void FixedUpdate()
    {
        currentAnim = anim.GetCurrentAnimatorStateInfo(0);
        SetAnimator();
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, lockPosZ);
        
        // 플레이어에게 조작권한이 있다면 움직임
        if (isMove) Movement();
        else if (!isMove)
        {
            moveDir.x = 0f;
            moveDir.y -= curGravity * Time.deltaTime;
            controller.Move(moveDir * moveSpeed * Time.deltaTime);
        }

        //캐릭터 방향 회전
        // 왼쪽 회전
        if (inputAxis < 0 && isFocusRight) { TurnPlayer(); }
        // 오른쪽 회전
        else if (inputAxis > 0 && !isFocusRight) { TurnPlayer(); }
    }

    void SetAnimator()
    {
        if (controller.isGrounded && isMove)
        {
            anim.SetBool("Jump", false);
            anim.SetBool("Dash", false);
            anim.SetBool("Fall", false);

            // 키 입력 시 달리기 애니메이션 재생
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                anim.SetBool("Run", true);

                if (!audioSource.isPlaying)
                    audioSource.PlayOneShot(runSound);
            }
            else
            {
                anim.SetBool("Run", false);
                audioSource.Stop();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetBool("Jump", true);
            }
        }
        else if (!controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetBool("Dash", true);
            }

            else if(controller.velocity.y <= -0.01 && currentAnim.nameHash.Equals(runState))
            {
                anim.SetBool("Fall", true);
            }
        }
    }

    void Movement()
    {
        inputAxis = Input.GetAxis("Horizontal"); // 키 입력

        // 좌우 동시 입력을 막기위함
        if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow) ||
            Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            inputAxis = 0f;
            anim.SetBool("Run", false);
        }

        // 지상에 있을 시
        if (controller.isGrounded)
        {
            curGravity = 50f;
            //이동
            moveDir = Vector3.right * inputAxis;
            // 점프
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartBasicJump();
            }
        }
        // 공중에 있을 시
        else if (!controller.isGrounded)
        {
            moveDir.x = inputAxis;

            if (Input.GetKeyDown(KeyCode.Space) && isJumping)
                StartDashJump();

            if (controller.velocity.y <= -0.01)
                curGravity = dropGravity;
        }

        moveDir.y -= curGravity * Time.deltaTime;
        controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    void StartBasicJump()
    {
        curGravity = upGravity;
        isJumping = true;
        anim.SetBool("Jump", true);
        pEffect.StartEffect(PlayerEffectList.BASIC_JUMP);

        moveDir.y += basicJumpHight;
    }

    void StartDashJump()
    {
        curGravity = upGravity;
        isJumping = false;
        anim.SetBool("Dash", true);
        
        moveDir.y = dashJumpHight;
    }

    //캐릭터 방향 회전
    public void TurnPlayer()
    {
        isFocusRight = !isFocusRight;
        focusRight *= -1f;

        transform.Rotate(new Vector3(0, 1, 0), 180);

        wahleMove.ResetSpeed();
    }


    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("DeadLine"))
        {
            PlayerDie();
        }
        else if (coll.CompareTag("StartPoint"))
        {
            Save();
        }
        else if (coll.CompareTag("Hold"))
        {
            WahleCtrl.curState = WahleCtrl.instance.StepHold();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Hold"))
        {
            WahleCtrl.instance.transform.parent = this.transform.parent;
            WahleCtrl.instance.ChangeState(WahleState.MOVE);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("OBJECT"))
        {
            if (Input.GetKey(KeyCode.LeftShift) && transform.position.y < hit.transform.position.y)
            {
                if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    hit.gameObject.GetComponent<PushBox>().PushObject(this.transform, isFocusRight);
            }
        }
    }

    public void PlayerDie()
    {
        StartCoroutine(ResetPlayer());
    }

    IEnumerator ResetPlayer()
    {
        dying = true;
        GetComponent<CharacterController>().enabled = false;
        FadeInOut.instance.StartFadeInOut(1, 2, 3);
        isMove = false;

        lunaModel.SetActive(false);
        clothModel.SetActive(false);
        pEffect.StartEffect(PlayerEffectList.DIE);

        yield return new WaitForSeconds(1.3f);
        GetComponent<CharacterController>().enabled = true;
        //ObjectPosResetMgr.instance.ResetPos();

        GetPlayerData();
        lunaModel.SetActive(true);
        clothModel.SetActive(true);

        isMove = true;

        yield return new WaitForSeconds(1f);
        dying = false;
    }

    void GetPlayerData()
    {
        Data pData = new Data(); // 플레이어 데이터 저장을 위한 클래스 변수
        pData = DataSaveLoad.Load();
        if (pData != null)
            transform.position = pData.pPosition;
        else
        {
            Save();
            pData = DataSaveLoad.Load();
            transform.position = pData.pPosition;
        }
    }
    public void animReset()
    {
        anim.SetBool("Run", false);
        anim.SetBool("Jump", false);
        anim.SetBool("Dash", false);
    }

    void OnEnable()
    {
        WayPoint.OnSave += Save;
    }

    //플레이어 데이터 저장
    void Save()
    {
        Data pData = new Data();
        pData.pPosition = transform.position;
        DataSaveLoad.Save(pData);
    }


    // 2단 점프 끝났을 때 실행
    void SetEndAnim()
    {
        anim.SetBool("Dash", false);
    }

    public void SetStopMove()
    {
        isMove = false;
        moveDir.x = 0f;
        anim.SetBool("Idle", true);
    }

    public void SetPushAnim(bool isPush)
    {
        anim.SetBool("Push", isPush);
    }

    public int GetPlayingAnimation()
    {
        return anim.GetCurrentAnimatorStateInfo(0).nameHash;
    }
}