using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] GameObject cardView;
    [SerializeField] GameObject opponentView;
    float lerpDuration = 0.1f;

    bool isLerping = false;
    Quaternion lastRotation;
    bool viewingCard = false;

    void Start()
    {
        lastRotation = transform.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isLerping)
        {
            Quaternion targetRotation;

            if (!viewingCard)
            {
                lastRotation = transform.rotation;

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
        lastRotation = transform.rotation; 
        float timeElapsed = 0f;

        while (timeElapsed < lerpDuration)
        {
            transform.rotation = Quaternion.Lerp(lastRotation, targetRotation, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        isLerping = false;
    }
}
