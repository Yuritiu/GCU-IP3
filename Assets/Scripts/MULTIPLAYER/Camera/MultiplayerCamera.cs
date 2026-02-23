using UnityEngine;
using Unity.Netcode;

public class MultiplayerCamera : NetworkBehaviour
{
    [Header("Clamp Angles")]
    public Vector2 xClamp = new Vector2(-75f, 75f);
    public Vector2 yClamp = new Vector2(-75f, 75f);

    private Vector2 currentRotation;
    private float sensitivity;

    [HideInInspector] public bool cameraLocked = false;

    [Header("Camera Positions")]
    [SerializeField] private Transform knifeTarget;
    [SerializeField] private Transform opponentTarget;

    private void Start()
    {
        if (!IsOwner)
        {
            enabled = false; //Only local player can control camera
            return;
        }

        //Load sensitivity from PlayerPrefs
        sensitivity = PlayerPrefs.HasKey("cameraSensitivity")
            ? PlayerPrefs.GetFloat("cameraSensitivity")
            : 100f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentRotation = new Vector2(transform.eulerAngles.y, transform.eulerAngles.x);
    }

    private void Update()
    {
        if (cameraLocked || !IsOwner) return;

        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        currentRotation.x += mouseX;
        currentRotation.y -= mouseY;

        currentRotation.x = Mathf.Clamp(currentRotation.x, xClamp.x, xClamp.y);
        currentRotation.y = Mathf.Clamp(currentRotation.y, yClamp.x, yClamp.y);

        transform.localRotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0f);
    }

    //Runtime sensitivity update
    public void SetSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
        PlayerPrefs.SetFloat("cameraSensitivity", newSensitivity);
        PlayerPrefs.Save();
    }

    //Turn based camera functions
    public void SetCameraToKnifeTarget()
    {
        if (knifeTarget == null) return;

        cameraLocked = true;
        StartCoroutine(LerpCameraToTarget(knifeTarget.position, knifeTarget.rotation, 0.5f));
    }

    public void SetCameraToOpponentTarget()
    {
        if (opponentTarget == null) return;

        cameraLocked = true;
        StartCoroutine(LerpCameraToTarget(opponentTarget.position, opponentTarget.rotation, 0.5f));
    }

    private System.Collections.IEnumerator LerpCameraToTarget(Vector3 targetPos, Quaternion targetRot, float duration)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;

        cameraLocked = false;
    }
}