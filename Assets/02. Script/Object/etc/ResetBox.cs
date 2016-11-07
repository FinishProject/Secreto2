using UnityEngine;
using System.Collections;

public class ResetBox : MonoBehaviour {

    public Transform resetPoint;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("OBJECT"))
        {
            col.transform.position = resetPoint.position;
        }
    }
}
