using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Text scoreUI;
    private float score;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    public void SetScore()
    {
        score += 10f;
        scoreUI.text = score.ToString();
    }
}
