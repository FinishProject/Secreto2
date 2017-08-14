using UnityEngine;
using System.Collections;

public class BaseSphere : MonoBehaviour {

    // 구체의 상태
    public enum ChaseState
    {
        IDLE,
        CHARGE,
        SHOT,
    }

    protected ChaseState state = ChaseState.IDLE;

    public Transform[] stones; // 각 돌들의 위치 배열
    protected Vector3[] originPos; // 기존 위치를 저장할 배열
    protected Vector3[] fillPos; // 벌어질 위치를 저장할 배열
    public Vector3[] setFillPositions;

    protected Transform playerTr; // 플레이어의 위치
    protected AudioSource audioSoruce;

    // 스피드
    public float shootingSpeed = 100.0f; // 공격 속도
    public float[] blockSpeed; // 각 돌들의 움직임 속도
    public float reloadTime = 5.0f; // 재장전 속도

    public float shakeAmount = 0.1f; // 흔들릴 강도
    protected float moveDir = 1.0f;
    protected bool isShot = false; // 공격 여부

    protected virtual void Start()
    {
        audioSoruce = GetComponent<AudioSource>();
        playerTr = PlayerCtrl.instance.transform;
    }

    /* 각 돌들의 초기 위치 저장 */
    protected virtual Vector3[] SetBlockOriginPosition(Transform[] blockOriginPos)
    {
        Vector3[] originPosition = new Vector3[blockOriginPos.Length];

        for (int i = 0; i < originPosition.Length; ++i)
        {
            originPosition[i] = blockOriginPos[i].position;
        }

        return originPosition;
    }

    protected virtual Vector3[] SetBlockFillPosition(Transform[] stonesPos, Vector3[] blockFillPos)
    {
        Vector3[] setFillPos = new Vector3[blockFillPos.Length];
         
        for(int i = 0; i < setFillPos.Length; ++i)
        {
            setFillPos[i] = new Vector3(stones[i].position.x + blockFillPos[i].x,
                stones[i].position.y + blockFillPos[i].y, stones[i].position.z + blockFillPos[i].z);
        }

        return setFillPos;
    }

    protected virtual IEnumerator UpdateSphere()
    {
        // 상속받은 자식 클래스에서 정의
        yield return null;
    }

    protected virtual IEnumerator Idle()
    {
        // 상속받은 자식 클래스에서 정의
        yield return null;
    }

    protected virtual IEnumerator Charge()
    {
        // 상속받은 자식 클래스에서 정의
        yield return null;
    }

    protected virtual IEnumerator Fire()
    {
        // 상속받은 자식 클래스에서 정의
        yield return null;
    }

    protected virtual IEnumerator Fade(float fadeDir, float alpha)
    {
        // 상속받은 자식 클래스에서 정의
        yield return null;
    }
}
