using UnityEngine;
using System.Collections;

public class Layer_Object : MonoBehaviour {

    public float keepTime = 3f;
    public float delayTime = 3f;
    public GameObject layerBody;

    void Start()
    {
        StartCoroutine( ShootLayer());
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerCtrl>().getDamage(100);
        }
    }

    IEnumerator ShootLayer()
    {
        while(true)
        {
            layerBody.SetActive(false);
            yield return new WaitForSeconds(delayTime);
            layerBody.SetActive(true);
            yield return new WaitForSeconds(keepTime);
        }
    }
}
