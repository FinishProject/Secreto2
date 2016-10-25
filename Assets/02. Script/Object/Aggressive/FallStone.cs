using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallStone : MonoBehaviour {

    public int spawnNum = 2;
    private int arrayIndex = 0;

    private bool isActive = false;

    public GameObject stoneObject;
    private GameObject[] fallStone;

    public static FallStone instance;

    private Queue<int> stoneIndex = new Queue<int>();

	void Start () {
        instance = this;

        //fallStone = new GameObject[spawnNum];

        //for (int i = 0; i < spawnNum; i++)
        //{
        //    fallStone[i] = (GameObject)Instantiate(stoneObject, this.transform.position, Quaternion.identity);
        //    fallStone[i].SetActive(false);
        //}
	}
	
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isActive)
        {
            isActive = true;
            StartCoroutine(SpawnStone());
        }
    }
    
    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isActive = false;
        }
    }

    IEnumerator SpawnStone()
    {
        while (isActive)
        {
            GameObject stoneObj = (GameObject)Instantiate(stoneObject,
                new Vector3(
                    PlayerCtrl.instance.transform.position.x,
                    PlayerCtrl.instance.transform.position.y + 20f,
                    PlayerCtrl.instance.transform.position.z),
            new Quaternion(0, 0, 0, 0));

            yield return new WaitForSeconds(5f);

            Destroy(stoneObj);
            
            yield return null;
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
                    PlayerCtrl.instance.transform.position.x + 2f, 
                    PlayerCtrl.instance.transform.position.y + 20f,
                    PlayerCtrl.instance.transform.position.z);

                fallStone[arrayIndex].transform.position = spawnPos;

                CameraCtrl_6.instance.StartShake(0.13f);

                StartCoroutine(RemoveSotne(arrayIndex));
                arrayIndex++;
                
                yield return new WaitForSeconds(5f);
            }

            if (arrayIndex >= spawnNum)
                arrayIndex = 0;

            yield return null;
        }
    }

    IEnumerator RemoveSotne(int curIndex)
    {
        stoneIndex.Enqueue(curIndex);
        yield return new WaitForSeconds(3f);
        int removeIndex = stoneIndex.Dequeue();
        fallStone[removeIndex].SetActive(false);
    }
}
