using UnityEngine;
using System.Collections;

public class CloudeScroll : MonoBehaviour {

    public float scrollSpeed;
    public float length = 30f;

    public RectTransform recTr;
 
    void Update()
    {
        transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime);

        if(transform.position.x >= recTr.rect.width + length)
        {
            transform.position = new Vector3(-recTr.rect.width - length, transform.position.y, transform.position.z);
        }
    }
}
