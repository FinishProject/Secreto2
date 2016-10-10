using UnityEngine;
using System.Collections;

public class JingGume : MonoBehaviour {

    public GameObject[] gObject;
    private int cnt = 1;
    private bool isStep = false; // 밟은 여부
    private Color objColor;

    private JingGume parent;

    void Start () {

        //발판 오브젝트 끄기
        for (int i = 1; i < gObject.Length; i++)
        {
            Renderer render = gObject[i].GetComponent<Renderer>();
            objColor = render.material.color;
            objColor.a = 0f;
            render.material.color = objColor;

            gObject[i].SetActive(false);
        }
        // 부모 객체가 있을 시 부모 객체의 스크립트 가져옴
        if (transform.parent)
            parent = transform.parent.GetComponentInParent<JingGume>();
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isStep)
        {
            // 플레이어가 발판 밟을 시 부모 객체의 OnHolds 실행
            this.isStep = true;
            parent.OnHolds();
        }
    }
    // 발판 생성
    void OnHolds()
    {
        if (cnt < gObject.Length)
        {
            gObject[cnt].SetActive(true);
            StartCoroutine(UpHold(gObject[cnt]));
            cnt++;
        }
    }
    // 발판을 위로 올리고 알파값을 올림.
    IEnumerator UpHold(GameObject moveHold)
    {
        // 위치값 설정
        Vector3 originPos = moveHold.transform.position;
        Vector3 spawnPos = originPos;
        spawnPos.y -= 1f;
        moveHold.transform.position = spawnPos;

        // 알파값 설정
        float alpha = 0f;
        Renderer render = moveHold.GetComponent<Renderer>();
        objColor = render.material.color;

        while (true)
        {
            // 목표 위치 도달 시 반복문 빠져나감
            if (moveHold.transform.position.y.Equals(originPos.y))
                break;

            // 알파값을 1까지 올림.
            alpha += 2f * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            objColor.a = alpha;
            render.material.color = objColor;

            // 오브젝트 이동
            moveHold.transform.position = Vector3.MoveTowards(moveHold.transform.position, originPos, 2f * Time.deltaTime);
            yield return null;
        }
    }
}
