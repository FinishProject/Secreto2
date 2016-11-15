using UnityEngine;
using System.Collections;

public class ShotFireball : MonoBehaviour {

    private Vector3 targetPos;
    private float speed = 8f;

    private AudioSource source;
    
    void Start()
    {
        source = GetComponent<AudioSource>();
    }
    
    IEnumerator Move()
    {
        float moveSpeed = 0f;
        float time = 0f;

        Quaternion targetRot = Quaternion.LookRotation(targetPos - transform.position, Vector3.forward);
        targetRot.x = 0f;
        targetRot.y = 0f;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 100f * Time.deltaTime);

        

        while (true)
        {
            
            moveSpeed += speed * Time.deltaTime;
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
            //transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
            time += Time.deltaTime;

            if (time >= 5f)
            {
                //source.Stop();
                gameObject.SetActive(false);
            }

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
        source.Play();
        StartCoroutine(Move());
        
    }
}
