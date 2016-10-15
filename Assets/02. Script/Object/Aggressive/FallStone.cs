using UnityEngine;
using System.Collections;

public class FallStone : MonoBehaviour {

    public int spawnNum = 2;
    private int arrayIndex = 0;

    private bool isActive = false;

    public GameObject stoneObject;
    private GameObject[] fallStone;

    public static FallStone instance;

	void Start () {
        instance = this;

        fallStone = new GameObject[spawnNum];

        for (int i = 0; i < spawnNum; i++)
        {
            fallStone[i] = (GameObject)Instantiate(stoneObject, this.transform.position, Quaternion.identity);
            fallStone[i].SetActive(false);
        }
	}
	
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isActive)
        {
            isActive = true;
            StartCoroutine(FallStoneSpawn());
        }
    }
    
    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isActive = false;
        }
    }

    IEnumerator FallStoneSpawn()
    {
        while (isActive)
        {
            Vector3 spawnPos = PlayerCtrl.instance.transform.position;
            spawnPos.y += 20f;
            spawnPos.x += 5f;
            if (arrayIndex < spawnNum && !fallStone[arrayIndex].activeSelf)
            {
                fallStone[arrayIndex].SetActive(true);
                fallStone[arrayIndex].transform.position = spawnPos;
                arrayIndex++;
                yield return new WaitForSeconds(2f);
            }

            if (arrayIndex >= spawnNum)
                arrayIndex = 0;

            yield return null;
        }
    }
}
