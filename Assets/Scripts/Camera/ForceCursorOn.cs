using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceCursorOn : MonoBehaviour
{
    void Update()
    {
        if (!Cursor.visible || Cursor.lockState != CursorLockMode.None)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
