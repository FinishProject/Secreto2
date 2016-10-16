using UnityEngine;
using System.Collections;

public class Stone : MonoBehaviour {

    public float speed = 30f;
    float moveSpeed = 0f;
    float countDown = 0f;

	void Update () {

        countDown += Time.deltaTime;
        moveSpeed += speed * Time.deltaTime;
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        if (countDown >= 3f)
        {
            this.gameObject.SetActive(false);
        }
	}

    void OnEnable()
    {
        moveSpeed = 0f;
        countDown = 0f;
    }
}
