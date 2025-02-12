using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeThrow : MonoBehaviour
{
    [Header("Throw Variables")]
    [SerializeField] GameObject targetPosition;
    GameObject knifeGO;
    [SerializeField] float throwDuration;
    [SerializeField] float throwDelay;
    [SerializeField] Animator animator;
    Quaternion startRotation;

    Quaternion targetRotation;

    void Start()
    {
        startRotation = transform.rotation;
        StartCoroutine(ThrowKnifeToTable(gameObject, targetPosition.transform.position, targetPosition.transform.rotation, throwDuration, throwDelay));
    }

    IEnumerator ThrowKnifeToTable(GameObject knife, Vector3 targetPosition, Quaternion targetRotation, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);
        knifeGO = knife;
        Vector3 startPosition = knife.transform.position;
        float elapsedTime = 0f;

        //Move Card With A Parabolic Arc (Throw)
        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;

            //Throwing Arc Height
            float arcHeight = Mathf.Sin(progress * Mathf.PI) * 1.2f;
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            currentPosition.y += arcHeight;

            Quaternion currentRotation = Quaternion.Lerp(startRotation, targetRotation, progress);

            knife.transform.position = currentPosition;
            knife.transform.rotation = currentRotation;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        knife.transform.position = targetPosition;
        knife.transform.rotation = targetRotation;

        animator.enabled = true;
        animator.SetBool("Wiggle", true);

        StartCoroutine(StopAnimator());
    }

    IEnumerator StopAnimator()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("Wiggle", false);
        animator.enabled = false;

        knifeGO.transform.position = targetPosition.transform.position;
        knifeGO.transform.rotation = targetRotation;
    }
}
