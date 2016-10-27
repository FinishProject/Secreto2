using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundMgr : MonoBehaviour {

    public AudioClip[] audioArray;

    private AudioSource[] source;
    public static SoundMgr instance;
    public List<AudioSource> sourceList = new List<AudioSource>();

	void Start () {
        instance = this;

        source = GetComponents<AudioSource>();
    }
    // 사운드 실행
    public void PlayAudio(string audioName)
    {
        AudioClip newClip = FindAudioClip(audioName);
        if (!GetPlayingClip(newClip))
        {
            sourceList.Add(gameObject.AddComponent<AudioSource>());
            if (newClip != null)
            {
                sourceList[sourceList.Count - 1].clip = newClip;
                sourceList[sourceList.Count - 1].Play();
            }
        }
    }

    // 현재 플레이중인 클립 여부
    bool GetPlayingClip(AudioClip clipName)
    {
        for (int i = 0; i < sourceList.Count; i++)
        {
            if (sourceList[i] == null)
                sourceList.RemoveAt(i);
            else if (sourceList[i].clip == clipName)
                return true;
        }
        return false;
    }

    // 배열에서 클립 찾기
    AudioClip FindAudioClip(string clipName)
    {
        for(int i=0; i<audioArray.Length; i++)
        {
            if (audioArray[i].name == clipName)
                return audioArray[i];
        }
        return null;
    }
    // 사운드 중단
    public void StopAudio(string clipName)
    {
        for (int i = 0; i < sourceList.Count; i++)
        {
            if(sourceList[i] == null)
                sourceList.RemoveAt(i);
            else if (sourceList[i].clip.name == clipName)
            {
                Destroy(sourceList[i].GetComponent<AudioSource>());
                sourceList.RemoveAt(i);
            }
        }
    }

}
