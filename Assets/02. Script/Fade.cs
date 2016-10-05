using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fade : MonoBehaviour {
    public Texture2D fadeTexture;
    public float fadeSpeed = 0.8f;
    private float FadeDir = -1f;
    public float alpha;
    private int drawDepth = -1000;

    void OnGUI()
    {
        alpha += FadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
    }

    public float BeginFade(float direction)
    {
        FadeDir = direction;
        return alpha;
    }

    public void SetLoad()
    {
        FadeDir = -1f;
        alpha = 1;
    }

}
