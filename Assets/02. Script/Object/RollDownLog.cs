﻿using UnityEngine;
using System.Collections;

public class RollDownLog : MonoBehaviour {

    public float LogSpeed = 1500;
    public bool isStart = false;
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
        isShaking = false;

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

    public IEnumerator loopRollDown()
    {
        while(true)
        {
            StartRollDownLog();
            yield return null;

        }
    }

    void Update()
    {
        if(isMoving)
        {
            
            rigidbody.AddForce(Vector3.right * LogSpeed, ForceMode.Force);
            if (!isShaking && Vector3.Distance(transform.position, PlayerCtrl.instance.transform.position) < startShakeRange)
            {
                isShaking = true;
                CameraCtrl_1.instance.ShakeStart(transform, startShakeRange + 1);
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
            StartCoroutine(PlayerCtrl.instance.PlayerDie());
        }
    }

   

}
