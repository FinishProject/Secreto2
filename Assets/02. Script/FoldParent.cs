using UnityEngine;
using System.Collections;

public class FoldParent : MonoBehaviour {

    private FoldHold[] fold;

	// Use this for initialization
	void Start () {
        fold = gameObject.GetComponentsInChildren<FoldHold>();
	}
	
	//void OnTriggerEnter(Collider col)
 //   {
 //       if (col.CompareTag("Player"))
 //       {
 //           for(int i=0; i<fold.Length; i++)
 //           {
 //               fold[i].StartMove();
 //           }
 //       }
 //   }

 //   void OnTriggerExit(Collider col)
 //   {
 //       if (col.CompareTag("Player"))
 //       {
 //           for (int i = 0; i < fold.Length; i++)
 //           {
 //               fold[i].isActive = false;
 //           }
 //       }
 //   }
}
