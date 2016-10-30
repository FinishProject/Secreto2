using UnityEngine;
using System.Collections;

public class ChaseSphere : MonoBehaviour {

    public GameObject laser;

    private Transform playerTr;

    void Start()
    {
        playerTr = PlayerCtrl.instance.transform;
    }

    void Update()
    {
        //Vector3 look = (playerTr.position - transform.position);
        //float rotationZ = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg;
        //Debug.Log(rotationZ);
        //transform.rotation = Quaternion.Euler(0.0f, 0.0f, -rotationZ);

        //transform.lookat(new vector3(transform.position.x, transform.position.y, playertr.position.z));

        Quaternion newRotation = Quaternion.LookRotation(playerTr.position - laser.transform.position, Vector3.forward);
        newRotation.x = 0f;
        newRotation.y = 0f;

        laser.transform.rotation = Quaternion.Slerp(laser.transform.rotation, newRotation, 5f * Time.deltaTime);
    }
}
