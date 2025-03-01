using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public float sensitivity = 100f;

    [Header("Smoothing Settings")]
    public bool enableSmoothing = true;
    public float smoothing = 5f;

    [Header("Look Boundaries")]
    public Vector2 xClamp = new Vector2(-90f, 90f);
    public Vector2 zClamp = new Vector2(-45f, 45f);

    [Header("Target Points")]
    public Vector2 barTarget = new Vector2(0f, 0f);
    public Vector2 knifeTarget = new Vector2(0f, 0f);
    public Vector2 opponentTarget = new Vector2(0f, 0f);
    public Vector2 positionRotationTarget = new Vector2(0f, 0f);

    [Header("Camera Position Targets")]
    public Transform positionTarget;
    public Transform knifeTargetPoint;

    [Header("Lerp Settings")]
    public float positionLerpSpeed = 1f;
    public float rotationLerpSpeed = 1f;
    public float positionLerpDuration = 1f;

    public bool cameraLocked = false; // Added to lock/unlock camera movement

    private Vector2 smoothedVelocity;
    private Vector2 currentLookingPos;
    public Vector2 targetLookingPos;
    private bool isRotatingToTarget = false;
    private bool isMovementUnlocked = true;
    private bool isInNewPosition = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Camera mainCamera;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        targetLookingPos = currentLookingPos;

        originalPosition = transform.position;
        originalRotation = transform.rotation;

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!cameraLocked) // Prevent movement if locked
        {
            if (isRotatingToTarget)
            {
                HandleTargetTransition();
            }
            else if (isMovementUnlocked)
            {
                HandleFreeMovement();
            }
        }

        CheckForKeyPresses();
    }

    private void CheckForKeyPresses()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetCameraToBarTarget();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetCameraToKnifeTarget();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetCameraToOpponentTarget();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetCameraToPositionTarget();
        }
    }

    private void HandleFreeMovement()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseZ = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        if (enableSmoothing)
        {
            smoothedVelocity.x = Mathf.Lerp(smoothedVelocity.x, mouseX, 1f / smoothing);
            smoothedVelocity.y = Mathf.Lerp(smoothedVelocity.y, mouseZ, 1f / smoothing);
            currentLookingPos.x += smoothedVelocity.x;
            currentLookingPos.y -= smoothedVelocity.y;
        }
        else
        {
            currentLookingPos.x += mouseX;
            currentLookingPos.y -= mouseZ;
        }

        currentLookingPos.x = Mathf.Clamp(currentLookingPos.x, xClamp.x, xClamp.y);
        currentLookingPos.y = Mathf.Clamp(currentLookingPos.y, zClamp.x, zClamp.y);

        transform.localRotation = Quaternion.Euler(currentLookingPos.y, currentLookingPos.x, 0f);
    }

    private void HandleTargetTransition()
    {
        currentLookingPos = Vector2.Lerp(currentLookingPos, targetLookingPos, Time.deltaTime * rotationLerpSpeed);
        transform.localRotation = Quaternion.Euler(currentLookingPos.y, currentLookingPos.x, 0f);

        if (Vector2.Distance(currentLookingPos, targetLookingPos) < 0.1f)
        {
            isRotatingToTarget = false;

            if (targetLookingPos == knifeTarget)
            {
                cameraLocked = true; // Lock camera ONLY after the knife transition
            }
            else if (targetLookingPos == opponentTarget)
            {
                isMovementUnlocked = true;
                if (isInNewPosition)
                {
                    MoveCamera(originalPosition);
                    isInNewPosition = false;
                }
            }
        }
    }


    public void SetCameraToBarTarget()
    {
        SetCameraTarget(barTarget);
    }

    public void SetCameraToKnifeTarget()
    {
        MoveCamera(knifeTargetPoint.position);
        SetCameraTarget(knifeTarget);
        isInNewPosition = true;
    }

    public void SetCameraToOpponentTarget()
    {
        cameraLocked = false;
        MoveCamera(originalPosition);
        SetCameraTarget(opponentTarget);
        isInNewPosition = false;
    }

    public void SetCameraToPositionTarget()
    {
        MoveCamera(positionTarget.position);
        SetCameraTarget(positionRotationTarget);
        isInNewPosition = true;
    }

    private void SetCameraTarget(Vector2 target)
    {
        isRotatingToTarget = true;
        isMovementUnlocked = false;
        targetLookingPos = target;
    }

    private void MoveCamera(Vector3 newPosition)
    {
        StartCoroutine(LerpCameraPosition(newPosition));
    }

    private IEnumerator LerpCameraPosition(Vector3 newPosition)
    {
        float timeElapsed = 0f;
        Vector3 initialPosition = transform.position;

        while (timeElapsed < positionLerpDuration)
        {
            float easedTime = Mathf.SmoothStep(0f, 1f, timeElapsed / positionLerpDuration);
            transform.position = Vector3.Lerp(initialPosition, newPosition, easedTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = newPosition;
    }
}
