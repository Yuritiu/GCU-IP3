using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    [Header("Camera Positions")]
    public Transform initialCameraPosition;
    public Transform targetCameraPosition;

    [Header("Transition Settings")]
    public float transitionSpeed = 5f;
    public float mouseSensitivity = 2f;

    [Header("Camera Angle Limits (Freelook)")]
    public float transitionMinX = 85f;
    public float transitionMaxX = 95f;
    public float transitionMinY = 0f;
    public float transitionMaxY = 0f;

    [Header("Internal State Variables")]
    private Camera mainCamera;
    private bool transitioningToTarget = false;
    private Vector3 initialMousePosition;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    [Header("Freelook Script References")]
    private Freelook freeLookScript;
    private float originalMinX;
    private float originalMaxX;
    private float originalMinY;
    private float originalMaxY;

    private void Start()
    {
        mainCamera = Camera.main;
        initialMousePosition = Input.mousePosition;
        initialRotation = initialCameraPosition.rotation;
        targetRotation = targetCameraPosition.rotation;

        freeLookScript = mainCamera.GetComponent<Freelook>();
        if (freeLookScript != null)
        {
            originalMinX = freeLookScript.minX;
            originalMaxX = freeLookScript.maxX;
            originalMinY = freeLookScript.minY;
            originalMaxY = freeLookScript.maxY;
        }
    }

    private void Update()
    {
        HandleInput();
        UpdateCameraPositionAndRotation();
        SmoothMouseLook();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            transitioningToTarget = true;
            initialRotation = mainCamera.transform.rotation;
            targetRotation = targetCameraPosition.rotation;
            initialMousePosition = Input.mousePosition;

            if (freeLookScript != null)
            {
                freeLookScript.minX = transitionMinX;
                freeLookScript.maxX = transitionMaxX;
                freeLookScript.minY = transitionMinY;
                freeLookScript.maxY = transitionMaxY;
            }
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            transitioningToTarget = false;
            initialRotation = mainCamera.transform.rotation;
            targetRotation = initialCameraPosition.rotation;
            initialMousePosition = Input.mousePosition;

            if (freeLookScript != null)
            {
                freeLookScript.minX = originalMinX;
                freeLookScript.maxX = originalMaxX;
                freeLookScript.minY = originalMinY;
                freeLookScript.maxY = originalMaxY;
            }
        }
    }

    private void UpdateCameraPositionAndRotation()
    {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, transitioningToTarget ? targetCameraPosition.position : initialCameraPosition.position, Time.deltaTime * transitionSpeed);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, transitioningToTarget ? targetRotation : initialRotation, Time.deltaTime * transitionSpeed);
    }

    private void SmoothMouseLook()
    {
        Vector3 mouseOffset = Input.mousePosition - initialMousePosition;
        mouseOffset *= mouseSensitivity * Time.deltaTime;

        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, (transitioningToTarget ? targetRotation : initialRotation) * Quaternion.Euler(-mouseOffset.y, mouseOffset.x, 0), Time.deltaTime * transitionSpeed);
    }
}

