

using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    private Transform _mainCameraTransf = null;
	public bool LockX = false;
	public bool LockY = true;
	public bool LockZ = false;

	float xlock = 0.0f;
	float ylock = 0.0f;
	float zlock = 0.0f;

    void Start()
    {
		if( Camera.main != null )
		{
			_mainCameraTransf = Camera.main.transform;
		}
		else
		{
			GameObject goMainCamera = GameObject.FindWithTag("MainCamera");
			if (goMainCamera != null)
				_mainCameraTransf = goMainCamera.transform;
		}


    }

    void Update()
    {
		if (_mainCameraTransf != null)
        {

			if (LockX == false) {
				xlock = transform.eulerAngles.x;
			} else {
				xlock =  -_mainCameraTransf.eulerAngles.x;
			}
			if (LockY == false) {
				ylock = transform.eulerAngles.y;
			} else {
				ylock =  _mainCameraTransf.eulerAngles.y;
			}
			if (LockZ == false) {
				zlock = transform.eulerAngles.z;
			} else {
				zlock =  _mainCameraTransf.eulerAngles.z;
			}
//            transform.eulerAngles = new Vector3(transform.eulerAngles.x,
//											 _mainCameraTransf.eulerAngles.y,
//                                             transform.eulerAngles.z);

			transform.eulerAngles = new Vector3(xlock,ylock,zlock);
        }
    }
}
