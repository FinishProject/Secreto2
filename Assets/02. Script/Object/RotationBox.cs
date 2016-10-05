using UnityEngine;
using System.Collections;

public class RotationBox : MonoBehaviour {

    public float setTime = 0f;
    Quaternion rndValue;
    bool isRnd = false;

    void Start()
    {
        rndValue = Random.rotation;
    }

	void Update () {
        if(!isRnd)
            StartCoroutine(GetRndRotation());

        transform.rotation = Quaternion.Slerp(transform.rotation, rndValue, 0.5f * Time.deltaTime);

    }

    IEnumerator GetRndRotation()
    {
        isRnd = true;
        yield return new WaitForSeconds(setTime);
        isRnd = false;
        rndValue = Random.rotation;
    }
}
