using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleBookThrow : MonoBehaviour
{
    //TODO: THROW RULE BOOK FROM BARTENDER ONTO TABLE AFTER CARDS HAVE BEEN FANNED (REALISTIC THROW WITH BOUNCE), ALSO DO THE SAME WITH GUN BUT HAVE OPPONENT THROW ONTO TABLE (HAVE IT SPIN ON LANDING ON TABLE)

    [Header("Throw Variables")]
    [SerializeField] GameObject targetPosition;
    [SerializeField] float throwDuration;
    [SerializeField] float throwDelay;
    Quaternion startRotation;

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

        rulebook.transform.position = targetPosition;
        rulebook.transform.rotation = targetRotation;
    }
}
