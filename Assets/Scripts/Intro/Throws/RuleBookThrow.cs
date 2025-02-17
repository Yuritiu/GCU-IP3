using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleBookThrow : MonoBehaviour
{
    [Header("Throw Variables")]
    [SerializeField] GameObject targetPosition;
    [SerializeField] float throwDuration;
    [SerializeField] float throwDelay;
    Quaternion startRotation;

    [Header("References")]
    [SerializeField] AudioSource rulebookThrowAudioSource;

    void Start()
    {
        startRotation = transform.rotation;
        StartCoroutine(ThrowRuleBookToTable(gameObject, targetPosition.transform.position, targetPosition.transform.rotation, throwDuration, throwDelay));
    }

    IEnumerator ThrowRuleBookToTable(GameObject rulebook, Vector3 targetPosition, Quaternion targetRotation, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 startPosition = rulebook.transform.position;
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

            rulebook.transform.position = currentPosition;
            rulebook.transform.rotation = currentRotation;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rulebookThrowAudioSource.Play();

        rulebook.transform.position = targetPosition;
        rulebook.transform.rotation = targetRotation;
    }
}
