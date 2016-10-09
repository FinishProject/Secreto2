using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PushBox : MonoBehaviour {

    public float speed = 1f;
    public float uiPosY = 1f;
    private Vector3 moveDir = Vector3.zero;

    bool isRight = true;
    bool isActive = false;

    public void PushObject(Transform playerTr, bool isDir)
    {
        isRight = isDir;
        if(!isActive)
            StartCoroutine(Push(playerTr));
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
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

    IEnumerator Push(Transform playerTr)
    {
        PlayerCtrl.instance.SetPushAnim(true);
        ShowUI.instanace.OnImage(false);
        isActive = true;

        while (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKeyUp(KeyCode.LeftShift) || PlayerCtrl.inputAxis == 0f
                || transform.position.y < playerTr.position.y || isRight != PlayerCtrl.isFocusRight)
            {
                break;
            }

            SoundMgr.instance.PushObject(true);

            moveDir = playerTr.forward;
            transform.position += moveDir * speed * Time.deltaTime;

            yield return null;
        }
        SoundMgr.instance.PushObject(false);
        PlayerCtrl.instance.SetPushAnim(false);
        ShowUI.instanace.OnImage(true);
        ShowUI.instanace.SetPosition(this.transform, uiPosY);
        isActive = false;
    }
}
