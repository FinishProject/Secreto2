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
            if (arrayIndex < spawnNum && !fallStone[arrayIndex].activeSelf)
            {
                fallStone[arrayIndex].SetActive(true);

                Vector3 spawnPos = new Vector3 (
                    PlayerCtrl.instance.transform.position.x + 5f, 
                    PlayerCtrl.instance.transform.position.y + 20f,
                    PlayerCtrl.instance.transform.position.z);

                fallStone[arrayIndex].transform.position = spawnPos;
                Debug.Log(fallStone[arrayIndex].transform.position);
                arrayIndex++;
                yield return new WaitForSeconds(2f);
            }

            if (arrayIndex >= spawnNum)
                arrayIndex = 0;

            yield return null;
        }
    }
}
