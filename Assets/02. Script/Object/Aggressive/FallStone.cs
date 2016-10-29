using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallStone : MonoBehaviour {


    private bool isActive = false;

    public GameObject stoneObject;
    public Transform[] points;

    public static FallStone instance;

    public AudioClip clip;
    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
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

    // 돌 생성
    IEnumerator SpawnStone()
    {
        while (isActive)
        {
            //CameraCtrl_6.instance.StartShake(0.5f);
            //SoundMgr.instance.PlayAudio("Earthquake", false, 1f);
            source.PlayOneShot(clip);
            yield return new WaitForSeconds(1f);

            int spawnIndxe = GetDistance();

            GameObject stone = (GameObject)Instantiate(
                stoneObject, 
                new Vector3(points[spawnIndxe].position.x,
                points[spawnIndxe].position.y,
                PlayerCtrl.instance.transform.position.z - 1f),
               new Quaternion(0, 0, 0, 0));

            Destroy(stone, 5f);
            yield return new WaitForSeconds(5.1f);

            source.Stop();

            yield return null;
        }
    }
     
    // 플레이어와 가장 가까운 위치 구하기
    int GetDistance()
    {
        float firstDis = (PlayerCtrl.instance.transform.position - points[0].position).sqrMagnitude;
        int spawnIndex = 0;

        for (int i = 0; i < points.Length; i++)
        {
            float secondDis = (PlayerCtrl.instance.transform.position - points[i].position).sqrMagnitude;

            if (firstDis > secondDis)
            {
                firstDis = secondDis;
                spawnIndex = i;
            }
        }
        return spawnIndex;
    }
}
