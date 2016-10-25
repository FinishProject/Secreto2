using UnityEngine;
using System.Collections;

public class WanderFireball : MonoBehaviour {

    public float speed = 2f;
    public float maxLength = -150f;

    private Vector3 originLocation, finishLocation;

	// Use this for initialization
	void Start () {
        originLocation = transform.position;
        finishLocation = originLocation;
        finishLocation.x += maxLength;

        StartCoroutine(Movement());
	}

    IEnumerator Movement()
    {
        while (true)
        {

            if (transform.position.x.Equals(finishLocation.x))
            {
                Debug.Log("Finish");
            }

            transform.position = Vector3.MoveTowards(transform.position, finishLocation, speed * Time.deltaTime);
            yield return null;
        }
    }
}
