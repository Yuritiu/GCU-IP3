using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerCameraController : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public float sensitivity = 100f;

    [Header("Smoothing Settings")]
    //public bool enableSmoothing = true;
    //public float smoothing = 3f;

    [Header("Look Boundaries")]
    public Vector2 xClamp = new Vector2(-135f, 135f);
    public Vector2 zClamp = new Vector2(-75f, 75f);
    private Vector2 originalXClamp;
    private Vector2 originalZClamp;

    public bool cameraLocked = false;

    private Vector2 smoothedVelocity;
    private Vector2 currentLookingPos;
    public bool isMovementUnlocked = true;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Camera mainCamera;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalPosition = transform.position;
        originalRotation = transform.rotation;

        originalXClamp = xClamp;
        originalZClamp = zClamp;

        mainCamera = Camera.main;
    }

    void Update()
    {   
        if (!cameraLocked)
        {
            if (isMovementUnlocked)
            {
                HandleFreeMovement();
            }
        }
    }

    void HandleFreeMovement()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseZ = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        //if (enableSmoothing)
        //{
        //    smoothedVelocity.x = Mathf.Lerp(smoothedVelocity.x, mouseX, 1f / smoothing);
        //    smoothedVelocity.y = Mathf.Lerp(smoothedVelocity.y, mouseZ, 1f / smoothing);
        //    currentLookingPos.x += smoothedVelocity.x;
        //    currentLookingPos.y -= smoothedVelocity.y;
        //}
        //else
        //{
            currentLookingPos.x += mouseX;
            currentLookingPos.y -= mouseZ;
        //}

        currentLookingPos.x = Mathf.Clamp(currentLookingPos.x, xClamp.x, xClamp.y);
        currentLookingPos.y = Mathf.Clamp(currentLookingPos.y, zClamp.x, zClamp.y);

        transform.localRotation = Quaternion.Euler(currentLookingPos.y, currentLookingPos.x, 0f);
    }
}