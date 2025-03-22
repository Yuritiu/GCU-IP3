using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    public static CardView Instance;

    [SerializeField] GameObject cardView;
    [SerializeField] GameObject opponentView;
    float lerpDuration = 0.1f;

    bool isLerping = false;
    Quaternion lastRotation;
    public bool viewingCard = false;

    private Tutorial tutorial;
    public bool canLerp = true;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        lastRotation = transform.rotation;
        tutorial = FindFirstObjectByType<Tutorial>();
    }

    void Update()
    {
        if (!CameraController.Instance.isCameraMovementUnlocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            CameraController.Instance.cameraLocked = false;
            //viewingCard = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isLerping && CameraController.Instance.isCameraMovementUnlocked)
        {
            Quaternion targetRotation;

            if (!viewingCard)
            {
                if (!isLerping)
                {
                    lastRotation = transform.rotation;
                }

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                targetRotation = Quaternion.LookRotation(cardView.transform.position - transform.position);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                targetRotation = lastRotation;
            }

            StartCoroutine(LerpCameraRotation(targetRotation));
            viewingCard = !viewingCard;
        }
    }

    IEnumerator LerpCameraRotation(Quaternion targetRotation)
    {
        isLerping = true;
        Quaternion startRotation = transform.rotation;
        float timeElapsed = 0f;

        while (timeElapsed < lerpDuration)
        {
            float progress = timeElapsed / lerpDuration;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, progress);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        isLerping = false;

        //Update lastRotation ONLY IF Returning To Previous View
        if (!viewingCard)
        {
            lastRotation = transform.rotation;
        }
    }
}
