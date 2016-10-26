using UnityEngine;
using System.Collections;

public class PushGidoong : MonoBehaviour, Sensorable_Player
{ 
    public GameObject pusher;
    public GameObject childObj;
    Rigidbody pushersRigidbody;
    int passCount;
    public bool pushFirstTime;
	// Use this for initialization
	void Start () {
        pushersRigidbody = pusher.GetComponent<Rigidbody>();
        passCount = 0;
    }

    public bool ActiveSensor_Player(int index)
    {
        passCount++;
        if(pushFirstTime || passCount >= 2)
        {
            pushersRigidbody.AddForce(-Vector3.forward * 3000);
            StartCoroutine(RemoveTag());
        }
        return true;
    }

    IEnumerator RemoveTag()
    {
        yield return new WaitForSeconds(2f);
        pusher.SetActive(false);
        childObj.tag = "Untagged";
    }
}
