using UnityEngine;
using System.Collections;

public class PushGidoong : MonoBehaviour, Sensorable_Player
{ 
    public GameObject pusher;
    Rigidbody pushersRigidbody;

	// Use this for initialization
	void Start () {
        pushersRigidbody = pusher.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {

    }

    public bool ActiveSensor_Player(int index)
    {
        Debug.Log(1);
        pushersRigidbody.AddForce(-Vector3.forward * 3000);
        return true;
    }
}
