using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PushBox : MonoBehaviour {

    public float speed = 2f;
    public float uiPosY = 1f;
    private Vector3 moveDir = Vector3.zero;

    bool isRight = true;
    bool isActive = false;

    public AudioClip clip;
    public AudioSource source;

    // PlayerCtrl 에서 플레이어가 밀때 실행
    public void PushObject(Transform playerTr, bool isPlayerRight)
    {
        isRight = isPlayerRight;
        // 플레이어 정면으로 밀린다.
        moveDir = playerTr.forward;
        transform.position += moveDir * speed * Time.deltaTime;

        if (!isActive)
        {
            isActive = true;
            StartCoroutine(Pushing(playerTr));
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isActive)
        {
            ShowUI.instanace.OnImage(true);
            ShowUI.instanace.SetPosition(this.transform, uiPosY);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            ShowUI.instanace.OnImage(false);
        }
    }
    IEnumerator Pushing(Transform playerTr)
    {
        PlayerCtrl.instance.SetPushAnim(true);
        ShowUI.instanace.OnImage(false);

        while (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            // Shitf 키를 누르지 않거나, 플레이어 위치가 박스보다 높으면 밀기 종료
            if (Input.GetKeyUp(KeyCode.LeftShift) || transform.position.y < playerTr.position.y
                || isRight != PlayerCtrl.isFocusRight)
            {
                break;
            }

            // 사운드 재생
            if (!source.isPlaying)
                source.PlayOneShot(clip);

            yield return null;
        }
        source.Stop();
        //SoundMgr.instance.StopAudio("rock_push");
        ShowUI.instanace.OnImage(true);
        PlayerCtrl.instance.SetPushAnim(false);
        ShowUI.instanace.SetPosition(this.transform, uiPosY);
        isActive = false;
    }

    // 밀기
    IEnumerator Push(Transform playerTr)
    {
        PlayerCtrl.instance.SetPushAnim(true);

        ShowUI.instanace.OnImage(false);
        
        // 이동 방향키 누르고 있을 동안
        while (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            // Shitf 키를 누르지 않거나, 플레이어 위치가 박스보다 높으면 밀기 종료
            if (Input.GetKeyUp(KeyCode.LeftShift) || transform.position.y < playerTr.position.y 
                || isRight != PlayerCtrl.isFocusRight)
            {
                break;
            }

            // 플레이어 정면으로 밀린다.
            moveDir = playerTr.forward;
            transform.position += moveDir * speed * Time.deltaTime;

            // 사운드 재생
            SoundMgr.instance.PlayAudio("rock_push",false, 1f);

            yield return null;
        }

        // 사운드, 애니메이션 종료
        SoundMgr.instance.StopAudio("rock_push");
        PlayerCtrl.instance.SetPushAnim(false);

        ShowUI.instanace.OnImage(true);
        ShowUI.instanace.SetPosition(this.transform, uiPosY);
        isActive = false;
    }
}
