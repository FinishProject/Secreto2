using UnityEngine;
using System.Collections;

public enum Dir
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
}

public class BulletObject_FourWay : MonoBehaviour
{
    Transform thisTr;
    Vector3 moveVector;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Player"))
        {
            PlayerCtrl.instance.getDamage(20);
        }
        else if(col.tag.Equals("MONSTER"))
        {
            return;
        }
        gameObject.SetActive(false);

    }

    public IEnumerator Moveing(Transform startTr, Dir moveDir, float speed )
    {
        StartCoroutine(OffActive());
        gameObject.transform.position = startTr.position;
        thisTr = gameObject.transform;
        moveVector = startTr.position;


        while (true)
        {
            if (!gameObject.activeSelf)
                break;

            switch (moveDir)
            {
                case Dir.LEFT:
                    moveVector.x = thisTr.position.x + (-1 * speed * Time.deltaTime);
                    break;
                case Dir.RIGHT:
                    moveVector.x = thisTr.position.x + (speed * Time.deltaTime);
                    break;
                case Dir.UP:
                    moveVector.y = thisTr.position.y + (speed * Time.deltaTime);
                    break;
                case Dir.DOWN:
                    moveVector.y = thisTr.position.y + (-1 * speed * Time.deltaTime);
                    break;
            }

            thisTr.position = moveVector;

            yield return null;
        }
    }

    IEnumerator OffActive()
    {
        yield return new WaitForSeconds(5);
        gameObject.SetActive(false);
    }
}