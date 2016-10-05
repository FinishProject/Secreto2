using UnityEngine;
using System.Collections;

public class SoundMgr : MonoBehaviour {

    public AudioClip objPushSound;

    private AudioSource source;

    public static SoundMgr instance;

    bool isTest = true;

	// Use this for initialization
	void Start () {
        instance = this;

        source = GetComponent<AudioSource>();
	}

    public void PushObject(bool isPlay)
    {
        if (!source.isPlaying && isPlay)
            source.PlayOneShot(objPushSound);
        else if (!isPlay)
            source.Stop();
    }
}
