using UnityEngine;
using System.Collections;

public class BackGroundCtrl : MonoBehaviour {

    // 배경 배열
    public Transform[] backgrounds;
    // 플레이어
    public Transform player;
    // 처음 플레이어의 위치
    public Vector3 startPositionPlayer;
    public Vector3 startPositionHelper;
    // 시작 위치를 기준으로 움직인 거리
    private float playerDistanceX;
    private float playerDistanceY;
    // 속도
    private float speed = 1.0f;

    private float comparePosX;
    private float comparePosY;

    private Vector3 comparePos;
    // 선형보간
    private float smooth = 3.0f;

    public Transform tr;

    //public FollowCamera CameraScript;

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();

        tr = GetComponent<Transform>();
    }

    void Start()
    {
        //CameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowCamera>();

        startPositionPlayer = player.position;
    }

    void FixedUpdate()
    {

        //캐릭터의 첫 위치를 기준으로 움직인 거리를 구한다.
        playerDistanceX = (startPositionPlayer.x - player.position.x);
        playerDistanceY = (startPositionPlayer.y - player.position.y);

        for (int i = 0; i < backgrounds.Length; i++)
        {
            comparePosX = backgrounds[i].position.x + playerDistanceX * ((i + 1) * speed);
            comparePosY = backgrounds[i].position.y + playerDistanceY * ((i + 1) * speed);

            comparePos = new Vector3(comparePosX, comparePosY, 5.0f);

            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, comparePos, smooth * Time.deltaTime);
        }
        startPositionPlayer = player.position;
    }

}
