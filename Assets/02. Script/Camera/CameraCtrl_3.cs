using UnityEngine;
using System.Collections;

public class CameraCtrl_3 : MonoBehaviour {
    Vector3 ori;
    float y, z;
    Transform playerTr;
	// Use this for initialization
	void Start () {
        ori = transform.position;
        playerTr = PlayerCtrl.instance.transform;
        y = transform.position.y - playerTr.position.y;
        z = transform.position.z - playerTr.position.z;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 temp = new Vector3(0, y, z);
        transform.position = Vector3.Lerp(transform.position, playerTr.position + temp,  2.5f * Time.deltaTime);

        if(PlayerCtrl.controller.velocity.y > -8)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTr.position + temp, 10f * Time.deltaTime);
        }
    }
}
