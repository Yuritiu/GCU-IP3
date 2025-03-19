using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeAmount = 0.01f;

    [Header("Parent Object Settings")]
    [SerializeField] private Transform parentObject;

    [SerializeField] private Vector3 originalPosition;

    public bool isShaking = false;

    private void Start()
    {
        if (parentObject == null)
        {
            parentObject = transform; // Default to the object's own transform if not assigned
        }
        originalPosition = parentObject.localPosition;
    }

    void Update()
    {
        if (isShaking)
        {
            parentObject.localPosition = originalPosition + Random.insideUnitSphere * shakeAmount;
        }
    }

    public void StartShake()
    {
        isShaking = true;
    }

    public void StopShake()
    {
        isShaking = false;
        parentObject.localPosition = originalPosition; // Reset position when shaking stops
    }
}
