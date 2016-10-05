using UnityEngine;
using System.Collections;

public class serch : MonoBehaviour {


    void Serch()
    {
        //                0  1  2  3   4   5   6   7
        float[] array = { 1, 2, 3, 8, 10, 30, 84, 95};
        float nearIdx = 0;
        float range = 9999;
        float target = 12f;

        float temp;
        for (int i = 0; i < array.Length; i++)
        {
            
            temp = Mathf.Abs(target - array[i]);
            
            if (temp < range)
            {
                range = temp;
                nearIdx = i;
            }
        }

    }

	// Use this for initialization
	void Start () {
        Serch();

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
