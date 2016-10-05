/*
using UnityEngine;
using System.Collections;
using UnityEditor.Events;
using UnityEngine.UI;
*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MovieMgr : MonoBehaviour
{
    public MovieTexture movieclips;
    public Image SkipImg;

    private MeshRenderer meshRenderer;
    private AudioSource audio;
    private bool trigger;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = movieclips;
        audio = GetComponent<AudioSource>();
        StartCoroutine(StartMovie());
    }

    IEnumerator StartMovie()
    {
        FadeInOut.instance.StartFadeInOut(0, 1, 2.5f);
        StartCoroutine(fadeSkip(true));
        yield return new WaitForSeconds(2f);

        audio.Play();
        movieclips.Play();
        trigger = false;

        while(true)
        {
            if (Input.GetKey(KeyCode.Escape) || !movieclips.isPlaying && !trigger)
            {
                trigger = true;
                StartCoroutine(fadeSkip(false));
                FadeInOut.instance.StartFadeInOut(1.5f, 3, 1.5f);
                yield return new WaitForSeconds(1.5f);
                Application.LoadLevel("LoadingScene");
                break;
            }
            yield return null;
        }
    }

    IEnumerator fadeSkip(bool fadeIn)
    {
        Color tempAlpha = SkipImg.color;
        float pressKeyFadeSpeed = 0.5f;

        if (!fadeIn)
        {
            tempAlpha.a = 1;
            pressKeyFadeSpeed *= -1;
        }
        else
        {
            tempAlpha.a = 0;
        }
        SkipImg.color = tempAlpha;

        while (true)
        {
            tempAlpha.a = pressKeyFadeSpeed * Time.deltaTime;
            SkipImg.color += tempAlpha;
            if (SkipImg.color.a >= 1.0f || SkipImg.color.a <= 0)
                break;

            yield return null;
        }
    }

}

/*
[RequireComponent(typeof(AudioSource))]
public class MovieMgr : MonoBehaviour
{
    Object[] movie_stills;
    int number_of_stills = 0;
    public bool loop = false;
    public bool playOnStart = false;
    public int fps = 30;
    public AudioClip sound;
    public string resourceSubfolder = "";

    private int stills = 0;
    private bool play = false;
    private bool loaded = false;
    private new AudioSource audio;
    void Update()
    {

        if (!resourceSubfolder.Equals("") && !loaded) {
            StartCoroutine(ImportVideo());
        }

        if (fps > 0)
        {
            if (play == true)
            {
                StartCoroutine(Player());
            }
        }
        else
        {
            Debug.LogError("'fps' must be set to a value greater than 0.");
        }
    }

    IEnumerator Player()
    {
        play = false;
        if (loop)
        {
            Debug.Log("looped. stills: " + stills + ", length: " + movie_stills.Length);
            if (stills >= movie_stills.Length)
            {
                audio.Stop();
                audio.clip = sound;
                audio.Play();
                stills = 0;
                Debug.Log("restarting. stills: " + stills);
            }
        }
        else
        {
            if (stills > movie_stills.Length)
            {
                audio.Stop();
                stills -= 1;
            }
        }

        if (stills >= 0 && stills < movie_stills.Length) {
            Texture2D MainTex = movie_stills[stills] as Texture2D;
            GetComponent<Renderer>().material.SetTexture("_MainTex", MainTex);
            stills += 1;
            int fps_fixer = fps * 3;
            float wait_time = 1.0f / fps_fixer;
            yield return new WaitForSeconds(wait_time);
            if (!audio.clip)
            {
                if (sound)
                {
                    audio.clip = sound;
                    audio.Play();
                }
            }
            play = true;
        }
    }

    public void Play() { play = true; }
    public void Pause() { play = false; }

    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.loop = false;
        number_of_stills -= 1;
    }


    IEnumerator ImportVideo()
    {
        movie_stills = Resources.LoadAll(resourceSubfolder, typeof(Texture2D));
        loaded = true;
        if (playOnStart)
            play = true;
        yield return null;
    }

    public void UnloadFromMemory()
    {
        play = false;
        audio.Stop();

        foreach (Object o in movie_stills) Destroy(o);
    }
}
*/

