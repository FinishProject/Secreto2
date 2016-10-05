using UnityEngine;
using System.Collections;

public class WindObj : MonoBehaviour {

    public float speed = 3f;
    private Vector3 moveDir;

    private bool isFly = false;

    CameraCtrl_5 cam;

    void Start()
    {
        cam = GetComponent<CameraCtrl_5>();
    }

	void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
            if(cam != null)
                cam.ChangeCamSpeed(500f);

            PlayerCtrl.curGravity = 0f;
            col.transform.position += Vector3.up * speed * Time.deltaTime;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (cam != null)
                cam.ResetCameSpeed();
            PlayerCtrl.curGravity = PlayerCtrl.instance.dropGravity;
        }
    }
}
