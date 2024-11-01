using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public Transform target;

    public float distance = 10f;
    public float height = 5f;
    public float rotationSpeed = 10f;
    public float angle = 20f;

    private float directionMultiplier;

    void Start()
    {
        directionMultiplier = Random.value < 0.5f ? -1f : 1f;
        Vector3 offset = new Vector3(0, height, -distance);
        transform.position = target.position + offset;

        transform.LookAt(target.position + Vector3.down * Mathf.Tan(angle * Mathf.Deg2Rad) * distance);
    }

    void Update()
    {
        transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime * directionMultiplier);

        transform.LookAt(target.position + Vector3.down * Mathf.Tan(angle * Mathf.Deg2Rad) * distance);
    }
}
