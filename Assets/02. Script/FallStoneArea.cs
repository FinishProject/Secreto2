using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallStoneArea : MonoBehaviour {

    public GameObject stoneObj;
    public Transform[] point;

    private AudioSource source;

    private Queue<GameObject> stoneQueue = new Queue<GameObject>();

    public AudioClip clip;

    public static bool isActive = false;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    IEnumerator SpawnStone()
    {
        int beginIndex = 0, newIndex = 0;
        while (isActive)
        {
            GameObject stone = (GameObject)Instantiate(stoneObj, point[newIndex].position, Quaternion.identity);

            if(!source.isPlaying)
            {
                source.clip = clip;
                source.Play();
                source.loop = true;
            }

            CameraCtrl_1.instance.StartShaking();
            StartCoroutine(DestroyStone(stone));

            yield return new WaitForSeconds(2f);

            while (true)
            {
                newIndex = Random.Range(0, point.Length);

                if (beginIndex != newIndex)
                    break;

                yield return null;
            }

            beginIndex = newIndex;

            yield return null;
        }
        source.Stop();
    }
    IEnumerator DestroyStone(GameObject stone)
    {
        stoneQueue.Enqueue(stone);
        yield return new WaitForSeconds(5f);
        GameObject destroyObj = stoneQueue.Dequeue();
        Destroy(destroyObj);
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
        if (col.CompareTag("Player") && isActive)
        {
            isActive = false;
            StopCoroutine(SpawnStone());
        }
    }
}
