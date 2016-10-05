using UnityEngine;
using System.Collections;

public class MapUI : MonoBehaviour {
    public GameObject worldArea;
    public GameObject playerImage;
    public GameObject mapImage;
    private Transform playerTr;

    // 보다 정확한 소수점을 알기 위해 Vector2 대신 구조체를 선언함
    struct XY
    {
        public float x;
        public float y;
    }

    XY ratio;
    XY playerPos;
    XY playerMap;
    Rect worldAreaRect;
    Rect mapRect;
    Rect playerRect;

    void Awake()
    {
        mapRect = mapImage.GetComponent<RectTransform>().rect;
        playerRect = playerImage.GetComponent<RectTransform>().rect;
        playerTr = PlayerCtrl.instance.transform;
    }

    void OnEnable() {
        TotalRatio();
        PlayerImgPosApply();
        MapPosUpdate();
        Debug.Log(transform.position);
        Debug.Log(playerImage.transform.position);
    }

    // 월드 구역(collider)안에서의 플레이어 위치를 비율로 변환
    void TotalRatio()
    {
        // 구역 위치,크기를 불러와 정보를 Rect안에 넣음
        Vector2 tempAreaPos = worldArea.transform.position;   
        Vector2 tempAreaScl = worldArea.transform.localScale;
        worldAreaRect.Set(
            (float)(tempAreaPos.x - (tempAreaScl.x * 0.5)),
            (float)(tempAreaPos.y - (tempAreaScl.y * 0.5)),
            tempAreaScl.x,
            tempAreaScl.y
            );

        // 플레이어 위치
        playerPos.x = playerTr.position.x;
        playerPos.y = playerTr.position.y;

        // Rect의 Left,Top와 플레이어와의 거리
        XY playerToAreaRange;
        playerToAreaRange.x = playerPos.x - worldAreaRect.xMin;
        playerToAreaRange.y = worldAreaRect.yMax - playerPos.y;

        // 거리를 비율로 변환
        ratio.x = playerToAreaRange.x / tempAreaScl.x;
        ratio.y = playerToAreaRange.y / tempAreaScl.y;
    }

    // 지도상의 플레이어 위치를 적용
    void PlayerImgPosApply()
    {
        // 비율을 받아와 지도의 크기 비례 거리를 구함
        XY ratioToRange;
        ratioToRange.x = mapRect.width * ratio.x;
        ratioToRange.y = mapRect.height * ratio.y;

        // 플레이어 위치(지도상 위치)를 지정해줌
        Vector3 playerImgPos = Vector3.zero;
        playerImgPos.x = ratioToRange.x - (playerRect.width * 0.5f);
        playerImgPos.y = (mapRect.height * 0.5f) - ratioToRange.y + playerRect.height;
        playerImage.transform.localPosition = playerImgPos;

    }

    // 맵의 위치 조절 (플레이어를 중앙에 위치하도록 하기 위해)
    void MapPosUpdate()
    {
        // 플레이어의 위치가(지도상의 위치) 화면 중앙을 넘었을 때
        if(transform.position.x < playerImage.transform.position.x)
        {
            Vector3 temp = mapImage.transform.position;
            temp.x -= playerImage.transform.position.x - transform.position.x;
            mapImage.transform.position = temp;
        }
    }
}
