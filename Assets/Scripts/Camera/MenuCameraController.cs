using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    [Header("Target References")]
    public Transform rulebookTargetViewPoint;
    public Transform customiseTargetViewPoint;

    [Header("Transition Settings")]
    public float transitionDuration = 0.5f;

    [Header("Handheld Motion")]
    public float handheldIntensity = 0.3f;
    public float handheldFrequency = 1f;

    [Header("Orbit Camera Script")]
    public CameraRotate orbitScript;

    [Header("UI Panels")]
    public GameObject rulebookConnectPage;
    public GameObject rulebookLobbyPage;
    public GameObject rulebookConnectUI;
    //public GameObject rulebookLobbyUI;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isTransitioning = false;

    private enum CameraMode { Orbit, Rulebook, Customise }
    private CameraMode currentMode = CameraMode.Orbit;

    void Start()
    {
        rulebookConnectPage.SetActive(true);
        rulebookConnectUI.SetActive(false);
        rulebookLobbyPage.SetActive(false);

        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void LockCursor(bool locked)
    {
        if (locked)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void GoToRulebookView()
    {
        if (!isTransitioning)
        {
            LockCursor(false);
            StartCoroutine(LerpToTarget(rulebookTargetViewPoint.position, rulebookTargetViewPoint.rotation, CameraMode.Rulebook));
        }
    }

    public void GoToCustomiseView()
    {
        if (!isTransitioning)
        {
            LockCursor(false);
            StartCoroutine(LerpToTarget(customiseTargetViewPoint.position, customiseTargetViewPoint.rotation, CameraMode.Customise));
        }
    }

    public void ReturnToOrbit()
    {
        if (!isTransitioning)
        {
            rulebookConnectUI.SetActive(false);
            LockCursor(true);
            StartCoroutine(ReturnToOrbitRoutine());
        }
    }

    private IEnumerator LerpToTarget(Vector3 endPos, Quaternion endRot, CameraMode mode)
    {
        isTransitioning = true;
        orbitScript.canRotate = false;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float timer = 0f;

        while (timer < transitionDuration)
        {
            float t = timer / transitionDuration;

            Vector3 interpolatedPos = Vector3.Lerp(startPos, endPos, t);
            Quaternion interpolatedRot = Quaternion.Slerp(startRot, endRot, t);

            //Add Handheld Shake Effect
            float offsetX = (Mathf.PerlinNoise(Time.time * handheldFrequency, 0f) - 0.5f) * handheldIntensity;
            float offsetY = (Mathf.PerlinNoise(0f, Time.time * handheldFrequency) - 0.5f) * handheldIntensity;

            transform.position = interpolatedPos;
            transform.rotation = interpolatedRot * Quaternion.Euler(offsetY, offsetX, 0f);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;

        currentMode = mode;

        if(currentMode == CameraMode.Rulebook)
        {
            rulebookConnectUI.SetActive(true);
        }

        isTransitioning = false;
    }

    private IEnumerator ReturnToOrbitRoutine()
    {
        isTransitioning = true;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float timer = 0f;

        while (timer < transitionDuration)
        {
            float t = timer / transitionDuration;

            Vector3 interpolatedPos = Vector3.Lerp(startPos, originalPosition, t);
            Quaternion interpolatedRot = Quaternion.Slerp(startRot, originalRotation, t);

            //Add Handheld Shake Effect
            float offsetX = (Mathf.PerlinNoise(Time.time * handheldFrequency, 0f) - 0.5f) * handheldIntensity;
            float offsetY = (Mathf.PerlinNoise(0f, Time.time * handheldFrequency) - 0.5f) * handheldIntensity;

            transform.position = interpolatedPos;
            transform.rotation = interpolatedRot * Quaternion.Euler(offsetY, offsetX, 0f);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        currentMode = CameraMode.Orbit;
        orbitScript.canRotate = true;
        isTransitioning = false;
    }
}
