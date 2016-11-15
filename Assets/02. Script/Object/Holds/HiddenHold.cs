using UnityEngine;
using System.Collections;

public class HiddenHold : MonoBehaviour {

    public Shader transShader;
    private Shader standardShader;

    public GameObject[] nextHold;
    private Vector3 originPos, finishPos;

    public float length = 3f;

    // Use this for initialization
    void Start () {
        standardShader = Shader.Find("Standard");

        if (transShader == null)
            transShader = Shader.Find("Custom/balpan_trans");

        originPos = transform.position;
        finishPos = originPos;
        finishPos.y -= length;

        transform.position = finishPos;
	}
	
	void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player") && nextHold != null)
        {
            Debug.Log("Set Next Hold");
        }
    }
}
