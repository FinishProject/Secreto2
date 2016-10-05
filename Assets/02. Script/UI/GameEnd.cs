using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameEnd : MonoBehaviour {

    Image BlackImg;
    float alpha;

    void OnEnable()
    {
        BlackImg = gameObject.GetComponent<Image>();
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {

        while (alpha < 1)
        {
            alpha += 0.5f * Time.deltaTime;
            BlackImg.color = new Color(0, 0, 0, alpha);
            yield return true;
        }


        //        Application.LoadLevel("CutScene");
        Application.LoadLevel("MainScene");


    }
}
