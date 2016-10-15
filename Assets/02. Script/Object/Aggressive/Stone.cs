using UnityEngine;
using System.Collections;

public class Stone : MonoBehaviour {

    float countDown = 0f;

	void Update () {

        countDown += Time.deltaTime;

        if (countDown >= 3f)
        {
            this.gameObject.SetActive(false);
        }
	}

    void OnEnable()
    {
        countDown = 0f;
    }
}
