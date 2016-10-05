using UnityEngine;
using System.Collections;

public class MouseMute : MonoBehaviour {

    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
