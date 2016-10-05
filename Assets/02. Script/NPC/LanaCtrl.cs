using UnityEngine;
using System.Collections;

public class LanaCtrl : NpcMgr
{
    public float recognRange = 80f; // 인식 범위
    private bool isAppear = true;
    private bool isSpeakAnim = true;
    private int cnt = 0;

    public Transform movePoint;
    public Transform FocusPos;
    public Transform camPos;

    void Start()
    {
        Init();
    }

    // Update
    protected override void CurUpdate()
    {
        distance = GetDistance(this.transform.position);
        // 등장
        if (distance <= recognRange && isAppear)
        {
            AppearNpc();
        }
        // 대화 시작
        if (distance <= 30f && !isAppear)
        {
//            InGameUI.instance.CinematicView(true);
//            Camera.main.GetComponent<CameraCtrl_4>().SetCinematicView(true, camPos.position, FocusPos.position);
            anim.SetBool("Speak", isSpeakAnim);
            base.StartSpeak(this.name);

            // Dialogue 애니메이션 재생 후 더 이상 재생 못하도록 함
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dialogue"))
            {
                isSpeakAnim = false;
            }
        }
        // 대화 종료
        else
        {
//            InGameUI.instance.CinematicView(false);
//            Camera.main.GetComponent<CameraCtrl_4>().SetCinematicView(false, Vector3.zero, Vector3.zero);
            anim.SetBool("Speak", false);
        }
    }

    // 포물선을 그리며 플레이어 앞으로 등장
    void AppearNpc()
    {
        anim.SetBool("Appear", true);

        Vector3 center = (movePoint.position + this.transform.position) * 0.5f;
        center -= new Vector3(0, 1, 1);
        Vector3 fromRelCenter = this.transform.position - center;
        Vector3 toRelCenter = movePoint.position - center;
        transform.position = Vector3.Slerp(fromRelCenter, toRelCenter, 3f * Time.deltaTime);
        transform.position += center;

        Vector3 lookTarget = new Vector3(0f, playerTr.position.y, playerTr.position.z);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.name.Equals(movePoint.name))
        {
            anim.SetBool("Appear", false);
            isAppear = false;
        }
    }
}
