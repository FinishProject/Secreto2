using UnityEngine;
using System.Collections;

public class ObjectPosResetMgr : MonoBehaviour {

    Transform[] objs;
    Vector3[] objsOrignPos;
    Quaternion[] objsOrignRot;
    public static ObjectPosResetMgr instance;
	void Start () {
        instance = this;
        objs = GetChildObj<Transform>(gameObject);
        objsOrignPos = new Vector3[objs.Length];
        objsOrignRot = new Quaternion[objs.Length];
        for (int i = 0; i < objs.Length; i++)
        {
            objsOrignPos[i] = objs[i].position;
            objsOrignRot[i] = objs[i].rotation;
        }
            

    }

    public void ResetPos()
    {
        for (int i = 0; i < objs.Length; i++)
        {
            objs[i].position = objsOrignPos[i];
            objs[i].rotation = objsOrignRot[i];

        }
    }

    // 자식 오브젝트 받아 올 때 ( 자식이 없으면 자신을 반환 )
    T[] GetChildObj<T>(GameObject obj) where T : class
    {
        T[] tempObjs = obj.GetComponentsInChildren<T>();
        int arraySize = tempObjs.Length;
        arraySize = arraySize > 1 ? arraySize - 1 : arraySize;
        T[] objs = new T[arraySize];

        if (arraySize == 1)
            objs[0] = tempObjs[0];

        for (int i = 0; i < tempObjs.Length - 1; i++)
            objs[i] = tempObjs[i + 1];

        return objs as T[];
    }
}
