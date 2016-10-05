using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PushObject : MonoBehaviour {

    private RectTransform recTr;
    public Image img;
    public Canvas canvas;

    // Use this for initialization
    void Start () {
        recTr = canvas.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 pos = Vector2.zero;
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, this.transform.position);
        RectTransform canvasRec = canvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRec, screenPos, Camera.main, out pos);
        img.rectTransform.localPosition = pos;

        //Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        //Vector2 WorldObject_ScreenPosition = new Vector2(
        //     ((ViewportPosition.x * recTr.sizeDelta.x) - (recTr.sizeDelta.x * 0.5f)),
        //     ((ViewportPosition.y * recTr.sizeDelta.y) - (recTr.sizeDelta.y * 0.5f)));
        //img.rectTransform.localPosition = WorldObject_ScreenPosition;
    }
}
