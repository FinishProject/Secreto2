using UnityEngine;
using System.Collections;

public class WayPoint : MonoBehaviour
{

    public delegate void SaveSystem();
    public static event SaveSystem OnSave;

    public GameObject[] effect;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && PlayerCtrl.dying)
        {
            InGameUI_2.instance.AvtiveLoad();
        }
        else if (col.CompareTag("Player") && !PlayerCtrl.dying)
        {
            OnSave();
            InGameUI_2.instance.AvtiveSave();

            if(effect.Length > 0)
                StartCoroutine(SetEffect());
        }

    }

    IEnumerator SetEffect()
    {
        Debug.Log("Effect");
        for (int i = 0; i < effect.Length; i++)
            effect[i].SetActive(true);

        yield return new WaitForSeconds(2.5f);
        effect[0].SetActive(false);
    }
}
