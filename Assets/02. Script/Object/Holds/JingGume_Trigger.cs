using UnityEngine;
using System.Collections;

public class JingGume_Trigger : MonoBehaviour
{

    public GameObject[] gObjects = null;
    public int[] requisiteCnts = null;     // 필요 조건 (개수)
    private int Cnt = 0;

    JingGume_Trigger parent;

    void Start()
    {
        //발판 끄기
        for (int i = 1; i < gObjects.Length; i++)
        {
            //            gObjects[i].SetActive(false);
            gObjects[i].GetComponentInParent<JingGume_Trigger>().setInit();
        }
        if (transform.parent)
            parent = transform.parent.GetComponentInParent<JingGume_Trigger>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Cnt++;
            parent.OnHolds();
        }
    }

    void OnHolds()
    {
        for (int i = 1; i < gObjects.Length; i++)
        {
            if (requisiteCnts[i] <= gObjects[i - 1].GetComponent<JingGume_Trigger>().Cnt)
            {
                //                gObjects[i].SetActive(true);
                StartCoroutine(gObjects[i].GetComponentInParent<JingGume_Trigger>().OpacityUp());
            }
        }
    }


    MeshRenderer meshRender; // 색정보 변경을 위한 변수
    Color color;             // 색정보 (오퍼시티 값을 조절하기 위해)
    BoxCollider[] colliders; // 콜리더 ( 한 발판에 콜리더 여러개 붙어 있는 경우가 있음)

    void setInit()
    {
        meshRender = GetComponent<MeshRenderer>();
        color = new Color(meshRender.material.color.r,
                         meshRender.material.color.g,
                         meshRender.material.color.b,
                         -1);
        meshRender.material.color = color;
        colliders = GetComponentsInChildren<BoxCollider>();
        for (int j = 0; j < colliders.Length; j++)
            colliders[j].enabled = false;
    }

    IEnumerator OpacityUp()
    {

        while (true)
        {
            color.a += 3f * Time.deltaTime; ;
            meshRender.material.color = color;

            if (color.a > 1f)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = true;
                }
                break;
            }
            yield return null;
        }
    }

}
