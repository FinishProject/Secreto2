using UnityEngine;
using System.Collections;

public class AltarCtrl : MonoBehaviour {

    public Renderer[] render = new Renderer[3];
    private Color offColor;
    private Color originColor;
    private Color drawColor;

    private bool isDraw = false;
    private bool isClear = false;
    private bool isOnBox = false;

    Color targetColor = new Color(0f, 0.8117652f, 1.5f);

    public GameObject altarEffect;
    public Transform stepHold;
    public GameObject infoUI;

    private Vector3 originPos, finishPos;

    public float length = 0.5f;
    public float speed = 1f;

   
    void Start()
    {
        //originColor = new Color(0f, 0.8117652f, 1.5f);
        originColor = new Color(0.322f, 0.322f, 0.322f);
        for (int i = 0; i < render.Length; i++)
        {
            render[i].material.SetColor("_EmissionColor", originColor);
        }

        originPos = stepHold.position;
        finishPos = originPos;
        finishPos.y -= length;

        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("OBJECT"))
        {
            PlayerCtrl.instance.animReset();
            StartCoroutine(ShowUI());
            isDraw = true;
            isClear = false;
            StartCoroutine(DrawColor());
            altarEffect.SetActive(true);
            isOnBox = true;
        }
    }

    IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(8f);
        infoUI.SetActive(true);
    }

  
    void OnCollisionExit(Collision col)
    {
        if (col.collider.CompareTag("OBJECT") && isOnBox)
        {
            SoundMgr.instance.StopAudio("Rock_On");
            isDraw = false;
            isClear = true;
            altarEffect.SetActive(false);
            StartCoroutine(ClearColor());
            isOnBox = false;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isDraw = true;
            isClear = false;
            StartCoroutine(DrawColor());
        }

    }


    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player") && !isOnBox)
        {
            isDraw = false;
            isClear = true;
            StartCoroutine(ClearColor());
        }
    }

    IEnumerator DrawColor()
    {
        drawColor = render[0].material.GetColor("_EmissionColor");
        //SoundMgr.instance.PlayAudio("Rock_On");
        while (isDraw)
        {
            if (drawColor.r >= targetColor.r)
                drawColor.r -= 0.05f;
            if (drawColor.g <= targetColor.g)
                drawColor.g += 0.02f;
            if (drawColor.b <= targetColor.b)
                drawColor.b += 0.03f;

            for (int i = 0; i < render.Length; i++)
            {
                render[i].material.SetColor("_EmissionColor", drawColor);
            }

            stepHold.position = Vector3.MoveTowards(stepHold.position, finishPos, speed * Time.deltaTime);

            if (drawColor.r <= targetColor.r && drawColor.g >= targetColor.g
                && drawColor.b >= targetColor.b)
                break;

            yield return null;
        }
        //SoundMgr.instance.StopAudio("Rock_On");
    }

    IEnumerator ClearColor()
    {
        drawColor = render[0].material.GetColor("_EmissionColor");
        while (isClear)
        {
            if (drawColor.r <= originColor.r)
                drawColor.r += 0.05f;
            if (drawColor.g >= originColor.g)
                drawColor.g -= 0.02f;
            if (drawColor.b >= originColor.b)
                drawColor.b -= 0.03f;

            for (int i = 0; i < render.Length; i++)
            {
                render[i].material.SetColor("_EmissionColor", drawColor);
            }

            stepHold.position = Vector3.MoveTowards(stepHold.position, originPos, speed * Time.deltaTime);

            if (drawColor.r >= originColor.r && drawColor.g <= originColor.g && 
                drawColor.b <= originColor.b)
                break;

            yield return null;
        }
    }
}
