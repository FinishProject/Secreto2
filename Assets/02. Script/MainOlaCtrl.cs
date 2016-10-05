using UnityEngine;
using System.Collections;

public class MainOlaCtrl : MonoBehaviour {

    public float Length = 3f;
    public float speed = 3f;

	void Update () {
        float moveSpeed = (Mathf.Sin(Time.time * speed) * Length);

        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
	
	}
}
