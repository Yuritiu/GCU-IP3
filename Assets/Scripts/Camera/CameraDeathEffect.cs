using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class CameraDeathEffect : MonoBehaviour
{
    public static CameraDeathEffect Instance;

    [Header("References")]
    [SerializeField] GameObject camera;
    [SerializeField] GameObject tableHit;
    [SerializeField] Image fadeImage;
    [SerializeField] GameObject LoseScreen;

    [Header("Audio Sources")]
    [SerializeField] AudioSource deathAudioSource;
    [SerializeField] AudioSource tableHitAudioSource1;
    [SerializeField] AudioSource tableHitAudioSource2;
    [SerializeField] AudioSource playerCoughAudioSource;

    float fallSpeed = 2f;
    float fadeSpeed = 1f;

    bool gameEnded = false;

    void Awake()
    {
        Instance = this;
    }

    public void TriggerDeathSequence()
    {
        if (!gameEnded)
        {
            gameEnded = true;

            StartCoroutine(DeathSequence());
        }
    }

    IEnumerator DeathSequence()
    {
        Vector3 startPosition = camera.transform.position;
        Quaternion startRotation = camera.transform.rotation;
        Vector3 endPosition = tableHit.transform.position;
        Quaternion endRotation = Quaternion.Euler(90, startRotation.eulerAngles.y, 0);

        playerCoughAudioSource.Play();
        BloodlossSystem.Instance.heartbeat.Stop();
        BloodlossSystem.Instance.heartbeatfast.Stop();

        float time = 0f;
        while (time < 1f)
        {

            time += Time.deltaTime * fallSpeed;
            //Camera Hit Table
            camera.transform.position = Vector3.Lerp(startPosition, endPosition, time);
            camera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, time);

            yield return null;
        }

        tableHitAudioSource1.Play();
        tableHitAudioSource2.Play();

        time = 0f;
        fadeImage.gameObject.SetActive(true);
        Color fadeColour = fadeImage.color;
        while (time < 2.5f)
        {
            time += Time.deltaTime * fadeSpeed;
            //Fade To Black
            fadeImage.color = new Color(fadeColour.r, fadeColour.g, fadeColour.b, time);

            yield return null;
        }

        EndGameLose();
    }

    void EndGameLose()
    {
        deathAudioSource.Play();

        LoseScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0f;
    }
}
