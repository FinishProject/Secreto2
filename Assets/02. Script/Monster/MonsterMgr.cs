using UnityEngine;
using System.Collections;

public class MonsterMgr : MonoBehaviour {

    public GameObject[] goMon = new GameObject[3];

    private float posX = 1f;

    void Start()
    {
        for(int i=1; i<goMon.Length; i++) {
            goMon[i].SetActive(false);
        }
    }
    //분열
    public void Split(Vector3 parentPos)
    {
        for (int i = 1; i < goMon.Length; i++)
        {
            goMon[i].SetActive(true);
            posX *= -1f;
            Vector3 posVec = new Vector3(parentPos.x + posX, parentPos.y,
                0f);
            goMon[i].transform.position = posVec;
        }
        StartCoroutine("Parabola", parentPos);
    }
    //분열 시 포물선
    IEnumerator Parabola(Vector3 parentPos)
    {
        float focus = 1f, time = 0f;
        float vx, vy, gr = 9.8f;

        Vector3 startPos = parentPos;   // 시작 위치
        Vector3 finishPos = new Vector3(startPos.x - 5f, 0, 0); // 도착 위치

        vx = (finishPos.x - startPos.x) / 2f; // x 속도
        vy = (finishPos.y - startPos.y + 2f * gr) / 2f; // y 속도

        while (time <= 1.9f) {
            time += Time.deltaTime;
            for (int i = 1; i < goMon.Length; i++)
            {
                if (!goMon[i].activeSelf) {
                    if (i == 1) focus = -1f;
                    else focus = 1f;
                }
                else focus *= -1;

                float sx = startPos.x + vx * time * focus;
                float sy = startPos.y + vy * time - 0.5f * gr * time * time;
                goMon[i].transform.position = new Vector3(sx, sy, 0f); 
            }
            yield return null;
        }
    }

}
