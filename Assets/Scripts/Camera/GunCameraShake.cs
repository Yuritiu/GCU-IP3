using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeAmount = 0.01f;

    [Header("Camera Settings")]
    [SerializeField] private Camera camera;

    [SerializeField] private Vector3 originalPosition;

    private bool isShaking = false;

    private void Start()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        originalPosition = camera.transform.localPosition;
    }

    void Update()
    {
        if (isShaking)
        {
            camera.transform.localPosition = originalPosition + Random.insideUnitSphere * shakeAmount;
        }
    }

    public void StartShake()
    {
        isShaking = true;
    }

    public void StopShake()
    {
        isShaking = false;
        camera.transform.localPosition = originalPosition; // Reset position when shaking stops
    }
}
