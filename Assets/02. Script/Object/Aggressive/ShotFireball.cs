using UnityEngine;
using System.Collections;

public class ShotFireball : MonoBehaviour {

    private Vector3 targetPos;
    public float speed = 2f;
    
    IEnumerator Move()
    {
        float moveSpeed = 0f;
        while (true)
        {
            moveSpeed += speed * Time.deltaTime;

            Quaternion targetRot = Quaternion.LookRotation(targetPos - transform.position, Vector3.forward);
            targetRot.x = 0f;
            targetRot.y = 0f;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 5f * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);

            yield return null;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Land") || col.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }


    public void GetTarget(Vector3 _targetPos)
    {
        targetPos = _targetPos;
        StartCoroutine(Move());
    }
}
