using UnityEngine;
using System.Collections;

public class LandTagOnOff : MonoBehaviour {

    public GameObject obj;

    void Start()
    {
        obj.SetActive(false);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            obj.SetActive(true);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            obj.SetActive(false);
        }
    }
}
