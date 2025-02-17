using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunThrow : MonoBehaviour
{
    [Header("Throw Variables")]
    [SerializeField] GameObject targetPosition;
    [SerializeField] float throwDuration;
    [SerializeField] float throwDelay;
    Quaternion startRotation;

    [Header("References")]
    [SerializeField] ShootScript shootScript;
    [SerializeField] AudioSource gunThrowAudioSource;

    void Start()
    {
        shootScript.gunAnim.enabled = false;
        startRotation = transform.rotation;
        StartCoroutine(ThrowGunToTable(gameObject, targetPosition.transform.position, targetPosition.transform.rotation, throwDuration, throwDelay));
    }

    IEnumerator ThrowGunToTable(GameObject gun, Vector3 targetPosition, Quaternion targetRotation, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 startPosition = gun.transform.position;
        float elapsedTime = 0f;

        //Move Card With A Parabolic Arc (Throw)
        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;

            //Throwing Arc Height
            float arcHeight = Mathf.Sin(progress * Mathf.PI) * 0.2f;
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            currentPosition.y += arcHeight;

            Quaternion currentRotation = Quaternion.Lerp(startRotation, targetRotation, progress);

            gun.transform.position = currentPosition;
            gun.transform.rotation = currentRotation;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gunThrowAudioSource.Play();

        gun.transform.position = targetPosition;
        gun.transform.rotation = targetRotation;

        shootScript.gunAnim.enabled = true;
        shootScript.gunAnim.Play("idle");
    }
}
