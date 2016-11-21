using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Text scoreUI;
    private float score;

    public static GameManager instance;

    private GameObject player;

    public AudioClip[] clip;
    private AudioSource source;

    public static bool playerDie = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        player = PlayerCtrl.instance.gameObject;

        source = GetComponent<AudioSource>();
    }

    public void SetScore()
    {
        score += 10f;
        scoreUI.text = score.ToString();
    }

    public void SetPlayerDie()
    {
        if (!source.isPlaying)
            source.PlayOneShot(clip[0]);
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        playerDie = true;
        PlayerCtrl.instance.SetStopMove(false);
        player.SetActive(false);
        FadeInOut.instance.StartFadeInOut(1, 2, 3);

        yield return new WaitForSeconds(1.5f);
        player.SetActive(true);
        PlayerCtrl.instance.GetPlayerData();

        if (!PlayerCtrl.isFocusRight)
        {
            Quaternion localRot = PlayerCtrl.instance.transform.rotation;
            localRot.w = 0.7f;
            PlayerCtrl.instance.transform.rotation = localRot;
        }

        if (source.isPlaying)
            source.Stop();
        source.PlayOneShot(clip[1]);

        yield return new WaitForSeconds(2f);

        PlayerCtrl.instance.SetStopMove(true);
        PlayerCtrl.dying = false;
        playerDie = false;
    }
}
