using UnityEngine;
using System.Collections;

public class WayPoint : MonoBehaviour
{

    public delegate void SaveSystem();
    public static event SaveSystem OnSave;

    public GameObject[] effect;

    public AudioClip clip;
    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && PlayerCtrl.dying)
        {
            InGameUI_2.instance.AvtiveLoad();
        }
        else if (col.CompareTag("Player") && !PlayerCtrl.dying)
        {
            //saveTr = col.transform;
            //OnSave();
            PlayerCtrl.instance.Save();
            InGameUI_2.instance.AvtiveSave();

            if(effect.Length > 0)
                StartCoroutine(SetEffect());

            if (!source.isPlaying)
                source.PlayOneShot(clip);
        }

    }

    IEnumerator SetEffect()
    {
        SoundMgr.instance.PlayAudio("Save_Point", false, 0.3f);
        for (int i = 0; i < effect.Length; i++)
            effect[i].SetActive(true);

        yield return new WaitForSeconds(2.5f);
        effect[0].SetActive(false);
        SoundMgr.instance.StopAudio("Save_Point");
    }
}
