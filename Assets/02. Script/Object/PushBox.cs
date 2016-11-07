using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PushBox : MonoBehaviour {

    public float speed = 2f;
    public float uiPosY = 1f;
    private Vector3 moveDir = Vector3.zero;

    bool isRight = true;
    bool isActive = false;

    public bool isPush = true;

    public AudioClip clip;
    public AudioSource source;

    // PlayerCtrl 에서 플레이어가 밀때 실행
    public void PushObject(Transform playerTr, bool isFocusRight)
    {
        isRight = isFocusRight;
        if (!isActive)
        {
            isActive = true;
            StartCoroutine(Pushing(playerTr));
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

            if (isPush)
            {
                // 플레이어 정면으로 밀린다.
                moveDir = playerTr.forward;
                transform.position += moveDir * speed * Time.deltaTime;

                // 사운드 재생
                if (!source.isPlaying)
                    source.PlayOneShot(clip);
            }

            yield return null;
        }
        source.Stop();
        //SoundMgr.instance.StopAudio("rock_push");
        PlayerCtrl.instance.SetPushAnim(false);
        ShowUI.instanace.OnImage(true);
        ShowUI.instanace.SetPosition(this.transform, uiPosY);
        isActive = false;
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
}
