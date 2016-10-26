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
            CameraCtrl_6.instance.StartShake(0.5f);

            yield return new WaitForSeconds(0.5f);

            GameObject stone = (GameObject)Instantiate(stoneObject,
                new Vector3(PlayerCtrl.instance.transform.position.x + 1f,
                PlayerCtrl.instance.transform.position.y + 10f,
                PlayerCtrl.instance.transform.position.z), 
                new Quaternion(180f, 0f, 0f, 0));

            

            yield return new WaitForSeconds(6f);

            yield return null;
        }
    }

    void DestroyObject(GameObject obj)
    {
        Destroy(obj, 5f);
    }
}
