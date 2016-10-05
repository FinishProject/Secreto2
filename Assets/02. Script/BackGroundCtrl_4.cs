using UnityEngine;
using System.Collections;

public class BackGroundCtrl_4 : MonoBehaviour
{
    [System.Serializable]
    public struct BackGround
    {
        public Material bg_F;
        public Material bg_M;
        public Material bg_E;
        public Material bg_Sky;
    }
    public BackGround bg;

    [System.Serializable]
    public struct BackGrounds_Speed
    {
        public float bg_F;
        public float bg_M;
        public float bg_E;
        public float bg_Sky;
    }
    public BackGrounds_Speed bg_value;

    Vector3 addPos, tempPos;
    Transform tr, playerTr;
    void Start()
    {
        tr = transform;
        playerTr = PlayerCtrl.instance.transform;
        addPos = tr.position - playerTr.position;

    }
    void Update()
    {
        tempPos = playerTr.position + addPos;
        tempPos.y = transform.position.y;
        transform.position = tempPos;

        MoveBg_X(bg.bg_F, bg_value.bg_F * 0.1f);
        MoveBg_X(bg.bg_M, bg_value.bg_M * 0.1f);
        MoveBg_X(bg.bg_E, bg_value.bg_E * 0.1f);
        MoveBg_X(bg.bg_Sky, bg_value.bg_Sky * 0.1f);
    }

    Vector2 bg_Offset;
    void MoveBg_X(Material material, float moveValue)
    {
        bg_Offset = material.GetTextureOffset("_MainTex");
        if (PlayerCtrl.instance.isMove && Mathf.Abs(PlayerCtrl.controller.velocity.x) > 0)
            bg_Offset.x += PlayerCtrl.controller.velocity.x * moveValue;
        material.SetTextureOffset("_MainTex", bg_Offset);
    }

}