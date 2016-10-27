using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallStone : MonoBehaviour {

    public int spawnNum = 2;
    private int arrayIndex = 0;

    private bool isActive = false;

    public GameObject stoneObject;
    public Transform[] points;
    private GameObject[] fallStone;

    public static FallStone instance;

    private Queue<int> stoneIndex = new Queue<int>();
	
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
            SoundMgr.instance.PlayAudio("Earthquake", false);

            yield return new WaitForSeconds(0.5f);

            int spawnIndxe = GetDistance();

            Debug.Log(spawnIndxe);

            GameObject stone = (GameObject)Instantiate(
                stoneObject, points[spawnIndxe].position,
               new Quaternion(0, 0, 0, 0));

            Destroy(stone, 5f);
            yield return new WaitForSeconds(5f);
            SoundMgr.instance.StopAudio("Earthquake");

            yield return new WaitForSeconds(1f);

            yield return null;
        }
    }

    int GetDistance()
    {
        float firstDis = (points[0].position - PlayerCtrl.instance.transform.position).sqrMagnitude;
        for(int i=1; i < points.Length; i++)
        {
            float secondDis = (points[i].position - PlayerCtrl.instance.transform.position).sqrMagnitude;

            if (firstDis >= secondDis)
                return i;
        }

        return 0;
    }
}
