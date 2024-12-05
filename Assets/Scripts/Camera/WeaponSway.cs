using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float swayMultiplier;

    private void Update()
    {
        float MouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float MouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;
        

        Quaternion rotationX = Quaternion.AngleAxis(-MouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(MouseX, Vector3.up);
        

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
