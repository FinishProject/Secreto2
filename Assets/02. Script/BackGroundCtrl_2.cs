using UnityEngine;
using System.Collections;

public class BackGroundCtrl_2 : MonoBehaviour
{
    [System.Serializable]
    public struct BackGround
    {
        public GameObject bg_F;
        public GameObject bg_M;
        public GameObject bg_E;
    }
    public BackGround bg1, bg2;

    [System.Serializable]
    public struct BackGrounds_Speed
    {
        public float bg_F;
        public float bg_M;
        public float bg_E;
    }
    public BackGrounds_Speed bg_value;

    private struct Background_Info
    {
        public float bg_F_sizeX;
        public float bg_M_sizeX;
        public float bg_E_sizeX;

        public float bg_F_MovedX;
        public float bg_M_MovedX;
        public float bg_E_MovedX;

        public float bg_F_Space;
        public float bg_M_Space;
        public float bg_E_Space;
    }
    private Background_Info bg_info;

    private float zeroBaseX;
    private float CamPosZ;
    private Transform camTr;

    void BG_Info_init()
    {
        bg_info.bg_F_sizeX = bg1.bg_M.transform.localScale.x;
        bg_info.bg_M_sizeX = bg1.bg_M.transform.localScale.x;
        bg_info.bg_E_sizeX = bg1.bg_E.transform.localScale.x;

        bg_info.bg_F_Space = bg2.bg_F.transform.position.x - bg1.bg_F.transform.position.x;
        bg_info.bg_M_Space = bg2.bg_M.transform.position.x - bg1.bg_M.transform.position.x;
        bg_info.bg_E_Space = bg2.bg_E.transform.position.x - bg1.bg_E.transform.position.x;

        bg_info.bg_F_MovedX = 0.0f;
        bg_info.bg_M_MovedX = 0.0f;
        bg_info.bg_E_MovedX = 0.0f;
    }

    void Start()
    {
        zeroBaseX = GameObject.Find("Start_Save_Area").transform.position.x;
        camTr = GameObject.FindGameObjectWithTag("MainCamera").transform;
        CamPosZ = camTr.position.z;

        BG_Info_init();
    }

    void comparePos()
    {
        if (bg_info.bg_F_MovedX >= bg_info.bg_F_Space)
        {
            bg_info.bg_F_MovedX = 0;

            if (bg1.bg_F.transform.position.x < bg2.bg_F.transform.position.x)
                bg1.bg_F.transform.position = new Vector3(bg2.bg_F.transform.position.x + bg_info.bg_F_Space, bg2.bg_F.transform.position.y, bg2.bg_F.transform.position.z);
            else
                bg2.bg_F.transform.position = new Vector3(bg1.bg_F.transform.position.x + bg_info.bg_F_Space, bg1.bg_F.transform.position.y, bg1.bg_F.transform.position.z);
        }

        if (bg_info.bg_M_MovedX >= bg_info.bg_M_Space)
        {
            bg_info.bg_M_MovedX = 0;

            if (bg1.bg_M.transform.position.x < bg2.bg_M.transform.position.x)
                bg1.bg_M.transform.position = new Vector3(bg2.bg_M.transform.position.x + bg_info.bg_M_Space, bg2.bg_M.transform.position.y, bg2.bg_M.transform.position.z);
            else
                bg2.bg_M.transform.position = new Vector3(bg1.bg_M.transform.position.x + bg_info.bg_M_Space, bg1.bg_M.transform.position.y, bg1.bg_M.transform.position.z);
        }

        if (bg_info.bg_E_MovedX >= bg_info.bg_E_Space)
        {
            bg_info.bg_E_MovedX = 0;

            if (bg1.bg_E.transform.position.x < bg2.bg_E.transform.position.x)
                bg1.bg_E.transform.position = new Vector3(bg2.bg_E.transform.position.x + bg_info.bg_E_Space, bg2.bg_E.transform.position.y, bg2.bg_E.transform.position.z);
            else
                bg2.bg_E.transform.position = new Vector3(bg1.bg_E.transform.position.x + bg_info.bg_E_Space, bg1.bg_E.transform.position.y, bg1.bg_E.transform.position.z);
        }
    }

    void Update()
    {

        comparePos();

        MoveBg_X(bg1.bg_F, bg_value.bg_F, ref bg_info.bg_F_MovedX);
        MoveBg_X(bg2.bg_F, bg_value.bg_F);

        MoveBg_X(bg1.bg_M, bg_value.bg_M, ref bg_info.bg_M_MovedX);
        MoveBg_X(bg2.bg_M, bg_value.bg_M);

        MoveBg_X(bg1.bg_E, bg_value.bg_E, ref bg_info.bg_E_MovedX);
        MoveBg_X(bg2.bg_E, bg_value.bg_E);

    }

    Vector3 tempPos;
    void MoveBg_X(GameObject gameObj, float moveValue, ref float movedX)
    {
        float tempVal = 0;
        tempPos = gameObj.transform.position;

        tempVal = PlayerCtrl.controller.velocity.x * moveValue;
        tempPos.x -= tempVal;
        movedX -= tempVal * -1;
        gameObj.transform.position = tempPos;
    }

    void MoveBg_X(GameObject gameObj, float moveValue)
    {
        tempPos = gameObj.transform.position;
        tempPos.x -= PlayerCtrl.controller.velocity.x * moveValue;
        gameObj.transform.position = tempPos;
    }

}