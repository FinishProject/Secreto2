using UnityEngine;
using System.Collections;

public class RollDownLog : MonoBehaviour {

    public float LogSpeed = 1500;
    MeshRenderer LogMesh;
    float startShakeRange = 20;
    Vector3 orignPos;
    Quaternion orignRot;
    new Rigidbody rigidbody;    
    CapsuleCollider collider;

    bool isMoving;
    bool isShaking;
    void Awake()
    {
        orignPos = transform.position;
        orignRot = transform.rotation;

        collider = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        LogMesh = GetComponentInChildren<MeshRenderer>();
        SetLog();
    }

    void SetLog()
    {
        rigidbody.isKinematic = true;
        isMoving = false;
        LogMesh.enabled = false;
        collider.enabled = false;

        transform.position = orignPos;
        transform.rotation = orignRot;

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    public void StartRollDownLog()
    {
       rigidbody.isKinematic = false;
       isMoving = true;
       LogMesh.enabled = true;
       collider.enabled = true;
        
    }

    void Update()
    {
        if(isMoving)
        {
            rigidbody.AddForce(Vector3.right * LogSpeed, ForceMode.Force);
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

           
            SetLog();

        }

        if(isMoving && col.CompareTag("Player"))
        {
            PlayerCtrl.instance.PlayerDie();
            SetLog();
        }
    }

   

}
