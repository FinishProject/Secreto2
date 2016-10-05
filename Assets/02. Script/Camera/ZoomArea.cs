using UnityEngine;
using System.Collections;

public class ZoomArea : MonoBehaviour {
    public ZoomState zoomState;
    void Start()
    {
        zoomState.areaSize = transform.localScale.x;
        zoomState.areaX = transform.position.x - (zoomState.areaSize * 0.5f);
    }
}
