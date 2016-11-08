using UnityEngine;
using System.Collections;

[System.Serializable]
public struct ZoomState
{
    [System.NonSerialized]
    public float areaX;                 // 줌 구역 시작 X좌표 위치
    [System.NonSerialized]
    public float areaSize;              // 줌 구역 길이
    public float deep;                  // 줌 깊이
    public float height;                // 줌 높이
    public float startRangePercent;     // 줌인, 아웃 시작 비율
}

public class ZoomArea : MonoBehaviour {
    public ZoomState zoomState;
    void Start()
    {
        zoomState.areaSize = transform.localScale.x;
        zoomState.areaX = transform.position.x - (zoomState.areaSize * 0.5f);
    }
}
