using UnityEngine;
using System.Collections;

public class ShotRazorObj : MonoBehaviour {

    public float maxLength = 30f;
    private float interValue = 0.115f;

    public GameObject startObj;
    public GameObject lazerObj;
    public Transform startPoint;

    private Vector3 shotPoint;

    void Start()
    {
        shotPoint = startPoint.position;
        shotPoint.x += 0.5f;
        startObj.transform.position = shotPoint;
    }

	void Update () {
        ShotRay();
	}

    void ShotRay()
    {
        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.right);

        if (Physics.Raycast(startPoint.position, forward, out hit, maxLength))
        {
            if (hit.collider.CompareTag("Player"))
            {
                PlayerCtrl.instance.PlayerDie();
            }

            // 레이저 크기를 레이캐스트 충돌 위치와의 거리를 구하여 크기를 변경
            Vector3 scale = lazerObj.transform.localScale;   
            scale.x = hit.distance * interValue;
            lazerObj.transform.localScale = scale;
            startObj.transform.position = startPoint.position;
        }
    }
}
