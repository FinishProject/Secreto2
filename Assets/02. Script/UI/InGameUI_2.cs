using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameUI_2 : MonoBehaviour {

    public GameObject gameEnd;
    public GameObject pauseUI;
    public static InGameUI_2 instance;

    public Image saveImg;
    public Image loadImg;
    public Image circleImg;

    [System.Serializable]
    public struct SaveInfo
    {
        public int angle;
        public int speed;
    }
    public SaveInfo saveInfo;

    void Awake()
    {
        instance = this;
        pauseUI.SetActive(false);

        saveImg.enabled = false;
        loadImg.enabled = false;
        circleImg.enabled = false;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pauseUI.activeSelf)
        {
            pauseUI.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && pauseUI.activeSelf)
        {
            // 일시정지 UI에서도 ESC입력 처리를 따로 해야 하므로 pauseUI.SetActive(false) 사용 안함 
            pauseUI.GetComponent<PauseUI_2>().ClosePauseUI();
        }
    }

    public void GameEnd()
    {
        gameEnd.SetActive(true);
    }

    public void AvtiveSave()
    {
        StartCoroutine(SaveLoad(saveImg));
    }

    public void AvtiveLoad()
    {
        StartCoroutine(SaveLoad(loadImg));
    }

    public IEnumerator SaveLoad(Image img)
    {

        img.enabled = true;

        circleImg.enabled = true;
        float round = 0;

        while(true)
        {
            if (round > saveInfo.angle)
                break;

            circleImg.transform.Rotate(new Vector3(0, 0, -saveInfo.speed));

            round += saveInfo.speed;
            yield return new WaitForSeconds(0.01f);
        }


        img.enabled = false;
        circleImg.enabled = false;
    }
}
