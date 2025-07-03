using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerCameraController : NetworkBehaviour
{
    [Header("Sensitivity Settings")]
    public float sensitivity = 100f;

    [Header("Smoothing Settings")]
    //public bool enableSmoothing = true;
    //public float smoothing = 3f;

    [Header("Look Boundaries")]
    public Vector2 xClamp = new Vector2(-115f, 115f);
    public Vector2 zClamp = new Vector2(-75f, 75f);
    private Vector2 originalXClamp;
    private Vector2 originalZClamp;

    public bool cameraLocked = false;

    private Vector2 smoothedVelocity;
    private Vector2 currentLookingPos;
    public bool isMovementUnlocked = true;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = GetComponent<Camera>();

        if (!IsOwner)
        {
            //Disable Camera For Non Owners
            if (playerCamera != null)
            {
                playerCamera.enabled = false;
                AudioListener listener = playerCamera.GetComponent<AudioListener>();
                if (listener != null) listener.enabled = false;
            }

            this.enabled = false;
            return;
        }

        //Set as MainCamera Only For The Owner
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
            playerCamera.tag = "MainCamera";
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //------------ Camera Logic START ------------
        //Make Camera Look in The Direction The Player is Facing
        Vector3 forward = transform.forward;
        float initialYaw = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

        currentLookingPos.x = initialYaw;
        currentLookingPos.y = 0f;

        transform.localRotation = Quaternion.Euler(currentLookingPos.y, currentLookingPos.x, 0f);

        originalPosition = transform.position;
        originalRotation = transform.rotation;

        originalXClamp = xClamp;
        originalZClamp = zClamp;
        //------------ Camera Logic END ------------
    }

    void Update()
    {
        if (!IsOwner || !isMovementUnlocked) return;

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