using UnityEngine;
using System.Collections;


public class Test : MonoBehaviour {
    [System.Serializable]
    public enum Dir
    {
        up, down, left, right,
    }
    public Dir shootDir;
    public float rate = 10.53f;
    Vector3 oriPos , oriScal;
    void Start()
    {
        oriPos = transform.position;
        oriScal = transform.localScale;

        if (shootDir == Dir.up)
            transform.Rotate(new Vector3(0, 0, 1), 90);
        if (shootDir == Dir.down)
        {
            transform.Rotate(new Vector3(0, 0, 1), 270);
 //           oriPos.x = (tempScale.x * 0.5f * rate);
        }
            
        if (shootDir == Dir.left)
            transform.Rotate(new Vector3(0, 1, 0), 180);
        

    }
    void Update()
    {
        Shootlazer();
    }

    void Shootlazer()
    {
        Vector3 tempScale = transform.localScale;
        Vector3 tempPos = transform.position;

        switch(shootDir)
        {
            case Dir.up:
                tempScale.x += 0.02f;
                tempPos.y = oriPos.y + (tempScale.x * 0.5f * rate) - (oriScal.x * 0.5f * rate);
                transform.localScale = tempScale;
                transform.position = tempPos;
                break;
            case Dir.down:
                tempScale.x += 0.02f;
                tempPos.y = oriPos.y - (tempScale.x * 0.5f * rate) - (oriScal.x * 0.5f * rate);
                transform.localScale = tempScale;
                transform.position = tempPos;
                break;
            case Dir.left:
                tempScale.x += 0.02f;
                tempPos.x = oriPos.x - (tempScale.x * 0.5f * rate) - (oriScal.x * 0.5f * rate);
                transform.localScale = tempScale;
                transform.position = tempPos;
                break;

            case Dir.right:
                tempScale.x += 0.02f;
                tempPos.x = oriPos.x + (tempScale.x * 0.5f * rate) - (oriScal.x * 0.5f * rate);
                transform.localScale = tempScale;
                transform.position = tempPos;
                break;
        }

        
    }

    
}
