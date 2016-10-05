using UnityEngine;
using System.Collections;

public class WindGrass : MonoBehaviour {

    private Mesh mesh;
    private Vector3[] verts_ref;
    private Vector3[] verts;
    float time_offset;

    public float x_tweak = 0.060f;
    public float y_tweak = 0.0085f;

    public float max_X = 0.4f;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        verts_ref = mesh.vertices;
        verts = new Vector3[verts_ref.Length];
        for(int i=0; i<verts_ref.Length; i++)
        {
            verts[i] = verts_ref[i];
        }
        //System.Array.Copy(verts_ref, verts, verts_ref.Length);
        time_offset = Random.Range(0, 10000);
    }

    void OnBecameInvisible()
    {
        enabled = false;
    }

    void OnBecameVisible()
    {
        enabled = true;
    }

    void Update()
    {
        float time = time_offset + Time.time;

        float valX = Mathf.Sin(time);
        float valY = Mathf.Cos(time);
        float k = 1.0f;

        for (int i = (int)(verts.Length * max_X); i < verts.Length; ++i)
        {
            Vector3 v = verts_ref[i];

            if (i % 5 == 0) k = 1.0f;
            if (i % 5 == 1) k = 1.2f;

            v.x = v.x + k * x_tweak * valX;
            v.y = v.y + k * y_tweak * valY;
            verts[i] = v;
        }
        mesh.vertices = verts;
    }
}
