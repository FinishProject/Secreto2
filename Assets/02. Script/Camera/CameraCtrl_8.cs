using UnityEngine;
using System.Collections;
/********************************* 사용 방법 *************************************

    







**********************************************************************************/
public class CameraCtrl_8 : MonoBehaviour {

    struct myRect
    {
        public float left;
        public float right;
        public float top;
        public float buttom;

        public float half_width;
        public float half_height;
    }
    myRect boxRect, playerRect;

    public Transform camParent_Tr;
    public Transform boxes_Tr;
    public Transform box_Up_Tr;
    public Transform box_Down_Tr;
    public Transform box_Player_Tr;

    Vector3 addPos_playerbox;
    Transform tr;
    Transform playerTr;
    // Use this for initialization
    void Start () {
        playerTr = PlayerCtrl.instance.transform;
        tr = transform;
        BoxInit();

    }

    void BoxInit()
    {
        boxRect.half_width  = (box_Up_Tr.lossyScale.x * 0.5f);
        boxRect.half_height = (box_Up_Tr.lossyScale.y * 0.5f) + (box_Down_Tr.lossyScale.y * 0.5f);

        playerRect.half_width  = (box_Player_Tr.lossyScale.x * 0.5f);
        playerRect.half_height = (box_Player_Tr.lossyScale.y * 0.5f);

        addPos_playerbox = box_Player_Tr.position - playerTr.position;
        addPos_playerbox.z = 0;
    }

    // Update is called once per frame
    void Update ()
    {
        box_Player_Tr.position = playerTr.position + addPos_playerbox;
        RectUpdate();
    }

    void LateUpdate()
    {
        LineChecker();
    }



    RaycastHit hit;
    void RectUpdate()
    {
        boxRect.left   = box_Up_Tr.position.x - boxRect.half_width;
        boxRect.right  = box_Up_Tr.position.x + boxRect.half_width;
        boxRect.top    = boxes_Tr.position.y  + boxRect.half_height;
        boxRect.buttom = boxes_Tr.position.y  - boxRect.half_height;

        playerRect.left   = box_Player_Tr.position.x - playerRect.half_width;
        playerRect.right  = box_Player_Tr.position.x + playerRect.half_width;
        playerRect.top    = box_Player_Tr.position.y + playerRect.half_height;
        playerRect.buttom = box_Player_Tr.position.y - playerRect.half_height;

        
        if(Physics.Raycast(playerTr.position, -Vector3.up, out hit,  0.1f))
        {
            tempPos = camParent_Tr.position;
            tempPos.y -= (box_Up_Tr.position.y - (box_Up_Tr.lossyScale.y * 0.5f)) - playerRect.buttom;
            camParent_Tr.position = Vector3.Lerp(camParent_Tr.position, tempPos, 10 * Time.smoothDeltaTime);

        }
    }
    Vector3 tempPos;
    void LineChecker()
    {
        // 플레이어가 좌측선을 넘었을 때
        if (playerRect.left < boxRect.left)
        {
            tempPos = camParent_Tr.position;
            tempPos.x -= boxRect.left - playerRect.left;
            camParent_Tr.position = Vector3.Lerp(camParent_Tr.position, tempPos, 10 * Time.smoothDeltaTime); 
        }
        // 플레이어가 오른쪽선을 넘었을 때
        else if (playerRect.right > boxRect.right)
        {
            tempPos = camParent_Tr.position;
            tempPos.x += playerRect.right - boxRect.right;
            camParent_Tr.position = Vector3.Lerp(camParent_Tr.position, tempPos, 10 * Time.smoothDeltaTime);
        }

        
        // 플레이어가 상단선을 넘었을 때
        if (playerRect.top > boxRect.top)
        {
            
            tempPos = camParent_Tr.position;
            tempPos.y += playerRect.top - boxRect.top;
            camParent_Tr.position = Vector3.Lerp(camParent_Tr.position, tempPos, 10 * Time.smoothDeltaTime);
        }
        // 플레이어가 하단선을 넘었을 때
        else if (playerRect.buttom < boxRect.buttom)
        {
            tempPos = camParent_Tr.position;
            tempPos.y -= boxRect.buttom - playerRect.buttom;
            camParent_Tr.position = Vector3.Lerp(camParent_Tr.position, tempPos, 10 * Time.smoothDeltaTime);
        }
        
    }

}
