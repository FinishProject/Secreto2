using UnityEngine;
using System.Collections;

public class RollDownLog : MonoBehaviour {

    float startShakeRange = 20;
    Vector3 orignPos;
    Quaternion orignRot;
    new Rigidbody rigidbody;

    bool isMoving;
    bool isShaking;
    void Start()
    {
//        orignPos = transform.position;
//        orignRot = transform.rotation;
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {

        isMoving = true;


        if(isMoving)
        {
            rigidbody.AddForce(Vector3.right * 1500 , ForceMode.Force);
            if (!isShaking && Vector3.Distance(transform.position, PlayerCtrl.instance.transform.position) < startShakeRange)
            {
                Debug.Log(3);
                isShaking = true;
                CameraCtrl_6.instance.ShakeStart(transform, startShakeRange + 1);
            }
        }
    } 

    
    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("DeadLine"))
        {
            transform.position = orignPos;
            transform.rotation = orignRot;
            gameObject.SetActive(false);

            //rigidbody.velocity = Vector3.zero;
            //rigidbody.angularVelocity = Vector3.zero;

        }

        if(col.CompareTag("Player"))
        {
            PlayerCtrl.instance.PlayerDie();
        }
    }

   

}
