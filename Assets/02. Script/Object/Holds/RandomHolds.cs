using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomHolds : MonoBehaviour {

    public GameObject holds;

    private Transform[] points = new Transform[5];
    private List<GameObject> gaHold = new List<GameObject>();
    private List<int> beforeNum = new List<int>();

	void Start () {
        points = GetComponentsInChildren<Transform>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Init();
            StartCoroutine("SpwanHolds");
        }
    }

    IEnumerator SpwanHolds()
    {
        int rndNum = 0;
        while(beforeNum.Count < 3)
        {
            rndNum = Random.Range(1, points.Length);
            
            if (beforeNum.Contains(rndNum)) { rndNum = Random.Range(1, points.Length - 1); }
            else {
                beforeNum.Add(rndNum);
                gaHold.Add((GameObject)Instantiate(holds, points[rndNum].position, Quaternion.identity));
            }
            yield return null;
        }
    }

    void Init()
    {
        for(int i=0; i < gaHold.Count; i++) {
            Destroy(gaHold[i].gameObject);
        }
        gaHold.RemoveRange(0, gaHold.Count);
        beforeNum.RemoveRange(0, beforeNum.Count);
    }
}
