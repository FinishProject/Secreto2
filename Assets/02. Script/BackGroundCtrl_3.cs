using UnityEngine;
using System.Collections;

public class BackGroundCtrl_3 : MonoBehaviour {

    [System.Serializable]
    public struct BackGround
    {
        public GameObject bg_F;
        public GameObject bg_M;
        public GameObject bg_E;
    }
    public BackGround bg;

    [System.Serializable]
    public struct BackGrounds_Speed
    {
        public float bg_F;
        public float bg_M;
        public float bg_E;
    }
    public BackGrounds_Speed bg_value;


    void Update()
    {
        MoveBg_X(bg.bg_F, bg_value.bg_F);
        MoveBg_X(bg.bg_M, bg_value.bg_M);
        MoveBg_X(bg.bg_E, bg_value.bg_E);

    }

    Vector3 tempPos;

    void MoveBg_X(GameObject gameObj, float moveValue)
    {
        tempPos = gameObj.transform.position;
        if(PlayerCtrl.instance.isMove && Mathf.Abs(PlayerCtrl.controller.velocity.x) > 0)
            tempPos.x -= PlayerCtrl.controller.velocity.x * moveValue;
        gameObj.transform.position = tempPos;
    }
}
