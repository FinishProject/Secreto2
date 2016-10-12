using UnityEngine;
using System.Collections;

public class rotateTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * 20f * Time.deltaTime);
	}
}
