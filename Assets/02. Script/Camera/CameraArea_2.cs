using UnityEngine;
using System.Collections;

public class CameraArea_2 : MonoBehaviour {

    public float val;
    public bool moving;
    public Transform moving_bace;

    private Vector3 orign;
    
    void Start()
    {
        orign = transform.position;
        if (moving)
            StartCoroutine(UpdatePos());
    }

    IEnumerator UpdatePos()
    {
        while(true)
        {
            orign.y = moving_bace.transform.position.y+1;
            transform.position = orign;
            val = moving_bace.position.y + 1;
            yield return null;
        }   
    }

}
