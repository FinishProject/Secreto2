using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour {

    private Text txt;
    private float score = 0f;

	// Use this for initialization
	void Start () {
        txt = GetComponent<Text>();
        txt.text = score.ToString();
    }
	
    public void SetScore()
    {
        score += 10f;
        txt.text = score.ToString();
    }
}
