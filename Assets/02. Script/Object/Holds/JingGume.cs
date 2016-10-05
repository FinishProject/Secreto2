using UnityEngine;
using System.Collections;

public class JingGume : MonoBehaviour {

    public GameObject[] gObject;
    private int cnt = 1;
    private bool isStep = false; // 밟은 여부

    private JingGume parent;

    void Start () {

        //발판 오브젝트 끄기
        for (int i = 1; i < gObject.Length; i++)
        {
            gObject[i].SetActive(false);
        }
        if (transform.parent)
            parent = transform.parent.GetComponentInParent<JingGume>();
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isStep)
        {
            this.isStep = true;
            parent.OnHolds();
        }
    }

    void OnHolds()
    {
        if (cnt < gObject.Length)
        {
            gObject[cnt].SetActive(true);
            StartCoroutine(UpHold(gObject[cnt]));
            cnt++;
        }
    }

    IEnumerator UpHold(GameObject moveHold)
    {
        Vector3 originPos = moveHold.transform.position;
        Vector3 spawnPos = originPos;
        spawnPos.y -= 1f;
        moveHold.transform.position = spawnPos;

        while (true)
        {
            if (moveHold.transform.position.y.Equals(originPos.y))
                break;

            moveHold.transform.position = Vector3.MoveTowards(moveHold.transform.position, originPos, 2f * Time.deltaTime);
            yield return null;
        }
    }
}
