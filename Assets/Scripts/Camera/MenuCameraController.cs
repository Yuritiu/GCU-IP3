using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    public static MenuCameraController Instance;

    [Header("References")]
    [SerializeField] MultiplayerManager multiplayerManager;

    [Header("Target References")]
    public Transform rulebookTargetViewPoint;
    public Transform customiseTargetViewPoint;

    [Header("Transition Settings")]
    public float transitionDuration = 0.5f;

    [Header("Orbit Camera Script")]
    public CameraRotate orbitScript;

    [Header("UI Panels")]
    public GameObject mainMenuCanvas;
    public GameObject customisationCanvas;
    public GameObject rulebookConnectPage;
    public GameObject rulebookLobbyPage;
    public GameObject rulebookConnectUI;
    //public GameObject rulebookLobbyUI;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public bool isTransitioning = false;

    public enum CameraMode { Orbit, Rulebook, Customise }
    public CameraMode currentMode = CameraMode.Orbit;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rulebookConnectPage.SetActive(true);
        rulebookConnectUI.SetActive(false);
        rulebookLobbyPage.SetActive(false);
        customisationCanvas.SetActive(false);

        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (LobbyState.InLobby) return;

        if (Input.GetKeyDown(KeyCode.Escape) && !isTransitioning)
        {
            if (currentMode == CameraMode.Rulebook)
            {
                if (rulebookConnectPage)
                {
                    rulebookConnectUI.SetActive(false);
                    ReturnToOrbit();
                }
            }
        }
    }

    public void GoToRulebookView()
    {
        if (!isTransitioning)
        {
            customisationCanvas.SetActive(false);
            transitionDuration = 0.5f;
            StartCoroutine(LerpToTarget(rulebookTargetViewPoint.position, rulebookTargetViewPoint.rotation, CameraMode.Rulebook));
        }
    }

    public void GoToCustomiseView()
    {
        if (!isTransitioning)
        {
            transitionDuration = 0.75f;
            StartCoroutine(LerpToTarget(customiseTargetViewPoint.position, customiseTargetViewPoint.rotation, CameraMode.Customise));
        }
    }

    public void ReturnToOrbit()
    {
        if (!isTransitioning)
        {
            customisationCanvas.SetActive(false);
            rulebookConnectUI.SetActive(false);
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

            transform.position = interpolatedPos;
            transform.rotation = interpolatedRot;

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

        if(currentMode == CameraMode.Customise)
        {
            customisationCanvas.SetActive(true);
        }

        isTransitioning = false;
    }

    private IEnumerator ReturnToOrbitRoutine()
    {
        isTransitioning = true;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        // --- Calculate correct orbit position & rotation manually ---
        Transform orbitTarget = orbitScript.target;
        float distance = orbitScript.distance;
        float height = orbitScript.height;
        float angle = orbitScript.angle;

        Vector3 offset = new Vector3(0f, height, -distance);
        Vector3 endPos = orbitTarget.position + offset;
        Quaternion endRot = Quaternion.LookRotation(
            orbitTarget.position + Vector3.down * Mathf.Tan(angle * Mathf.Deg2Rad) * distance - endPos
        );

        float timer = 0f;

        while (timer < transitionDuration)
        {
            float t = timer / transitionDuration;

            Vector3 interpolatedPos = Vector3.Lerp(startPos, endPos, t);
            Quaternion interpolatedRot = Quaternion.Slerp(startRot, endRot, t);

            transform.position = interpolatedPos;
            transform.rotation = interpolatedRot;

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;

        mainMenuCanvas.SetActive(true);

        currentMode = CameraMode.Orbit;
        orbitScript.canRotate = true;
        isTransitioning = false;
    }
}