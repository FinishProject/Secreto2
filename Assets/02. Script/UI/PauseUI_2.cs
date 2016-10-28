using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseUI_2 : MonoBehaviour
{
    public Image YesImg;
    public Image NoImg;

    public AudioSource source;
    public AudioClip clip;

    private bool isSelectYes;
    public bool IsSelectYes // 현재 선택된 메뉴
    {
        get
        {
            return isSelectYes; 
        }
        set
        {
            isSelectYes = value;
            if(isSelectYes)
            {
                YesImg.enabled = true;
                NoImg.enabled = false;
            }
            else
            {
                YesImg.enabled = false;
                NoImg.enabled = true;
            }
        }
    }

    void OnEnable()
    {
        Time.timeScale = 0;        // 게임 시간을 멈추기 위함
        IsSelectYes = true;
        YesImg.enabled = true;
        NoImg.enabled = false;
    }

    void OnDisable()
    {
        Time.timeScale = 1;
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            IsSelectYes = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            IsSelectYes = false;
        }
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            //SoundMgr.instance.PlaySelectSound();
            source.PlayOneShot(clip);
            if (IsSelectYes)
                ToTitleScene();
            else
                ClosePauseUI();
        }
    }

    // 타이틀 씬으로
    public void ToTitleScene()
    {
        Application.LoadLevel("MainScene 1");
    }

    // PauseUI 종료
    public void ClosePauseUI()
    {
        gameObject.SetActive(false);
    }

    // 예 버튼 눌렀을 때
    public void selectYesButton()
    {
        if (IsSelectYes)
            ToTitleScene();
        else
            IsSelectYes = true;
    }

    // 아니오 버튼 눌렀을 때
    public void selectNoButton()
    {
        if (!IsSelectYes)
            ClosePauseUI();
        else
            IsSelectYes = false;
    }

}
