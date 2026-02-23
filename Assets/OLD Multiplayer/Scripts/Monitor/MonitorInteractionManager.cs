using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MonitorInteractionManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject monitorUI;
    [SerializeField] public MonitorMouseController mouseController;
    [SerializeField] public MultiplayerCameraController multiplayerCameraController;
    [SerializeField] public Camera playerCamera;
    [SerializeField] public Transform playerTransform;

    [Header("View Change Settings")]
    [SerializeField] public float exitRaiseYAmount = 0.5f;
    [SerializeField] public float enterFOV = 35f;
    [SerializeField] public float exitFOV = 60f;
    [SerializeField] public float lerpDuration = 0.3f;

    [Header("View Change Variables")]
    private Vector3 initialPosition;
    private float initialFOV;

    [Header("Coroutines")]
    private Coroutine positionLerpCoroutine;
    private Coroutine fovLerpCoroutine;
    private Quaternion initialCameraRotation;
    private Coroutine rotationLerpCoroutine;

    private bool inMonitor = false;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        initialCameraRotation = playerCamera.transform.rotation;
        initialPosition = playerTransform.position;
        initialFOV = playerCamera.fieldOfView;

        EnterMonitorModeInstant();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (!inMonitor && Input.GetMouseButtonDown(0))
        {
            EnterMonitorMode();
        }
    }

    public void EnterMonitorModeInstant()
    {
        inMonitor = true;

        StopAllLerps();

        //Reset Position & FOV Instantly
        SetPlayerY(initialPosition.y);
        playerCamera.fieldOfView = enterFOV;

        multiplayerCameraController.crosshair.gameObject.SetActive(false);
        mouseController.EnableMouse();
        multiplayerCameraController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    void EnterMonitorMode()
    {
        inMonitor = true;

        //-------------------------- Lerping Behaviour --------------------------
        StopAllLerps();

        positionLerpCoroutine = StartCoroutine(LerpPlayerY(playerTransform.position.y, initialPosition.y, lerpDuration));
        fovLerpCoroutine = StartCoroutine(LerpCameraFOV(playerCamera.fieldOfView, enterFOV, lerpDuration));
        rotationLerpCoroutine = StartCoroutine(LerpCameraRotation(playerCamera.transform.rotation, initialCameraRotation, lerpDuration));
        //------------------------------------------------------------------------

        multiplayerCameraController.crosshair.gameObject.SetActive(false);
        mouseController.EnableMouse();
        multiplayerCameraController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    public void ExitMonitorMode()
    {
        inMonitor = false;

        //-------------------------- Lerping Behaviour --------------------------
        StopAllLerps();

        float targetY = initialPosition.y + exitRaiseYAmount;

        positionLerpCoroutine = StartCoroutine(LerpPlayerY(playerTransform.position.y, targetY, lerpDuration));
        fovLerpCoroutine = StartCoroutine(LerpCameraFOV(playerCamera.fieldOfView, exitFOV, lerpDuration));
        //------------------------------------------------------------------------

        multiplayerCameraController.crosshair.gameObject.SetActive(true);
        mouseController.DisableMouse();
        multiplayerCameraController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    #region Lerping Functions
    private IEnumerator LerpPlayerY(float fromY, float toY, float duration)
    {
        float elapsed = 0f;
        Vector3 pos = playerTransform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newY = Mathf.Lerp(fromY, toY, elapsed / duration);
            SetPlayerY(newY);
            yield return null;
        }

        SetPlayerY(toY);
        positionLerpCoroutine = null;
    }

    private IEnumerator LerpCameraRotation(Quaternion from, Quaternion to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            playerCamera.transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            yield return null;
        }

        playerCamera.transform.rotation = to;
        rotationLerpCoroutine = null;
    }

    private IEnumerator LerpCameraFOV(float fromFOV, float toFOV, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            playerCamera.fieldOfView = Mathf.Lerp(fromFOV, toFOV, elapsed / duration);
            yield return null;
        }

        playerCamera.fieldOfView = toFOV;
        fovLerpCoroutine = null;
    }

    private void SetPlayerY(float y)
    {
        Vector3 pos = playerTransform.position;
        pos.y = y;
        playerTransform.position = pos;
    }

    private void StopAllLerps()
    {
        if (positionLerpCoroutine != null)
            StopCoroutine(positionLerpCoroutine);

        if (fovLerpCoroutine != null)
            StopCoroutine(fovLerpCoroutine);

        if (rotationLerpCoroutine != null)
            StopCoroutine(rotationLerpCoroutine);

        positionLerpCoroutine = null;
        fovLerpCoroutine = null;
        rotationLerpCoroutine = null;
    }

    #endregion
}