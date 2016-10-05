using UnityEngine;
using System.Collections;

public class DownWall : MonoBehaviour {

    public float downSpeed = 5f;
    public float upSpeed = 2f;
    public float waitTime = 1f;
    public float maxLength = 5f;

    private bool isDown = false;

    private Vector3 originPos, finishPos;

    void Start()
    {
        originPos = this.transform.position;
        finishPos = originPos;
        finishPos.y += maxLength;

        transform.position = finishPos;
    }

    void Update()
    {
        // 아래 목표 위치 도달 시
        if(transform.position.y - 0.1f <= originPos.y)
        {
            StartCoroutine(WaitMove(false));
        }
        // 위 목표 위치 도달 시
        else if(transform.position.y + 0.1f >= finishPos.y)
        {
            StartCoroutine(WaitMove(true));
        }
        // 아래로 이동
        if (isDown)
        {
            transform.position = Vector3.Lerp(transform.position, originPos, downSpeed * Time.deltaTime);
        }
        // 위로 이동
        else if (!isDown)
        { 
            transform.position = Vector3.Lerp(transform.position, finishPos, upSpeed * Time.deltaTime);
        }
    }

    IEnumerator WaitMove(bool isUp)
    {
        yield return new WaitForSeconds(waitTime);
        if(isUp) { isDown = true; }
        else if(!isUp) { isDown = false; }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerCtrl.instance.getDamage(200f);
        }
    }


    //void Start()
    //{
    //    originPos = this.transform.position;
    //    finishPos = originPos;
    //    finishPos.y = originPos.y + maxHeight;

    //    if (isUp)
    //        transform.position = finishPos;
    //}

    //void Update()
    //{
    //    if (transform.position.y <= originPos.y && speed < -1f) StartCoroutine("ChangeDirection");
    //    else if (transform.position.y >= finishPos.y && speed > 1f) StartCoroutine("ChangeDirection");

    //    if (isMove)
    //        transform.Translate(Vector3.up * speed * Time.deltaTime);
    //}

    //IEnumerator ChangeDirection()
    //{
    //    isMove = false;
    //    yield return new WaitForSeconds(2f);
    //    speed *= -1f;
    //    isMove = true;
    //    StopCoroutine("ChangeDirection");
    //}

 //   public GameObject[] wallObj = new GameObject[4]; // 벽 오브젝트
 //   public float speed = 5f;

 //   private int count = 0;
 //   private Vector3[] originPos, maxPos; // 초기 위치, 최고 높이 위치

 //   void Start()
 //   {
 //       originPos = new Vector3[wallObj.Length];
 //       maxPos = new Vector3[wallObj.Length];
 //       // 벽 오브젝트들의 위치를 maxPos의 위치로 이동
 //       for (int i = 0; i < wallObj.Length; i++)
 //       {
 //           originPos[i] = wallObj[i].gameObject.transform.position;
 //           Vector3 pos = wallObj[i].gameObject.transform.position;
 //           maxPos[i] = new Vector3(pos.x, pos.y + 5f, 0f);
 //           wallObj[i].transform.position = maxPos[i];
 //       }
 //   }
	//void Update () {
 //       //벽 오브젝트 순차적으로 초기 위치까지 아래로 이동
	//    if(wallObj[count].transform.position.y >= originPos[count].y)
 //       {
 //           wallObj[count].transform.Translate(Vector3.down * speed * Time.deltaTime);
 //       }
 //       else {
 //           // 순차적으로 벽 오브젝트 변경
 //           if (count < wallObj.Length)
 //               StartCoroutine("NextCount");
 //           // 마지막 벽이 내려가면 처음 벽부터 위로 올림
 //           if (count >= wallObj.Length - 1)
 //               StartCoroutine("UpWall");
 //       }
	//}
 //   //다음 벽 오브젝트 선택
 //   IEnumerator NextCount()
 //   {
 //       yield return new WaitForSeconds(0.2f);
 //       if (count < wallObj.Length -1)
 //           count++;
 //       StopCoroutine("NextCount");
 //   }
 //   //처음 벽 오브젝트부터 순차적으로 위로 다시 이동
 //   IEnumerator UpWall()
 //   {
 //       bool isActive = true;
 //       int cnt = 0;
 //       while (isActive)
 //       {
 //           if (wallObj[cnt].transform.position.y <= maxPos[cnt].y)
 //               wallObj[cnt].transform.Translate(Vector3.up * 0.1f * Time.deltaTime);
 //           else if (wallObj[cnt].transform.position.y >= maxPos[cnt].y && cnt < wallObj.Length - 1)
 //               cnt++;
 //           else
 //               isActive = false;

 //           yield return null;
 //       }
 //       count = 0; // 다시 처음부터 아래로 내리기 위해 카운트를 0으로 함.
 //       StopCoroutine("UpWall");
 //   }
}
