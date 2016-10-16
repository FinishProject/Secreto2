using UnityEngine;
using System.Collections;

public class ShotRazorObj : MonoBehaviour {

    public float maxLength = 30f;
    private float fadeSpeed = 1f;
    private float interValue = 0.12f;

    public GameObject startObj;
    public GameObject lazerObj;
    public Transform startPoint;

    public GameObject lazerMat;
    private Vector3 shotPoint;

    private float fadeDir = -1f;
    private float alpha = 0f;

    void Start()
    {
        shotPoint = startPoint.position;
        shotPoint.x += 0.5f;
        startObj.transform.position = shotPoint;

        Vector3 scale = lazerObj.transform.localScale;
        // 보간 값을 곱하여 레이저의 길이를 조절한다.
        scale.x = maxLength * interValue;
        lazerObj.transform.localScale = scale;
        startObj.transform.position = startPoint.position;

        StartCoroutine(SetLazer());

    }

	void Update () {
        if(alpha == 1f)
            ShotRay();
	}
    

    IEnumerator SetLazer()
    {
        Renderer meshRender = lazerMat.GetComponent<Renderer>();
        Color setColor = meshRender.material.color;

        while (true)
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            setColor.a = alpha;

            meshRender.material.color = setColor;

            if (alpha == 0f || alpha == 1f)
            {
                fadeDir *= -1f;
                yield return new WaitForSeconds(5f);
            }

            if (fadeDir == 1f)
                fadeSpeed = 0.3f;
            else
                fadeSpeed = 2f;

            yield return null;
        }
    }

    void ShotRay()
    {
        RaycastHit hit;
        // 발사할 방향을 로컬 좌표에서 월드 좌표로 변환한다.
        Vector3 forward = transform.TransformDirection(Vector3.right);

        Debug.DrawRay(startPoint.position, forward, Color.red, 5f);

        if (Physics.Raycast(startPoint.position, forward, out hit, maxLength))
        {
            if (hit.collider.CompareTag("Player") && alpha == 1f)
            {
                PlayerCtrl.instance.PlayerDie();
            }
            else if (hit.collider.CompareTag("Land"))
            {
                //레이저 크기를 레이캐스트 충돌 위치와의 거리를 구하여 크기를 변경
                Vector3 scale = lazerObj.transform.localScale;
                //보간 값을 곱하여 레이저의 길이를 조절한다.
                scale.x = hit.distance * interValue;
                lazerObj.transform.localScale = scale;
                startObj.transform.position = startPoint.position;
            }

        }
    }
}
