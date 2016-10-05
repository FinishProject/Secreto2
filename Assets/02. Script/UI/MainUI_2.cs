using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainUI_2 : MonoBehaviour
{
    [System.Serializable]
    public struct Cloudes
    {
        public Transform cloudeTr;
        public float speed;
        public float length;
    };
    [System.Serializable]
    public struct OlaInfo
    {
        public Transform olaTr;
        public float speed;
        public float length;
    }

    public Cloudes[] cloude = new Cloudes[3];
    private RectTransform rectTr;

    public OlaInfo ola;

    public Animator anim;
    public Image pressAnyKey;
    public GameObject selectButton;
    public GameObject menu;
    int curSelectIdx;
    Transform[] menuButtons;

    public float menuFadeSpeed = 0.3f;
    public float pressKeyFadeSpeed = 1f;

    void Awake()
    {
        rectTr = GetComponent<RectTransform>();
        anim = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
    }

    void Start()
    {
        curSelectIdx = 1;
        menuButtons = menu.GetComponentsInChildren<Transform>();
        selectButton.SetActive(false);
        menu.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(FadeText());
        StartCoroutine(OffAnimation());
    }

    void Update()
    {
        Debug.Log(curSelectIdx);
        CloudeMove();
        OlaMove();
    }

    void CloudeMove()
    {
        for(int i=0; i<cloude.Length; i++)
        {
            cloude[i].cloudeTr.Translate(Vector3.right * cloude[i].speed * Time.deltaTime);

            if(cloude[i].cloudeTr.position.x >= rectTr.rect.width + cloude[i].length)
            {
                cloude[i].cloudeTr.position = 
                    new Vector3(-rectTr.rect.width - cloude[i].length, cloude[i].cloudeTr.position.y, cloude[i].cloudeTr.position.z);
            }
        }
    }

    void OlaMove()
    {
        float moveSpeed = Mathf.Sin(Time.time * ola.speed) * ola.length;

        ola.olaTr.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }

    bool MouseInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse2) ||
            Input.GetKeyDown(KeyCode.Mouse3) || Input.GetKeyDown(KeyCode.Mouse4) || Input.GetKeyDown(KeyCode.Mouse5) || Input.GetKeyDown(KeyCode.Mouse6))
            return true;
        else
            return false;
    }

    IEnumerator OffAnimation()
    {
        yield return new WaitForSeconds(3f);
        anim.enabled = false;
        StartCoroutine(PressAnyKey());
    }

    IEnumerator PressAnyKey()
    {
        while (true)
        {
            // 키 입력을 받았을 때
            if (Input.anyKeyDown && !MouseInput())
            {
                menu.SetActive(true);
                selectButton.SetActive(true);
                StartCoroutine(fadeMenu(menu));
                pressAnyKey.enabled = false;
                break;
            }
            yield return null;
        }
    }

    IEnumerator FadeText()
    {
        Color tempAlpha = pressAnyKey.color;
        tempAlpha.a = 0;
        pressAnyKey.color = tempAlpha;

        while (true)
        {
            tempAlpha.a += pressKeyFadeSpeed * Time.deltaTime;
            pressAnyKey.color = tempAlpha;

            // alpha 반전 ( 완전 불투명 상태가 됬을 때)
            if (pressAnyKey.color.a >= 1.0f)
            {
                pressKeyFadeSpeed *= -1f;
            }

            // alpha 반전 ( 투명 상태가 됬을 때)
            else if (pressAnyKey.color.a <= 0)
            {
                tempAlpha.a = 0;
                pressKeyFadeSpeed *= -1f;
            }
            yield return null;
        }
    }

    // 메뉴를 페이드 해줌
    IEnumerator fadeMenu(GameObject obj)
    {
        GameObject[] objs = GetChildObj(obj);           // 자식 오브젝트를 불러옴
        Image[] objsImage = new Image[objs.Length + 1]; // 셀렉트 버튼을 추가 하기 위해 +1 해줌
        for (int i = 0; i < objs.Length + 1; i++)
        {
            if (i != objs.Length)
                objsImage[i] = objs[i].gameObject.GetComponent<Image>();
            else
                objsImage[i] = selectButton.GetComponent<Image>();          // 셀렉트 버튼과 메뉴 버튼은 부모자식 관계가 아니라 따로 추가 해줌

            objsImage[i].color = new Color(objsImage[i].color.r, objsImage[i].color.g, objsImage[i].color.b, 0);
        }


        Color tempAlpha = new Color(0, 0, 0, 0);

        tempAlpha.a = 0;
        pressAnyKey.color = tempAlpha;

        while (true)
        {
            tempAlpha.a = menuFadeSpeed * 2f * Time.deltaTime;
            for (int i = 0; i < objsImage.Length; i++)
                objsImage[i].color += tempAlpha;

            if (objsImage[0].color.a >= 1.0f)
            {
                StartCoroutine(SelectMenu());
                break;
            }

            yield return null;
        }
    }

    // 자식 오브젝트 받아 올 때 ( 자식이 없으면 자신을 반환 )
    GameObject[] GetChildObj(GameObject obj)
    {
        Transform[] tempObjs = obj.GetComponentsInChildren<Transform>();
        int arraySize = tempObjs.Length;
        arraySize = arraySize > 1 ? arraySize - 1 : arraySize;
        GameObject[] objs = new GameObject[arraySize];

        if (arraySize == 1)
            objs[0] = tempObjs[0].gameObject;

        for (int i = 0; i < tempObjs.Length - 1; i++)
            objs[i] = tempObjs[i + 1].gameObject;

        return objs;
    }



    IEnumerator SelectMenu()
    {
        while (true)
        {
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && curSelectIdx > 1)
                curSelectIdx--;
            else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && curSelectIdx < 2)
                curSelectIdx++;
            else if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                ConnectFunction(curSelectIdx);

            else
            {
                Vector3 tempPos = selectButton.transform.position;
                tempPos.y = menuButtons[curSelectIdx].position.y;
                selectButton.transform.position = tempPos;
            }

            yield return null;
        }
    }

    void ConnectFunction(int curIdx)
    {
        switch (curIdx)
        {
            case 1: StartCoroutine(StartNewGame()); break;
            case 2: ExitGame(); break;
        }
    }

    IEnumerator StartNewGame()
    {
        while (true)
        {
            float fadeAlpha = GetComponent<Fade>().BeginFade(1);

            if (fadeAlpha.Equals(1))
            {
                Application.LoadLevel("IntroMovieScene");
            }

            yield return null;
        }
    }


    public void ExitGame()
    {
        Application.Quit();
    }
}
