using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseUI : MonoBehaviour
{
    public GameObject titlePopup;
    public GameObject exitPopup;
    public GameObject selectButton;
    public GameObject menu;
    public GameObject worldMap;

    int curSelectIdx;               // 현재 선택된 메뉴
    bool inputedSpace;              // 스페이스바 입력 상태
    bool isInPopup;                 // 팝업 메뉴창이 떠 있는지

    Transform[] menuButtons;        // 메뉴들의 버튼 위치

    [System.Serializable]
    public struct PopupButtons
    {
        public GameObject yes_Off;
        public GameObject yes_On;
        public GameObject no_Off;
        public GameObject no_On;
    }
    public PopupButtons titlePopupButtons;
    public PopupButtons exitPopupButtons;

    void OnEnable()
    {
        Time.timeScale = 0;     // 게임 시간을 멈추기 위함
        curSelectIdx = 1;       
        menuButtons = menu.GetComponentsInChildren<Transform>();

        ClossExitPopup();
        ClossTitlePopup();

        inputedSpace = false;
        isInPopup = false;

        StartCoroutine(SelectMenu());
    }

    void OnDisable()
    {
        Time.timeScale = 1;
    }

    // PauseUI 종료
    public void ClosePauseUI()
    {
        if(!isInPopup)
            gameObject.SetActive(false);
    }

    #region 지도

    public void OpenWorlMap()
    {
        isInPopup = true;
        worldMap.SetActive(true);
        StartCoroutine(InWorlMap());
    }

    IEnumerator InWorlMap()
    {
        while(true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseWorlMap();
                break;
            }

            yield return null;
        }
    }
    public void CloseWorlMap()
    {
        isInPopup = false;
        worldMap.SetActive(false);
    }

    #endregion

    #region 스페이스바 입력
    // 시간 딜레이를 위한 함수 ( Time.timeScale = 0 이면 WaitForSeconds가 안먹혀서 사용 )
    static IEnumerator WaitForRealSeconds(float delay)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + delay)
        {
            yield return null;
        }
    }

    // 스페이스바 입력시 ( 중복 입력 방지 )
    IEnumerator InputSpace(float delay)
    {
        inputedSpace = true;
        yield return StartCoroutine(WaitForRealSeconds(delay));
        inputedSpace = false;

    }
    #endregion


    #region 메뉴
    // 매뉴 선택 조작
    IEnumerator SelectMenu()
    {
        while (true)
        {
            if ( Input.GetKeyDown(KeyCode.UpArrow) && !isInPopup && curSelectIdx > 1)
                curSelectIdx -= 2;
            else if (Input.GetKeyDown(KeyCode.DownArrow) && !isInPopup && curSelectIdx < 9)
                curSelectIdx += 2;
            else if (Input.GetKeyDown(KeyCode.Space) && !inputedSpace)
            {
                StartCoroutine( InputSpace(0.1f) );
                ConnectFunction_Menu(curSelectIdx);
            }  

            else
            {
                Vector3 tempPos = selectButton.transform.position;
                tempPos.y = menuButtons[curSelectIdx].position.y;
                selectButton.transform.position = tempPos;
            }

            yield return null;
        }
    }

    // 매뉴 선택 시 
    void ConnectFunction_Menu(int curIdx)
    {
        switch (curIdx)
        {
            case 1: OpenWorlMap(); break;
            case 3: break;
            case 5: break;
            case 7: OpenTitlePopup(); break;
            case 9: OpenExitPopup(); break;

        }
    }
    #endregion


    #region 종료 팝업
    IEnumerator SelectYesNo_Exit()
    {
        bool isYes = false;
        exitPopupButtons.yes_Off.SetActive(true);
        exitPopupButtons.yes_On.SetActive(false);
        exitPopupButtons.no_Off.SetActive(false);
        exitPopupButtons.no_On.SetActive(true);

        while (true)
        {
            yield return null;
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                isYes = false;
                exitPopupButtons.yes_Off.SetActive(true);
                exitPopupButtons.yes_On.SetActive(false);
                exitPopupButtons.no_Off.SetActive(false);
                exitPopupButtons.no_On.SetActive(true);

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                isYes = true;
                exitPopupButtons.yes_Off.SetActive(false);
                exitPopupButtons.yes_On.SetActive(true);
                exitPopupButtons.no_Off.SetActive(true);
                exitPopupButtons.no_On.SetActive(false);

            }
            else if (Input.GetKeyDown(KeyCode.Space) && !inputedSpace)
            {
                ConnectFunction_Exit_YesNo(isYes);
                StartCoroutine(InputSpace(0.1f));
                break;
            }
        }
    }

    void ConnectFunction_Exit_YesNo(bool isYes)
    {
        if (isYes)
        {
            ExitGame();
        }
        else
        {
            ClossExitPopup();
        }
    }


    public void OpenExitPopup()
    {
        isInPopup = true;
        exitPopup.SetActive(true);
        StartCoroutine(SelectYesNo_Exit());
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ClossExitPopup()
    {
        isInPopup = false;
        exitPopup.SetActive(false);
    }

    public void ClickYesNoButton_Exit(bool isYes)
    {
        if (isYes)
        {
            exitPopupButtons.yes_Off.SetActive(false);
            exitPopupButtons.yes_On.SetActive(true);
            exitPopupButtons.no_Off.SetActive(true);
            exitPopupButtons.no_On.SetActive(false);
        }
        else
        {
            exitPopupButtons.yes_Off.SetActive(true);
            exitPopupButtons.yes_On.SetActive(false);
            exitPopupButtons.no_Off.SetActive(false);
            exitPopupButtons.no_On.SetActive(true);
        }
    }
    #endregion


    #region 타이틀 팝업
    IEnumerator SelectYesNo_Title()
    {
        bool isYes = false;
        titlePopupButtons.yes_Off.SetActive(true);
        titlePopupButtons.yes_On.SetActive(false);
        titlePopupButtons.no_Off.SetActive(false);
        titlePopupButtons.no_On.SetActive(true);

        while (true)
        {
            yield return null;
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                isYes = false;
                titlePopupButtons.yes_Off.SetActive(true);
                titlePopupButtons.yes_On.SetActive(false);
                titlePopupButtons.no_Off.SetActive(false);
                titlePopupButtons.no_On.SetActive(true);

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                isYes = true;
                titlePopupButtons.yes_Off.SetActive(false);
                titlePopupButtons.yes_On.SetActive(true);
                titlePopupButtons.no_Off.SetActive(true);
                titlePopupButtons.no_On.SetActive(false);

            }
            else if (Input.GetKeyDown(KeyCode.Space) && !inputedSpace)
            {
                ConnectFunction_Title_YesNo(isYes);
                StartCoroutine(InputSpace(0.1f));
                break;
            }
        }
    }

    void ConnectFunction_Title_YesNo(bool isYes)
    {

        if (isYes)
        {
            ToTitleScene();
        }
        else
        {
            ClossTitlePopup();
        }
    }

    public void OpenTitlePopup()
    {
        isInPopup = true;
        titlePopup.SetActive(true);
        StartCoroutine(SelectYesNo_Title());
    }

    public void ToTitleScene()
    {
        Application.LoadLevel("MainScene");
    }

    public void ClossTitlePopup()
    {
        isInPopup = false;
        titlePopup.SetActive(false);
    }

    public void ClickYesNoButton_Title(bool isYes)
    {
        if (isYes)
        {
            titlePopupButtons.yes_Off.SetActive(false);
            titlePopupButtons.yes_On.SetActive(true);
            titlePopupButtons.no_Off.SetActive(true);
            titlePopupButtons.no_On.SetActive(false);
        }
        else
        {
            titlePopupButtons.yes_Off.SetActive(true);
            titlePopupButtons.yes_On.SetActive(false);
            titlePopupButtons.no_Off.SetActive(false);
            titlePopupButtons.no_On.SetActive(true);
        }
    }
    #endregion
}
