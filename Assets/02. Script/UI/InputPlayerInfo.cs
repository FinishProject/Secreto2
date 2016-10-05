using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System;


public class InputPlayerInfo : MonoBehaviour {

    public InputField jump1;
    public InputField jump2;
    public InputField gr;
    public InputField speed;
    public InputField speed_c;
    public GameObject camera;

    public Button botton;
    // Use this for initialization
    void Start () {
        jump1.text = PlayerCtrl.instance.basicJumpHight.ToString();
        jump2.text = PlayerCtrl.instance.dashJumpHight.ToString();
        //gr.   text = PlayerCtrl.instance.gravity_jump.ToString();
        speed.text = PlayerCtrl.instance.moveSpeed.ToString();
        //speed_c.text = camera.GetComponent<CameraCtrl>().speed.ToString();
    }


    public void s()
    {
        PlayerCtrl.instance.basicJumpHight = Convert.ToSingle(jump1.text);
        PlayerCtrl.instance.dashJumpHight = Convert.ToSingle(jump2.text);
        //PlayerCtrl.instance.gravity_jump = Convert.ToSingle(gr.text);
        PlayerCtrl.instance.moveSpeed = Convert.ToSingle(speed.text);
        //camera.GetComponent<CameraCtrl>().speed = Convert.ToSingle(speed_c.text);
        CameraCtrl.speed = Convert.ToSingle(speed_c.text);

    }
}
