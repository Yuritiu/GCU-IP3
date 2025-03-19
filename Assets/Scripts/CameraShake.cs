using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private float shakeTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (shakeDuration > 0)
        {
            // Generate a gentle shake
            float xShake = Random.Range(-1f, 1f) * shakeMagnitude;
            float yShake = Random.Range(-1f, 1f) * shakeMagnitude;

            // Apply shake to the camera's position
            transform.position = originalPosition + new Vector3(xShake, yShake, 0);

            // Decrease the shake duration and magnitude
            shakeTime += Time.deltaTime;
            shakeMagnitude = Mathf.Lerp(shakeMagnitude, 0, shakeTime / shakeDuration);
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            // Reset camera position once shake is finished
            transform.position = originalPosition;
        }
    }

    // Method to start the shake
    public void StartShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeTime = 0f;
        originalPosition = transform.position;
    }
}
