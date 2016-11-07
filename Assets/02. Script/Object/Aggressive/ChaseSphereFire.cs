using UnityEngine;
using System.Collections;

public class ChaseSphereFire : MonoBehaviour {

    public Transform fireBall;

    private Transform playerTr;

    void Start()
    {
        playerTr = PlayerCtrl.instance.transform;
        StartCoroutine(ShotFire());
    }

    IEnumerator ShotFire()
    {
        Vector3 targetPos = Vector3.zero;
        while (true)
        {
            if(!fireBall.gameObject.activeSelf)
            {
                fireBall.gameObject.SetActive(true);
                fireBall.position = this.transform.position;
                targetPos = playerTr.position;
            }

            fireBall.position = Vector3.Lerp(fireBall.position, targetPos, 1f * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(targetPos - fireBall.position, Vector3.forward);
            targetRot.x = 0f;
            targetRot.y = 0f;

            fireBall.rotation = Quaternion.Slerp(fireBall.rotation, targetRot, 3f * Time.deltaTime);

            yield return null;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Land"))
        {
            fireBall.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider col)
    {
       
        if (col.CompareTag("Land"))
            Debug.Log("11");
        //fireBall.gameObject.SetActive(false);
    }
}
