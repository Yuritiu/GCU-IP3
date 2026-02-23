using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerCameraController : NetworkBehaviour
{
    [Header("Camera Follow Settings")]
    public float followMultiplier = 3f;           // Camera follow speed
    public float maxRotationOffset = 30f;         // Max camera rotation offset from center

    [Header("Sensitivity Settings")]
    public float sensitivity = 100f;
    public float sensitivityMultiplier = 10f;

    [Header("Crosshair UI")]
    public RectTransform crosshair;
    public float crosshairClampMargin = 100f;

    [Header("Look Boundaries")]
    public Vector2 xClamp = new Vector2(-115f, 115f);
    public Vector2 zClamp = new Vector2(-75f, 75f);

    public bool cameraLocked = false;
    public bool isMovementUnlocked = true;

    private Vector2 currentLookingPos;
    private Vector2 screenCenter;

    private Camera playerCamera;

    public override void OnNetworkSpawn()
    {
        playerCamera = GetComponent<Camera>();

        if (!IsOwner)
        {
            // Only disable the visual component, not the full object
            if (crosshair != null)
                crosshair.gameObject.SetActive(false);

            if (playerCamera != null)
            {
                playerCamera.enabled = false;
                if (playerCamera.TryGetComponent(out AudioListener listener))
                    listener.enabled = false;
            }

            enabled = false;
            return;
        }

        playerCamera.enabled = true;
        playerCamera.tag = "MainCamera";

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        LoadSettings();

        Vector3 forward = transform.forward;
        float initialYaw = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

        currentLookingPos.x = initialYaw;
        currentLookingPos.y = 0f;

        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        if (crosshair != null)
            crosshair.anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        if (!IsOwner || !isMovementUnlocked || cameraLocked) return;

        HandleCrosshairMovement();
        HandleCameraRotationFromCrosshair();
    }

    void HandleCrosshairMovement()
    {
        if (crosshair == null) return;

        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        Vector2 mouseDelta = new Vector2(mouseX, mouseY) * sensitivityMultiplier;
        crosshair.anchoredPosition += mouseDelta;

        //Clamp Crosshair Inside Screen Bounds
        float maxX = screenCenter.x - crosshairClampMargin;
        float maxY = screenCenter.y - crosshairClampMargin;

        crosshair.anchoredPosition = new Vector2(Mathf.Clamp(crosshair.anchoredPosition.x, -maxX, maxX),Mathf.Clamp(crosshair.anchoredPosition.y, -maxY, maxY));
    }

    void HandleCameraRotationFromCrosshair()
    {
        if (crosshair == null) return;

        Vector2 offsetFromCenter = crosshair.anchoredPosition / screenCenter;

        currentLookingPos.x = Mathf.Lerp(currentLookingPos.x, offsetFromCenter.x * maxRotationOffset, Time.deltaTime * followMultiplier);
        currentLookingPos.y = Mathf.Lerp(currentLookingPos.y, -offsetFromCenter.y * maxRotationOffset, Time.deltaTime * followMultiplier);

        currentLookingPos.x = Mathf.Clamp(currentLookingPos.x, xClamp.x, xClamp.y);
        currentLookingPos.y = Mathf.Clamp(currentLookingPos.y, zClamp.x, zClamp.y);

        transform.localRotation = Quaternion.Euler(currentLookingPos.y, currentLookingPos.x, 0f);
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("Sensitivity"))
            sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        Debug.Log("RETRUEVED SENSITIVITY: " + sensitivity);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        PlayerPrefs.Save();
    }
}