using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 0.5f;
    public float shakeStrength = 0.05f;
    public float shakeFrequency = 1.0f;
    public AnimationCurve shakeAnimCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Debug - Key: F")]
    public bool testKey = true;

    private float shakeTimer = 0f;
    private float initialShakeDuration;
    private Vector3 originalPosition;

    public bool shouldShake = false;

    void Start()
    {
        originalPosition = transform.localPosition;
        initialShakeDuration = shakeDuration;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            float normalizedTime = 1 - (shakeTimer / initialShakeDuration);
            float currentStrength = shakeStrength * shakeAnimCurve.Evaluate(normalizedTime);

            transform.localPosition = originalPosition + Random.insideUnitSphere * currentStrength;
            shakeTimer -= Time.deltaTime * shakeFrequency;
        }

        //Test when F is pressed
        //if(testKey)
        //{
        //    if (Input.GetKeyDown(KeyCode.F))
        //    {
        //        TriggerShake();
        //    }
        //}

        //-- EXAMPLE -- HOW TO REFERENCE IN OTHER OBJECTS BELOW
        //START(): CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
        //ANYWHERE(): cameraShake.TriggerShake(1.0f, 1.2f, 1.5f);
}

    public void TriggerShake(float duration = -1, float strength = -1, float frequency = -1)
    {
        shakeTimer = (duration > 0) ? duration : shakeDuration;
        initialShakeDuration = shakeTimer;
        shakeStrength = (strength > 0) ? strength : shakeStrength;
        shakeFrequency = (frequency > 0) ? frequency : shakeFrequency;
    }
}
