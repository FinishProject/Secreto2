using UnityEngine;
using System.Collections;

public class CameraArea_3 : MonoBehaviour {

    [System.Serializable]
    public struct CorrectionValue
    {
        public float x, y, z;

        [System.NonSerialized]
        public float width;
    }
    public CorrectionValue correctionValue;

    void Start()
    {

    }

    static public CorrectionValue ZeroCorrectionValue()
    {
        CorrectionValue zero;
        zero.x = 0;
        zero.y = 0;
        zero.z = 0;
        zero.width = 0;
        return zero;
    }
}
