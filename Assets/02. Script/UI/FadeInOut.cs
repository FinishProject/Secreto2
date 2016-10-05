using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.ImageEffects;


public class FadeInOut : MonoBehaviour {

    Image BlackImg;
    float alpha;
    bool trigger;
    VignetteAndChromaticAberration vignette;

    [System.Serializable]
    public struct RGB
    {
        public float R;
        public float G;
        public float B;

    }
    [System.Serializable]
    public struct Fade_PlayerDead
    {
        public float fadeInTime;
        public float waitTime;
        public float fadeOutTime;


        public float alpha_max;
        [System.NonSerialized]
        public float Vgnetting_min;
        public float Vgnetting_max;

        [System.NonSerialized]
        public float chromaticAberration_min;
        public float chromaticAberration_max;
        
        public RGB BackGround_RGB;
    }

    public Fade_PlayerDead fade_PlayerDead_Info;
    public static FadeInOut instance;

    void Start()
    {
        instance = this;
        trigger = false;

        BlackImg = gameObject.GetComponent<Image>();
        vignette = Camera.main.GetComponent<VignetteAndChromaticAberration>();

        if (vignette != null)
        {
            fade_PlayerDead_Info.Vgnetting_min = vignette.intensity;
            fade_PlayerDead_Info.chromaticAberration_min = vignette.chromaticAberration;
        }

    }

//   void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Q))
//            StartFadeInOut_PlayerDead();
//    }

    public void StartFadeInOut(float fadeInTime, float waitTime, float fadeOutTime)
    {
        if(!trigger)
            StartCoroutine(Load(fadeInTime, waitTime, fadeOutTime));
    }

    public void StartFadeInOut_PlayerDead()
    {
        if (!trigger)
            StartCoroutine(Load_Dead());
    }

    IEnumerator Load(float fadeInTime, float waitTime, float fadeOutTime)
    {
        trigger = true;
        if (PlayerCtrl.instance != null)
            PlayerCtrl.instance.isMove = false;

        alpha = 0;
        while (alpha < 1 && fadeInTime > 0)
        {
            alpha += (1 / fadeInTime) * Time.deltaTime;
            BlackImg.color = new Color(0, 0, 0, alpha);
            yield return true;
        }

        if (PlayerCtrl.instance != null)
        {
            PlayerCtrl.instance.animReset();
        }

        if (waitTime > 0)
            yield return new WaitForSeconds(waitTime);

        alpha = 1;
        while (alpha > 0)
        {

            alpha -= (1 / fadeOutTime) * Time.deltaTime;
            BlackImg.color = new Color(0, 0, 0, alpha);
            yield return true;
        }
        trigger = false;
        if (PlayerCtrl.instance != null)
            PlayerCtrl.instance.isMove = true;
    }

    IEnumerator Load_Dead()
    {
        trigger = true;
        float timer = 0;
        while (timer < fade_PlayerDead_Info.fadeInTime)
        {
            vignette.intensity += (fade_PlayerDead_Info.Vgnetting_max / fade_PlayerDead_Info.fadeInTime) * Time.deltaTime;
            vignette.chromaticAberration += (fade_PlayerDead_Info.chromaticAberration_max / fade_PlayerDead_Info.fadeInTime) * Time.deltaTime;

            alpha += (fade_PlayerDead_Info.alpha_max / fade_PlayerDead_Info.fadeInTime) * Time.deltaTime;
            BlackImg.color = new Color(fade_PlayerDead_Info.BackGround_RGB.R/255,
                                       fade_PlayerDead_Info.BackGround_RGB.G/255,
                                       fade_PlayerDead_Info.BackGround_RGB.B/255, 
                                       alpha);
            timer += Time.deltaTime;
            yield return true;
        }
        timer = 0;

        yield return new WaitForSeconds(fade_PlayerDead_Info.waitTime);

        while (timer < fade_PlayerDead_Info.fadeOutTime)
        {
            vignette.intensity -= (fade_PlayerDead_Info.Vgnetting_max / fade_PlayerDead_Info.fadeOutTime) * Time.deltaTime;
            vignette.chromaticAberration -= (fade_PlayerDead_Info.chromaticAberration_max / fade_PlayerDead_Info.fadeOutTime) * Time.deltaTime;

            alpha -= (fade_PlayerDead_Info.alpha_max / fade_PlayerDead_Info.fadeOutTime) * Time.deltaTime;
            BlackImg.color = new Color(fade_PlayerDead_Info.BackGround_RGB.R/255,
                                       fade_PlayerDead_Info.BackGround_RGB.G/255,
                                       fade_PlayerDead_Info.BackGround_RGB.B/255,
                                       alpha);
            timer += Time.deltaTime;
            yield return true;
        }
        trigger = false;
    }
}
